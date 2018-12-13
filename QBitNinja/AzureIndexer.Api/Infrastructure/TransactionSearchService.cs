namespace AzureIndexer.Api.Infrastructure
{
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using AutoMapper;
    using Models;
    using Models.Response;
    using NBitcoin;

    public class TransactionSearchService : ITransactionSearchService
    {
        private readonly ConcurrentChain chain;
        private readonly QBitNinjaConfiguration configuration;
        private readonly IMapper mapper;

        public TransactionSearchService(ConcurrentChain chain, QBitNinjaConfiguration configuration, IMapper mapper)
        {
            this.chain = chain;
            this.configuration = configuration;
            this.mapper = mapper;
        }

        public async Task<TransactionResponseModel> FindTransaction(uint256 txId, bool colored)
        {
            var client = this.configuration.Indexer.CreateIndexerClient();
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
            var confirmed = blockIds.Select(this.chain.GetBlock).FirstOrDefault(b => b != null);
            if (confirmed == null)
            {
                return null;
            }

            return new BlockInformation
            {
                BlockId = confirmed.HashBlock,
                BlockHeader = confirmed.Header,
                Confirmations = this.chain.Tip.Height - confirmed.Height + 1,
                Height = confirmed.Height,
                MedianTimePast = confirmed.GetMedianTimePast(),
                BlockTime = confirmed.Header.BlockTime
            };
        }
    }
}
