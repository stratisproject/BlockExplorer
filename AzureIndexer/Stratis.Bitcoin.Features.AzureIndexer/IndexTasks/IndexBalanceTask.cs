namespace Stratis.Bitcoin.Features.AzureIndexer.IndexTasks
{
    using System.Collections.Generic;
    using Microsoft.Extensions.Logging;
    using NBitcoin;

    public class IndexBalanceTask : IndexTableEntitiesTaskBase<OrderedBalanceChange>
    {
        private readonly ILogger logger;

        WalletRuleEntryCollection _WalletRules;

        public IndexBalanceTask(IndexerConfiguration conf, WalletRuleEntryCollection walletRules, ILoggerFactory loggerFactory)
            : base(conf, loggerFactory)
        {
            this._WalletRules = walletRules;
            this.logger = loggerFactory.CreateLogger(this.GetType().FullName);
        }

        protected override Microsoft.WindowsAzure.Storage.Table.CloudTable GetCloudTable()
        {
            return this.Configuration.GetBalanceTable();
        }

        protected override Microsoft.WindowsAzure.Storage.Table.ITableEntity ToTableEntity(OrderedBalanceChange item)
        {
            return item.ToEntity();
        }

        protected override bool SkipToEnd
        {
            get
            {
                return this._WalletRules != null && this._WalletRules.Count == 0;
            }
        }

        protected override void ProcessBlock(BlockInfo block, BulkImport<OrderedBalanceChange> bulk, Network network, BulkImport<SmartContactEntry.Entity> smartContractBulk = null)
        {
            this.logger.LogTrace("()");

            foreach (Transaction tx in block.Block.Transactions)
            {
                uint256 txId = tx.GetHash();

                IEnumerable<OrderedBalanceChange> entries = this.Extract(txId, tx, block.BlockId, block.Block.Header, block.Height, network);
                foreach (OrderedBalanceChange entry in entries)
                {
                    bulk.Add(entry.PartitionKey, entry);
                }
            }

            this.logger.LogTrace("(-)");
        }

        private IEnumerable<OrderedBalanceChange> Extract(uint256 txId, Transaction tx, uint256 blockId, BlockHeader blockHeader, int height, Network network)
        {
            if (this._WalletRules != null)
            {
                return OrderedBalanceChange.ExtractWalletBalances(txId, tx, blockId, blockHeader, height, this._WalletRules, network);
            }
            else
            {
                return OrderedBalanceChange.ExtractScriptBalances(txId, tx, blockId, blockHeader, height, network);
            }
        }
    }
}