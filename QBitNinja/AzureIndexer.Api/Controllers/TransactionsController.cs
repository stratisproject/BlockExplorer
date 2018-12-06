namespace AzureIndexer.Api.Controllers
{
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using AutoMapper;
    using Infrastructure;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Models.Response;
    using NBitcoin;

    [Route("api/v1/transactions")]
    public class TransactionsController : ControllerBase
    {
        private readonly IMapper mapper;

        public TransactionsController(ConcurrentChain chain, QBitNinjaConfiguration config, IMapper mapper)
        {
            this.mapper = mapper;
            this.Configuration = config;
            this.Chain = chain;
        }

        public ConcurrentChain Chain { get; set; }

        public QBitNinjaConfiguration Configuration { get; set; }

        public Network Network => this.Configuration.Indexer.Network;

        [HttpGet]
        [Route("{txId}")]
        public async Task<TransactionResponseModel> Transaction(string txId, bool colored = false)
        {
            var response = await this.JsonTransaction(uint256.Parse(txId), colored);
            return response;
        }

        internal async Task<TransactionResponseModel> JsonTransaction(uint256 txId, bool colored)
        {
            var client = this.Configuration.Indexer.CreateIndexerClient();
            var tx = await client.GetTransactionAsync(true, colored, txId);
            if (tx == null || (tx.ColoredTransaction == null && colored) || tx.SpentCoins == null)
            {
                throw new HttpResponseException(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound,
                    ReasonPhrase = "Transaction not found"
                });
            }

            var response = new TransactionResponse()
            {
                TransactionId = tx.TransactionId,
                Transaction = tx.Transaction,
                IsCoinbase = tx.Transaction.IsCoinBase,
                Fees = tx.Fees,
                Block = this.FetchBlockInformation(tx.BlockIds),
                FirstSeen = tx.FirstSeen
            };

            for (int i = 0; i < tx.Transaction.Outputs.Count; i++)
            {
                var txout = tx.Transaction.Outputs[i];
                ICoin coin = new Coin(new OutPoint(txId, i), txout);
                if (colored)
                {
                    var entry = tx.ColoredTransaction.GetColoredEntry((uint)i);
                    if (entry != null)
                    {
                        coin = new ColoredCoin(entry.Asset, (Coin)coin);
                    }
                }

                response.ReceivedCoins.Add(coin);
            }

            if (!response.IsCoinbase)
            {
                for (int i = 0; i < tx.Transaction.Inputs.Count; i++)
                {
                    ICoin coin = new Coin(tx.SpentCoins[i].OutPoint, tx.SpentCoins[i].TxOut);
                    if (colored)
                    {
                        var entry = tx.ColoredTransaction.Inputs.FirstOrDefault(ii => ii.Index == i);
                        if (entry != null)
                        {
                            coin = new ColoredCoin(entry.Asset, (Coin)coin);
                        }
                    }

                    response.SpentCoins.Add(coin);
                }
            }

            return this.mapper.Map<TransactionResponseModel>(response);
        }

        private BlockInformation FetchBlockInformation(uint256[] blockIds)
        {
            var confirmed = blockIds.Select(this.Chain.GetBlock).FirstOrDefault(b => b != null);
            if (confirmed == null)
            {
                return null;
            }

            return new BlockInformation
            {
                BlockId = confirmed.HashBlock,
                BlockHeader = confirmed.Header,
                Confirmations = this.Chain.Tip.Height - confirmed.Height + 1,
                Height = confirmed.Height,
                MedianTimePast = confirmed.GetMedianTimePast(),
                BlockTime = confirmed.Header.BlockTime
            };
        }
    }
}
