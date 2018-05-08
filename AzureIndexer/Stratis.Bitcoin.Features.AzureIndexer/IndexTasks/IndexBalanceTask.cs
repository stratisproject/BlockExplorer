using System.Collections.Generic;
using NBitcoin;

namespace Stratis.Bitcoin.Features.AzureIndexer.IndexTasks
{
    public class IndexBalanceTask : IndexTableEntitiesTaskBase<OrderedBalanceChange>
    {
        private readonly WalletRuleEntryCollection walletRules;
        public IndexBalanceTask(IndexerConfiguration conf, WalletRuleEntryCollection walletRules)
            : base(conf)
        {
            this.walletRules = walletRules;
        }
        protected override Microsoft.WindowsAzure.Storage.Table.CloudTable GetCloudTable()
        {
            return Configuration.GetBalanceTable();
        }

        protected override Microsoft.WindowsAzure.Storage.Table.ITableEntity ToTableEntity(OrderedBalanceChange item)
        {
            return item.ToEntity();
        }

        protected override bool SkipToEnd => walletRules != null && walletRules.Count == 0;

        protected override void ProcessBlock(BlockInfo block, BulkImport<OrderedBalanceChange> bulk)
        {
            foreach (var tx in block.Block.Transactions)
            {
                var txId = tx.GetHash();

                var entries = Extract(txId, tx, block.BlockId, block.Block.Header, block.Height);
                foreach (var entry in entries)
                {
                    bulk.Add(entry.PartitionKey, entry);
                }
            }
        }

        private IEnumerable<OrderedBalanceChange> Extract(uint256 txId, Transaction tx, uint256 blockId, BlockHeader blockHeader, int height)
        {
            if (walletRules != null)
                return OrderedBalanceChange.ExtractWalletBalances(txId, tx, blockId, blockHeader, height, walletRules);
            else
                return OrderedBalanceChange.ExtractScriptBalances(txId, tx, blockId, blockHeader, height);
        }
    }
}

