using System.IO;
using System.Net;
using System.Text;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using AzureIndexer.Api.Models;
using AzureIndexer.Api.Notifications;
using NBitcoin.DataEncoders;
using Newtonsoft.Json;
using Stratis.Bitcoin.P2P.Protocol.Payloads;
using Stratis.Bitcoin.Utilities.JsonConverters;
using Stratis.Features.AzureIndexer.Entities;

namespace AzureIndexer.Api.Controllers
{
    using System.Threading.Tasks;
    using AutoMapper;
    using Infrastructure;
    using Microsoft.AspNetCore.Mvc;
    using Models.Response;
    using NBitcoin;

    [Route("api/v1/transactions")]
    public class TransactionsController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly ITransactionSearchService transactionSearchService;
        private readonly ISmartContractSearchService smartContractSearchService;
        private readonly ServiceBusClient serviceBusClient;
        private readonly ServiceBusAdministrationClient serviceBusAdministrationClient;

        public TransactionsController(
            ChainIndexer chain,
            QBitNinjaConfiguration config,
            IMapper mapper,
            ITransactionSearchService transactionSearchService,
            ISmartContractSearchService smartContractSearchService,
            ServiceBusClient serviceBusClient,
            ServiceBusAdministrationClient serviceBusAdministrationClient)
        {
            this.mapper = mapper;
            this.transactionSearchService = transactionSearchService;
            this.smartContractSearchService = smartContractSearchService;
            this.Configuration = config;
            this.Chain = chain;
            this.serviceBusClient = serviceBusClient;
            this.serviceBusAdministrationClient = serviceBusAdministrationClient;
        }

        public ChainIndexer Chain { get; set; }

        public QBitNinjaConfiguration Configuration { get; set; }

        public Network Network => this.Configuration.Indexer.Network;

        [HttpGet]
        [Route("{txId}")]
        public async Task<TransactionSummaryModel> Transaction(string txId, bool colored = false, bool loadSmartContractIfExists = false)
        {
            var response = await this.transactionSearchService.FindTransaction(uint256.Parse(txId), colored);
            var mappedResponse = this.mapper.Map<TransactionSummaryModel>(response);

            if (loadSmartContractIfExists) // TODO: put this in once we have real data && mappedResponse.IsSmartContract)
            {
                mappedResponse.SmartContract =
                    await this.smartContractSearchService.FindSmartContract(uint256.Parse(response.TransactionId));
            }

            return mappedResponse;
        }

        [HttpPost]
        [Route("broadcast")]
        public async Task<BroadcastResponse> Broadcast()
        {
            Transaction tx = null;
            switch (this.Request.ContentType)
            {
                case "application/json":
                    using (StreamReader stream = new StreamReader(this.Request.Body, Encoding.UTF8))
                    {
                        string body = await stream.ReadToEndAsync();
                        tx = NBitcoin.Transaction.Parse(body, RawFormat.BlockExplorer, this.Configuration.Indexer.Network);
                    }

                    break;
                case "application/octet-stream":
                    using (StreamReader stream = new StreamReader(this.Request.Body, Encoding.UTF8))
                    {
                        string body = await stream.ReadToEndAsync();
                        tx = new Transaction();
                        tx.FromBytes(Encoders.Hex.DecodeData(body));
                    }

                    break;

                default:
                    throw new HttpResponseException("UnsupportedMediaType", HttpStatusCode.UnsupportedMediaType);
            }

            var sender = this.serviceBusClient.CreateSender(nameof(BroadcastedTransaction));

            await this.EnsureTopicExists();

            var payload = Serializer.ToString(new BroadcastedTransaction(tx), this.Configuration.Indexer.Network);

            await sender.SendMessageAsync(new ServiceBusMessage(payload));

            var hash = tx.GetHash();
            for (int i = 0; i < 10; i++)
            {
                var indexed = await this.Configuration.Indexer.CreateIndexerClient().GetTransactionAsync(hash);
                if (indexed != null)
                {
                    return new BroadcastResponse
                    {
                        Success = true
                    };
                }

                var reject = await this.Configuration.GetRejectTable().ReadOneAsync(hash.ToString());
                if (reject != null)
                {
                    return new BroadcastResponse
                    {
                        Success = false,
                        Error = new BroadcastError
                        {
                            ErrorCode = reject.Code,
                            Reason = reject.Reason
                        }
                    };
                }

                await Task.Delay(100 * i);
            }

            return new BroadcastResponse
            {
                Success = true,
                Error = new BroadcastError
                {
                    ErrorCode = RejectCode.INVALID,
                    Reason = "Unknown"
                }
            };
        }

        private async Task EnsureTopicExists()
        {
            try
            {
                await this.serviceBusAdministrationClient.GetTopicAsync(nameof(BroadcastedTransaction));
            }
            catch (ServiceBusException ex)
            {
                if (ex.Reason == ServiceBusFailureReason.MessagingEntityNotFound)
                {
                    await this.serviceBusAdministrationClient.CreateTopicAsync(nameof(BroadcastedTransaction));
                }
            }
        }
    }
}
