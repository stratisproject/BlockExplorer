using Stratis.Bitcoin.Features.AzureIndexer.Repositories;

namespace Stratis.Bitcoin.Features.AzureIndexer
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Runtime.ExceptionServices;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.WindowsAzure.Storage.Table;
    using NBitcoin;
    using Stratis.Bitcoin.AsyncWork;
    using Stratis.Bitcoin.Features.AzureIndexer.Entities;
    using Stratis.Bitcoin.Features.AzureIndexer.Helpers;
    using Stratis.Bitcoin.Features.AzureIndexer.IndexTasks;
    using Stratis.Bitcoin.Features.AzureIndexer.Internal;

    public enum IndexerCheckpoints
    {
        Wallets,
        Transactions,
        Blocks,
        Balances,
        TokenTransactions
    }

    public class AzureIndexer
    {
        public static AzureIndexer CreateIndexer(IConfiguration config, ILoggerFactory loggerFactory, IAsyncProvider asyncProvider)
        {
            IndexerConfiguration indexerConfig = new IndexerConfiguration(config, loggerFactory, asyncProvider);
            return indexerConfig.CreateIndexer();
        }

        private readonly IndexerConfiguration configuration;

        private readonly ILoggerFactory _loggerFactory;

        private readonly ILogger _logger;

        public IndexerConfiguration Configuration => this.configuration;

        public AzureIndexer(IndexerConfiguration configuration, ILoggerFactory loggerFactory)
        {
            this._loggerFactory = loggerFactory;
            this.TaskScheduler = TaskScheduler.Default;
            this.CheckpointInterval = TimeSpan.FromMinutes(15.0);
            this.configuration = configuration ?? throw new ArgumentNullException("configuration");
            this.FromHeight = 0;
            this.ToHeight = int.MaxValue;
            this._logger = loggerFactory.CreateLogger(this.GetType().FullName);
            IndexerTrace.Configure(this._loggerFactory);
        }

        internal Checkpoint GetCheckpointInternal(IndexerCheckpoints checkpoint)
        {
            this._logger.LogTrace("({0}:{1})", nameof(checkpoint), checkpoint);

            Checkpoint chk = this.GetCheckpoint(checkpoint);
            if (this.IgnoreCheckpoints)
            {
                this._logger.LogTrace("Checkpoints ignored");
                chk = new Checkpoint(chk.CheckpointName, this.Configuration.Network, null, null, this._loggerFactory);
            }

            this._logger.LogTrace("(-)");
            return chk;
        }

        private void SetThrottling()
        {
            Helper.SetThrottling();
            ServicePoint tableServicePoint = ServicePointManager.FindServicePoint(this.Configuration.TableClient.BaseUri);
            tableServicePoint.ConnectionLimit = 1000;
        }

        private void PushTransactions(MultiValueDictionary<string, TransactionEntry.Entity> buckets, IEnumerable<TransactionEntry.Entity> indexedTransactions, BlockingCollection<TransactionEntry.Entity[]> transactions)
        {
            TransactionEntry.Entity[] array = indexedTransactions.ToArray();
            transactions.Add(array);
            buckets.Remove(array[0].PartitionKey);
        }

        private TimeSpan _Timeout = TimeSpan.FromMinutes(5.0);

        /// <summary>
        /// Gets or sets taskScheduler to parallelize individual object Index methods
        /// </summary>
        public TaskScheduler TaskScheduler { get; set; }

        public void Index(params Block[] blocks)
        {
            IndexBlocksTask task = new IndexBlocksTask(this.Configuration, this._loggerFactory);
            task.Index(blocks, this.TaskScheduler);
        }

        /// <summary>
        /// Used by API
        /// </summary>
        /// <param name="blocks">Block array</param>
        /// <returns>Block Task</returns>
        public Task IndexAsync(params Block[] blocks)
        {
            IndexBlocksTask task = new IndexBlocksTask(this.Configuration, this._loggerFactory);
            Task indexTask = task.IndexAsync(blocks, this.TaskScheduler);

            return indexTask;
        }

        public void Index(params TransactionEntry.Entity[] entities)
        {
            this.Index(entities.Select(e => e.CreateTableEntity(this.Configuration.Network)).ToArray(), this.Configuration.GetTransactionTable());
        }

        /// <summary>
        /// Used by API
        /// </summary>
        /// <param name="entities">Transaction Entities</param>
        /// <returns>Task</returns>
        public Task IndexAsync(params TransactionEntry.Entity[] entities)
        {
            return this.IndexAsync(entities.Select(e => e.CreateTableEntity(this.Configuration.Network)).ToArray(), this.Configuration.GetTransactionTable());
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
            IndexTableEntitiesTask task = new IndexTableEntitiesTask(this.Configuration, table, this._loggerFactory);
            task.Index(entities, this.TaskScheduler);
        }

        private Task IndexAsync(IEnumerable<ITableEntity> entities, CloudTable table)
        {
            IndexTableEntitiesTask task = new IndexTableEntitiesTask(this.Configuration, table, this._loggerFactory);
            Task indexTask = task.IndexAsync(entities, this.TaskScheduler);

            return indexTask;
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
            CheckpointRepository repository = new CheckpointRepository(
                this.configuration.GetBlocksContainer(),
                this.configuration.Network,
                string.IsNullOrWhiteSpace(this.configuration.CheckpointSetName) ? "default" : this.configuration.CheckpointSetName,
                this._loggerFactory);

            return repository;
        }

        internal ChainIndexer GetMainChain()
        {
            return this.Configuration.CreateIndexerClient().GetMainChain();
        }

        // Used just in Test project. Not sure why.
        public void IndexOrderedBalance(int height, Block block)
        {
            CloudTable table = this.Configuration.GetBalanceTable();
            uint256 blockId = block?.GetHash();
            BlockHeader header = block?.Header;

            IEnumerable<DynamicTableEntity> entities =
                    block
                        .Transactions
                        .SelectMany(t => OrderedBalanceChange.ExtractScriptBalances(t.GetHash(), t, blockId, header, height, this.configuration.Network))
                        .Select(_ => _.ToEntity())
                        .AsEnumerable();

            this.Index(entities, table);
        }

        // Used just in Test project. Not sure why.
        public void IndexWalletOrderedBalance(int height, Block block, WalletRuleEntryCollection walletRules)
        {
            try
            {
                this.IndexWalletOrderedBalanceAsync(height, block, walletRules).Wait();
            }
            catch (AggregateException ex)
            {
                this._logger.LogTrace("Exception: {0}", ex.ToString());
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            }
        }

        // Used just in Test project. Not sure why.
        public Task IndexWalletOrderedBalanceAsync(int height, Block block, WalletRuleEntryCollection walletRules)
        {
            CloudTable table = this.Configuration.GetBalanceTable();
            uint256 blockId = block?.GetHash();

            IEnumerable<DynamicTableEntity> entities =
                    block
                    .Transactions
                    .SelectMany(t => OrderedBalanceChange.ExtractWalletBalances(null, t, blockId, block.Header, height, walletRules, this.configuration.Network))
                    .Select(t => t.ToEntity())
                    .AsEnumerable();

            Task indexingTask = this.IndexAsync(entities, table);

            return indexingTask;
        }

        // Used just in Test project. Not sure why.
        public void IndexOrderedBalance(Transaction tx)
        {
            CloudTable table = this.Configuration.GetBalanceTable();
            IEnumerable<DynamicTableEntity> entities = OrderedBalanceChange.ExtractScriptBalances(tx, this.configuration.Network).Select(t => t.ToEntity()).AsEnumerable();
            this.Index(entities, table);
        }

        // Used by API.
        public Task IndexOrderedBalanceAsync(Transaction tx)
        {
            CloudTable table = this.Configuration.GetBalanceTable();
            IEnumerable<DynamicTableEntity> entities = OrderedBalanceChange.ExtractScriptBalances(tx, this.configuration.Network).Select(t => t.ToEntity()).AsEnumerable();
            return this.IndexAsync(entities, table);
        }

        internal const int BlockHeaderPerRow = 6;

        internal void Index(ChainIndexer chainIndexer, int startHeight, CancellationToken cancellationToken = default(CancellationToken))
        {
            var entries = new List<ChainPartEntry>(((chainIndexer.Height - startHeight) / BlockHeaderPerRow) + 5);
            startHeight = startHeight - (startHeight % BlockHeaderPerRow);
            ChainPartEntry chainPart = null;
            for (var i = startHeight; i <= chainIndexer.Tip.Height; i++)
            {
                if (chainPart == null)
                {
                    chainPart = new ChainPartEntry()
                    {
                        ChainOffset = i
                    };
                }

                ChainedHeader block = chainIndexer.GetHeader(i);
                chainPart.BlockHeaders.Add(block.Header);
                if (chainPart.BlockHeaders.Count == BlockHeaderPerRow)
                {
                    entries.Add(chainPart);
                    chainPart = null;
                }
            }

            if (chainPart != null)
            {
                entries.Add(chainPart);
            }

            this.Index(entries, cancellationToken);
        }

        private void Index(List<ChainPartEntry> chainParts, CancellationToken cancellationToken = default(CancellationToken))
        {
            CloudTable table = this.Configuration.GetChainTable();
            TableBatchOperation batch = new TableBatchOperation();
            ChainPartEntry last = chainParts[chainParts.Count - 1];

            foreach (ChainPartEntry entry in chainParts)
            {
                batch.Add(TableOperation.InsertOrReplace(entry.ToEntity()));
                if (batch.Count == 100)
                {
                    table.ExecuteBatchAsync(batch).GetAwaiter().GetResult();
                    batch = new TableBatchOperation();
                }

                IndexerTrace.RemainingBlockChain(entry.ChainOffset, last.ChainOffset + last.BlockHeaders.Count - 1);
            }

            if (batch.Count > 0)
            {
                table.ExecuteBatchAsync(batch, null, null, cancellationToken).GetAwaiter().GetResult();
            }
        }

        public TimeSpan CheckpointInterval { get; set; }

        public int FromHeight { get; set; }

        public bool IgnoreCheckpoints { get; set; }

        public int ToHeight { get; set; }

        public void IndexChain(ChainIndexer chainIndexer, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (chainIndexer == null)
            {
                throw new ArgumentNullException("chainIndexer");
            }

            this.SetThrottling();

            using (IndexerTrace.NewCorrelation("Index main chain to azure started"))
            {
                this.Configuration.GetChainTable().CreateIfNotExistsAsync().GetAwaiter().GetResult();
                IndexerTrace.InputChainTip(chainIndexer.Tip);
                IndexerClient client = this.Configuration.CreateIndexerClient();
                List<ChainBlockHeader> changes = client.GetChainChangesUntilFork(chainIndexer.Tip, true, cancellationToken).ToList();

                var height = 0;
                if (changes.Count != 0)
                {
                    IndexerTrace.IndexedChainTip(changes[0].BlockId, changes[0].Height);
                    if (changes[0].Height > chainIndexer.Tip.Height)
                    {
                        IndexerTrace.InputChainIsLate();

                        return;
                    }

                    height = changes[changes.Count - 1].Height + 1;
                    if (height > chainIndexer.Height)
                    {
                        IndexerTrace.IndexedChainIsUpToDate(chainIndexer.Tip);

                        return;
                    }
                }
                else
                {
                    IndexerTrace.NoForkFoundWithStored();
                }

                IndexerTrace.IndexingChain(chainIndexer.GetHeader(height), chainIndexer.Tip);
                this.Index(chainIndexer, height, cancellationToken);
            }
        }
    }
}
