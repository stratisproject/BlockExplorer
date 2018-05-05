using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage.Table;
using NBitcoin;
using Stratis.Bitcoin.Features.AzureIndexer.IndexTasks;
using Stratis.Bitcoin.Features.AzureIndexer.Internal;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Stratis.Bitcoin.Features.AzureIndexer
{
    public enum IndexerCheckpoints
    {
        Wallets,
        Transactions,
        Blocks,
        Balances
    }

    public class AzureIndexer
    {
        public static AzureIndexer CreateIndexer(IConfiguration config, ILoggerFactory loggerFactory)
        {
            var indexerConfig = new IndexerConfiguration(config, loggerFactory);
            return indexerConfig.CreateIndexer();
        }

        private readonly IndexerConfiguration _Configuration;

        private readonly ILoggerFactory loggerFactory;

        public IndexerConfiguration Configuration
        {
            get
            {
                return this._Configuration;
            }
        }

        public AzureIndexer(IndexerConfiguration configuration, ILoggerFactory loggerFactory)
        {
            this.loggerFactory = loggerFactory;

            if (configuration == null)
                throw new ArgumentNullException("configuration");
            this.TaskScheduler = TaskScheduler.Default;
            this.CheckpointInterval = TimeSpan.FromMinutes(15.0);
            this._Configuration = configuration;
            this.FromHeight = 0;
            this.ToHeight = int.MaxValue;
        }

        internal Checkpoint GetCheckpointInternal(IndexerCheckpoints checkpoint)
        {
            var chk = this.GetCheckpoint(checkpoint);
            if(this.IgnoreCheckpoints)
                chk = new Checkpoint(chk.CheckpointName, this.Configuration.Network, null, null, this.loggerFactory);
            return chk;
        }

        private void SetThrottling()
        {
            Helper.SetThrottling();
            ServicePoint tableServicePoint = ServicePointManager.FindServicePoint(this.Configuration.TableClient.BaseUri);
            tableServicePoint.ConnectionLimit = 1000;
        }

        private void PushTransactions(MultiValueDictionary<string, TransactionEntry.Entity> buckets,
                                        IEnumerable<TransactionEntry.Entity> indexedTransactions,
                                    BlockingCollection<TransactionEntry.Entity[]> transactions)
        {
            var array = indexedTransactions.ToArray();
            transactions.Add(array);
            buckets.Remove(array[0].PartitionKey);
        }

        private TimeSpan _Timeout = TimeSpan.FromMinutes(5.0);

        /// <summary>
        /// TaskScheduler to parallelize individual object Index methods
        /// </summary>
        public TaskScheduler TaskScheduler
        {
            get;
            set;
        }

        public void Index(params Block[] blocks)
        {
            var task = new IndexBlocksTask(this.Configuration);
            task.Index(blocks, this.TaskScheduler);
        }

        public Task IndexAsync(params Block[] blocks)
        {
            var task = new IndexBlocksTask(this.Configuration);
            return task.IndexAsync(blocks, this.TaskScheduler);
        }

        public void Index(params TransactionEntry.Entity[] entities)
        {
            this.Index(entities.Select(e => e.CreateTableEntity()).ToArray(), this.Configuration.GetTransactionTable());
        }

        public Task IndexAsync(params TransactionEntry.Entity[] entities)
        {
            return this.IndexAsync(entities.Select(e => e.CreateTableEntity()).ToArray(), this.Configuration.GetTransactionTable());
        }

        public void Index(IEnumerable<OrderedBalanceChange> balances)
        {
            this.Index(balances.Select(b => b.ToEntity()), this.Configuration.GetBalanceTable());
        }

        public Task IndexAsync(IEnumerable<OrderedBalanceChange> balances)
        {
            return this.IndexAsync(balances.Select(b => b.ToEntity()), this.Configuration.GetBalanceTable());
        }

        private void Index(IEnumerable<ITableEntity> entities, CloudTable table)
        {
            var task = new IndexTableEntitiesTask(this.Configuration, table);
            task.Index(entities, this.TaskScheduler);
        }

        private Task IndexAsync(IEnumerable<ITableEntity> entities, CloudTable table)
        {
            var task = new IndexTableEntitiesTask(this.Configuration, table);
            return task.IndexAsync(entities, this.TaskScheduler);
        }        

        public Checkpoint GetCheckpoint(IndexerCheckpoints checkpoint)
        {
            return this.GetCheckpointRepository().GetCheckpoint(checkpoint.ToString().ToLowerInvariant());
        }
        public Task<Checkpoint> GetCheckpointAsync(IndexerCheckpoints checkpoint)
        {
            return this.GetCheckpointRepository().GetCheckpointAsync(checkpoint.ToString().ToLowerInvariant());
        }

        public CheckpointRepository GetCheckpointRepository()
        {
            return new CheckpointRepository(this._Configuration.GetBlocksContainer(), 
                this._Configuration.Network, string.IsNullOrWhiteSpace(this._Configuration.CheckpointSetName) 
                ? "default" : this._Configuration.CheckpointSetName, this.loggerFactory);
        }

        internal ChainBase GetMainChain()
        {
            return this.Configuration.CreateIndexerClient().GetMainChain();
        }

        public void IndexOrderedBalance(int height, Block block)
        {
            var table = this.Configuration.GetBalanceTable();
            var blockId = block == null ? null : block.GetHash();
            var header = block == null ? null : block.Header;

            var entities =
                    block
                        .Transactions
                        .SelectMany(t => OrderedBalanceChange.ExtractScriptBalances(t.GetHash(), t, blockId, header, height))
                        .Select(_ => _.ToEntity())
                        .AsEnumerable();

            this.Index(entities, table);
        }

        public void IndexTransactions(int height, Block block)
        {
            var table = this.Configuration.GetTransactionTable();
            var blockId = block == null ? null : block.GetHash();
            var entities =
                        block
                        .Transactions
                        .Select(t => new TransactionEntry.Entity(t.GetHash(), t, blockId))
                        .Select(c => c.CreateTableEntity())
                        .AsEnumerable();
            this.Index(entities, table);
        }

        public void IndexWalletOrderedBalance(int height, Block block, WalletRuleEntryCollection walletRules)
        {
            try
            {
                this.IndexWalletOrderedBalanceAsync(height, block, walletRules).Wait();
            }
            catch(AggregateException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            }
        }
        public Task IndexWalletOrderedBalanceAsync(int height, Block block, WalletRuleEntryCollection walletRules)
        {
            var table = this.Configuration.GetBalanceTable();
            var blockId = block == null ? null : block.GetHash();

            var entities =
                    block
                    .Transactions
                    .SelectMany(t => OrderedBalanceChange.ExtractWalletBalances(null, t, blockId, block.Header, height, walletRules))
                    .Select(t => t.ToEntity())
                    .AsEnumerable();

            return this.IndexAsync(entities, table);
        }

        public void IndexOrderedBalance(Transaction tx)
        {
            var table = this.Configuration.GetBalanceTable();
            var entities = OrderedBalanceChange.ExtractScriptBalances(tx).Select(t => t.ToEntity()).AsEnumerable();
            this.Index(entities, table);
        }
        public Task IndexOrderedBalanceAsync(Transaction tx)
        {
            var table = this.Configuration.GetBalanceTable();
            var entities = OrderedBalanceChange.ExtractScriptBalances(tx).Select(t => t.ToEntity()).AsEnumerable();
            return this.IndexAsync(entities, table);
        }

        internal const int BlockHeaderPerRow = 6;
        internal void Index(ChainBase chain, int startHeight, CancellationToken cancellationToken = default(CancellationToken))
        {
            List<ChainPartEntry> entries = new List<ChainPartEntry>(((chain.Height - startHeight) / BlockHeaderPerRow) + 5);
            startHeight = startHeight - (startHeight % BlockHeaderPerRow);
            ChainPartEntry chainPart = null;
            for(int i = startHeight; i <= chain.Tip.Height; i++)
            {
                if(chainPart == null)
                    chainPart = new ChainPartEntry()
                    {
                        ChainOffset = i
                    };

                var block = chain.GetBlock(i);
                chainPart.BlockHeaders.Add(block.Header);
                if(chainPart.BlockHeaders.Count == BlockHeaderPerRow)
                {
                    entries.Add(chainPart);
                    chainPart = null;
                }
            }
            if(chainPart != null)
                entries.Add(chainPart);
            this.Index(entries, cancellationToken);
        }

        private void Index(List<ChainPartEntry> chainParts, CancellationToken cancellationToken = default(CancellationToken))
        {
            CloudTable table = this.Configuration.GetChainTable();
            TableBatchOperation batch = new TableBatchOperation();
            var last = chainParts[chainParts.Count - 1];

            foreach(var entry in chainParts)
            {
                batch.Add(TableOperation.InsertOrReplace(entry.ToEntity()));
                if(batch.Count == 100)
                {
                    table.ExecuteBatchAsync(batch).GetAwaiter().GetResult();
                    batch = new TableBatchOperation();
                }
                IndexerTrace.RemainingBlockChain(entry.ChainOffset, last.ChainOffset + last.BlockHeaders.Count - 1);
            }

            if(batch.Count > 0)
            {
                table.ExecuteBatchAsync(batch, null, null, cancellationToken).GetAwaiter().GetResult();
            }
        }

        public TimeSpan CheckpointInterval
        {
            get;
            set;
        }

        public int FromHeight
        {
            get;
            set;
        }

        public bool IgnoreCheckpoints
        {
            get;
            set;
        }

        public void IndexChain(ChainBase chain, CancellationToken cancellationToken = default(CancellationToken))
        {
            if(chain == null)
                throw new ArgumentNullException("chain");
            this.SetThrottling();

            using(IndexerTrace.NewCorrelation("Index main chain to azure started"))
            {
                this.Configuration.GetChainTable().CreateIfNotExistsAsync().GetAwaiter().GetResult();
                IndexerTrace.InputChainTip(chain.Tip);
                var client = this.Configuration.CreateIndexerClient();
                var changes = client.GetChainChangesUntilFork(chain.Tip, true, cancellationToken).ToList();

                var height = 0;
                if(changes.Count != 0)
                {
                    IndexerTrace.IndexedChainTip(changes[0].BlockId, changes[0].Height);
                    if(changes[0].Height > chain.Tip.Height)
                    {
                        IndexerTrace.InputChainIsLate();
                        return;
                    }
                    height = changes[changes.Count - 1].Height + 1;
                    if(height > chain.Height)
                    {
                        IndexerTrace.IndexedChainIsUpToDate(chain.Tip);
                        return;
                    }
                }
                else
                {
                    IndexerTrace.NoForkFoundWithStored();
                }

                IndexerTrace.IndexingChain(chain.GetBlock(height), chain.Tip);
                this.Index(chain, height, cancellationToken);
            }
        }

        public int ToHeight
        {
            get;
            set;
        }
    }
}
