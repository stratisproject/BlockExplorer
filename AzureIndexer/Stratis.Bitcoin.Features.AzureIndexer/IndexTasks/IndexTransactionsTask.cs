using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using NBitcoin;

namespace Stratis.Bitcoin.Features.AzureIndexer.IndexTasks
{
    public class IndexTransactionsTask : IndexTableEntitiesTaskBase<TransactionEntry.Entity>
    {
        private readonly ILogger logger;

        public IndexTransactionsTask(IndexerConfiguration configuration, ILoggerFactory loggerFactory)
            : base(configuration, loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger(GetType().FullName);
        }

        protected override void ProcessBlock(BlockInfo block, BulkImport<TransactionEntry.Entity> bulk, Network network)
        {
            this.logger.LogTrace("()");

            foreach (var transaction in block.Block.Transactions)
            {
                var indexed = new TransactionEntry.Entity(null, transaction, block.BlockId);
                bulk.Add(indexed.PartitionKey, indexed);
            }

            this.logger.LogTrace("(-)");
        }

        protected override CloudTable GetCloudTable()
        {
            return Configuration.GetTransactionTable();
        }

        protected override ITableEntity ToTableEntity(TransactionEntry.Entity indexed)
        {
            return indexed.CreateTableEntity();
        }
    }
}
