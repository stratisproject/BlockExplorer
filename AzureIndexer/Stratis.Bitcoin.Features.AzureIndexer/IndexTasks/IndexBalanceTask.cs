using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using NBitcoin;

namespace Stratis.Bitcoin.Features.AzureIndexer.IndexTasks
{
    public class IndexBalanceTask : IndexTableEntitiesTaskBase<OrderedBalanceChange>
    {
        private readonly ILogger logger;

        WalletRuleEntryCollection _WalletRules;
        public IndexBalanceTask(IndexerConfiguration conf, WalletRuleEntryCollection walletRules, ILoggerFactory loggerFactory)
            : base(conf, loggerFactory)
        {
            _WalletRules = walletRules;
            this.logger = loggerFactory.CreateLogger(GetType().FullName);
        }
        protected override Microsoft.WindowsAzure.Storage.Table.CloudTable GetCloudTable()
        {
            return Configuration.GetBalanceTable();
        }

        protected override Microsoft.WindowsAzure.Storage.Table.ITableEntity ToTableEntity(OrderedBalanceChange item)
        {
            return item.ToEntity();
        }

        protected override bool SkipToEnd
        {
            get
            {
                return _WalletRules != null && _WalletRules.Count == 0;
            }
        }

        protected override void ProcessBlock(BlockInfo block, BulkImport<OrderedBalanceChange> bulk, Network network)
        {
            this.logger.LogTrace("()");

            foreach (var tx in block.Block.Transactions)
            {
                var txId = tx.GetHash();

                var entries = extract(txId, tx, block.BlockId, block.Block.Header, block.Height, network);
                foreach (var entry in entries)
                {
                    bulk.Add(entry.PartitionKey, entry);
                }
            }

            this.logger.LogTrace("(-)");
        }

        private IEnumerable<OrderedBalanceChange> extract(uint256 txId, Transaction tx, uint256 blockId, BlockHeader blockHeader, int height, Network network)
        {
            if (_WalletRules != null)
                return OrderedBalanceChange.ExtractWalletBalances(txId, tx, blockId, blockHeader, height, _WalletRules, network);
            else
                return OrderedBalanceChange.ExtractScriptBalances(txId, tx, blockId, blockHeader, height, network);
        }
    }
}

