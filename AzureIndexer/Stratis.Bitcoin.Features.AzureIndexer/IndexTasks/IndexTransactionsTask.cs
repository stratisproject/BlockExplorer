namespace Stratis.Bitcoin.Features.AzureIndexer.IndexTasks
{
    using Microsoft.Extensions.Logging;
    using Microsoft.WindowsAzure.Storage.Table;
    using NBitcoin;

    public class IndexTransactionsTask : IndexTableEntitiesTaskBase<TransactionEntry.Entity>
    {
        private readonly ILogger logger;
        private readonly IndexerConfiguration config;

        public IndexTransactionsTask(IndexerConfiguration configuration, ILoggerFactory loggerFactory)
            : base(configuration, loggerFactory)
        {
            this.config = configuration;
            this.logger = loggerFactory.CreateLogger(GetType().FullName);
        }

        protected override void ProcessBlock(BlockInfo block, BulkImport<TransactionEntry.Entity> bulk, Network network)
        {
            this.logger.LogTrace("()");

            foreach (var transaction in block.Block.Transactions)
            {
                var indexed = new TransactionEntry.Entity(null, transaction, block.BlockId);
                if (indexed.HasSmartContract)
                {
                    var scEntity = new SmartContactEntry.Entity(indexed);

                }

                bulk.Add(indexed.PartitionKey, indexed);
            }

            this.logger.LogTrace("(-)");
        }

        protected CloudTable GetSmartContractCloudTable()
        {
            return this.Configuration.GetSmartContactTable();
        }

        protected CloudTable GetSmartContractCloudDetailTable()
        {
            return this.Configuration.GetSmartContactDetailTable();
        }

        protected override CloudTable GetCloudTable()
        {
            return this.Configuration.GetTransactionTable();
        }

        protected override ITableEntity ToTableEntity(TransactionEntry.Entity indexed)
        {
            return indexed.CreateTableEntity(config.Network);
        }

        protected ITableEntity ToTableEntity(SmartContactEntry.Entity indexed)
        {
            return indexed.CreateTableEntity(config.Network);
        }
    }
}
