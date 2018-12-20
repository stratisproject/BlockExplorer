using System.Collections.Generic;

namespace Stratis.Bitcoin.Features.AzureIndexer.IndexTasks
{
    using Microsoft.Extensions.Logging;
    using Microsoft.WindowsAzure.Storage.Table;
    using NBitcoin;

    public class IndexTransactionsTask : IndexTableEntitiesTaskBase<TransactionEntry.Entity>
    {
        private readonly ILogger logger;
        private readonly IndexerConfiguration config;
        private IndexTableEntitiesTaskBase<TransactionEntry.Entity> _indexTableEntitiesTaskBaseImplementation;

        public IndexTransactionsTask(IndexerConfiguration configuration, ILoggerFactory loggerFactory)
            : base(configuration, loggerFactory)
        {
            this.config = configuration;
            this.logger = loggerFactory.CreateLogger(GetType().FullName);
        }

        protected override void ProcessBlock(BlockInfo block, BulkImport<TransactionEntry.Entity> bulk, Network network, BulkImport<SmartContactEntry.Entity> smartContractBulk = null)
        {
            this.logger.LogTrace("()");

            foreach (var transaction in block.Block.Transactions)
            {
                var indexed = new TransactionEntry.Entity(null, transaction, block.BlockId);
                if (indexed.HasSmartContract)
                {
                    var scEntity = new SmartContactEntry.Entity(indexed);
                    smartContractBulk.Add(scEntity.PartitionKey, scEntity);
                }

                bulk.Add(indexed.PartitionKey, indexed);
            }

            this.logger.LogTrace("(-)");
        }

        protected override void IndexCore(string partitionName, IEnumerable<TransactionEntry.Entity> items)
        {
            throw new System.NotImplementedException();
        }

        protected override void IndexCore(string partitionName, IEnumerable<TransactionEntry.Entity> items, string partitionName2, IEnumerable<IIndexed> item2)
        {
            throw new System.NotImplementedException();
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
