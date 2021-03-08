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

        public TransactionsController(
            ChainIndexer chain,
            QBitNinjaConfiguration config,
            IMapper mapper,
            ITransactionSearchService transactionSearchService,
            ISmartContractSearchService smartContractSearchService)
        {
            this.mapper = mapper;
            this.transactionSearchService = transactionSearchService;
            this.smartContractSearchService = smartContractSearchService;
            this.Configuration = config;
            this.Chain = chain;
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
    }
}
