namespace Stratis.Bitcoin.Features.AzureIndexer.IndexTasks
{
    using System.Collections.Generic;
    using Microsoft.Extensions.Logging;
    using Microsoft.WindowsAzure.Storage.Table;
    using NBitcoin;
    using Stratis.Bitcoin.Features.AzureIndexer.Entities;

    public class IndexTokensTask : IndexTableEntitiesTaskBase<AddressTokenTransactionEntry>
    {
        private readonly ILogger logger;

        public IndexTokensTask(IndexerConfiguration configuration, ILoggerFactory loggerFactory) 
            : base(configuration, loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger(this.GetType().FullName);
        }

        protected override CloudTable GetCloudTable()
        {
            return this.Configuration.GetAddressTokenTransactionTable();
        }

        protected override void ProcessBlock(BlockInfo block, BulkImport<AddressTokenTransactionEntry> bulkImport, Network network, BulkImport<SmartContactEntry.Entity> SmartContractBulk = null)
        {
            throw new System.NotImplementedException();
        }

        protected override ITableEntity ToTableEntity(AddressTokenTransactionEntry item)
        {
            return item;
        }
    }
}