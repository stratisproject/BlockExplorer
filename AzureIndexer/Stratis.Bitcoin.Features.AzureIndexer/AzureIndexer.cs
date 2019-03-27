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
    using Stratis.Bitcoin.Features.AzureIndexer.IndexTasks;
    using Stratis.Bitcoin.Features.AzureIndexer.Internal;

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
            IndexerConfiguration indexerConfig = new IndexerConfiguration(config, loggerFactory);
            return indexerConfig.CreateIndexer();
        }

        private readonly IndexerConfiguration _configuration;

        private readonly ILoggerFactory _loggerFactory;

        private readonly ILogger _logger;

        public IndexerConfiguration Configuration
        {
            get
            {
                return this._configuration;
            }
        }

        /// <inheritdoc />
        public AzureIndexer(IndexerConfiguration configuration, ILoggerFactory loggerFactory)
        {
            this._loggerFactory = loggerFactory;
            this.TaskScheduler = TaskScheduler.Default;
            this.CheckpointInterval = TimeSpan.FromMinutes(15.0);
            this._configuration = configuration ?? throw new ArgumentNullException("configuration");
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
            this._logger.LogTrace("()");

            Helper.SetThrottling();
            ServicePoint tableServicePoint = ServicePointManager.FindServicePoint(this.Configuration.TableClient.BaseUri);
            tableServicePoint.ConnectionLimit = 1000;

            this._logger.LogTrace("(-)");
        }

        private void PushTransactions(MultiValueDictionary<string, TransactionEntry.Entity> buckets,
                                        IEnumerable<TransactionEntry.Entity> indexedTransactions,
                                    BlockingCollection<TransactionEntry.Entity[]> transactions)
        {
            this._logger.LogTrace("()");

            TransactionEntry.Entity[] array = indexedTransactions.ToArray();
            transactions.Add(array);
            buckets.Remove(array[0].PartitionKey);

            this._logger.LogTrace("(-)");
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
            this._logger.LogTrace("()");

            IndexBlocksTask task = new IndexBlocksTask(this.Configuration, this._loggerFactory);
            task.Index(blocks, this.TaskScheduler);

            this._logger.LogTrace("(-)");
        }

        public Task IndexAsync(params Block[] blocks)
        {
            this._logger.LogTrace("()");

            IndexBlocksTask task = new IndexBlocksTask(this.Configuration, this._loggerFactory);
            Task indexTask = task.IndexAsync(blocks, this.TaskScheduler);

            this._logger.LogTrace("()");
            return indexTask;
        }

        public void Index(params TransactionEntry.Entity[] entities)
        {
            this._logger.LogTrace("()");

            this.Index(entities.Select(e => e.CreateTableEntity(this.Configuration.Network)).ToArray(), this.Configuration.GetTransactionTable());

            this._logger.LogTrace("(-)");
        }

        public Task IndexAsync(params TransactionEntry.Entity[] entities)
        {
            return this.IndexAsync(entities.Select(e => e.CreateTableEntity(this.Configuration.Network)).ToArray(), this.Configuration.GetTransactionTable());
        }

        public void Index(IEnumerable<OrderedBalanceChange> balances)
        {
            this._logger.LogTrace("()");

            this.Index(balances.Select(b => b.ToEntity()), this.Configuration.GetBalanceTable());

            this._logger.LogTrace("(-)");
        }

        public Task IndexAsync(IEnumerable<OrderedBalanceChange> balances)
        {
            return this.IndexAsync(balances.Select(b => b.ToEntity()), this.Configuration.GetBalanceTable());
        }

        private void Index(IEnumerable<ITableEntity> entities, CloudTable table)
        {
            this._logger.LogTrace("()");

            IndexTableEntitiesTask task = new IndexTableEntitiesTask(this.Configuration, table, this._loggerFactory);
            task.Index(entities, this.TaskScheduler);

            this._logger.LogTrace("(-)");
        }

        private Task IndexAsync(IEnumerable<ITableEntity> entities, CloudTable table)
        {
            this._logger.LogTrace("()");

            IndexTableEntitiesTask task = new IndexTableEntitiesTask(this.Configuration, table, this._loggerFactory);
            Task indexTask = task.IndexAsync(entities, this.TaskScheduler);

            this._logger.LogTrace("(-)");
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
            this._logger.LogTrace("()");

            CheckpointRepository repository = new CheckpointRepository(
                this._configuration.GetBlocksContainer(),
                this._configuration.Network,
                string.IsNullOrWhiteSpace(this._configuration.CheckpointSetName) ? "default" : this._configuration.CheckpointSetName,
                this._loggerFactory);

            this._logger.LogTrace("(-)");
            return repository;
        }

        internal ChainBase GetMainChain()
        {
            return this.Configuration.CreateIndexerClient().GetMainChain();
        }

        public void IndexOrderedBalance(int height, Block block)
        {
            this._logger.LogTrace("()");

            CloudTable table = this.Configuration.GetBalanceTable();
            uint256 blockId = block?.GetHash();
            BlockHeader header = block?.Header;

            IEnumerable<DynamicTableEntity> entities =
                    block
                        .Transactions
                        .SelectMany(t => OrderedBalanceChange.ExtractScriptBalances(t.GetHash(), t, blockId, header, height, this._configuration.Network))
                        .Select(_ => _.ToEntity())
                        .AsEnumerable();

            this._logger.LogTrace("Indexing ordered balance");

            this.Index(entities, table);

            this._logger.LogTrace("(-)");
        }

        // TODO: Is it in use?
        //public void IndexTransactions(int height, Block block)
        //{
        //    this._logger.LogTrace("()");

        //    CloudTable table = this.Configuration.GetTransactionTable();
        //    uint256 blockId = block?.GetHash();
        //    IEnumerable<DynamicTableEntity> entities =
        //                block
        //                .Transactions
        //                .Select(t => new TransactionEntry.Entity(t.GetHash(), t, blockId))
        //                .Select(c => c.CreateTableEntity(this.Configuration.Network))
        //                .AsEnumerable();

        //    this._logger.LogTrace("Indexing transactions");

        //    this.Index(entities, table);

        //    this._logger.LogTrace("(-)");
        //}

        public void IndexWalletOrderedBalance(int height, Block block, WalletRuleEntryCollection walletRules)
        {
            this._logger.LogTrace("()");

            try
            {
                this.IndexWalletOrderedBalanceAsync(height, block, walletRules).Wait();
            }
            catch (AggregateException ex)
            {
                this._logger.LogTrace("Exception: {0}", ex.ToString());
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            }

            this._logger.LogTrace("(-)");
        }

        public Task IndexWalletOrderedBalanceAsync(int height, Block block, WalletRuleEntryCollection walletRules)
        {
            this._logger.LogTrace("()");

            CloudTable table = this.Configuration.GetBalanceTable();
            uint256 blockId = block?.GetHash();

            IEnumerable<DynamicTableEntity> entities =
                    block
                    .Transactions
                    .SelectMany(t => OrderedBalanceChange.ExtractWalletBalances(null, t, blockId, block.Header, height, walletRules, this._configuration.Network))
                    .Select(t => t.ToEntity())
                    .AsEnumerable();

            Task indexingTask = this.IndexAsync(entities, table);

            this._logger.LogTrace("(-)");
            return indexingTask;
        }

        public void IndexOrderedBalance(Transaction tx)
        {
            this._logger.LogTrace("()");

            CloudTable table = this.Configuration.GetBalanceTable();
            IEnumerable<DynamicTableEntity> entities = OrderedBalanceChange.ExtractScriptBalances(tx, this._configuration.Network).Select(t => t.ToEntity()).AsEnumerable();
            this.Index(entities, table);

            this._logger.LogTrace("(-)");
        }

        public Task IndexOrderedBalanceAsync(Transaction tx)
        {
            CloudTable table = this.Configuration.GetBalanceTable();
            IEnumerable<DynamicTableEntity> entities = OrderedBalanceChange.ExtractScriptBalances(tx, this._configuration.Network).Select(t => t.ToEntity()).AsEnumerable();
            return this.IndexAsync(entities, table);
        }

        internal const int BlockHeaderPerRow = 6;

        internal void Index(ChainBase chain, int startHeight, CancellationToken cancellationToken = default(CancellationToken))
        {
            this._logger.LogTrace("({0}:{1})", nameof(startHeight), startHeight);

            var entries = new List<ChainPartEntry>(((chain.Height - startHeight) / BlockHeaderPerRow) + 5);
            startHeight = startHeight - (startHeight % BlockHeaderPerRow);
            ChainPartEntry chainPart = null;
            for (var i = startHeight; i <= chain.Tip.Height; i++)
            {
                if (chainPart == null)
                {
                    chainPart = new ChainPartEntry()
                    {
                        ChainOffset = i
                    };
                }

                ChainedHeader block = chain.GetBlock(i);
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

            this._logger.LogTrace("(-)");
        }

        private void Index(List<ChainPartEntry> chainParts, CancellationToken cancellationToken = default(CancellationToken))
        {
            this._logger.LogTrace("()");

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
                this._logger.LogTrace("Batch count: {0}", batch.Count);

                table.ExecuteBatchAsync(batch, null, null, cancellationToken).GetAwaiter().GetResult();
            }

            this._logger.LogTrace("(-)");
        }

        public TimeSpan CheckpointInterval { get; set; }

        public int FromHeight { get; set; }

        public bool IgnoreCheckpoints { get; set; }

        public void IndexChain(ChainBase chain, CancellationToken cancellationToken = default(CancellationToken))
        {
            this._logger.LogTrace("()");

            if (chain == null)
            {
                throw new ArgumentNullException("chain");
            }

            this.SetThrottling();

            using (IndexerTrace.NewCorrelation("Index main chain to azure started"))
            {
                this.Configuration.GetChainTable().CreateIfNotExistsAsync().GetAwaiter().GetResult();
                IndexerTrace.InputChainTip(chain.Tip);
                IndexerClient client = this.Configuration.CreateIndexerClient();
                List<ChainBlockHeader> changes = client.GetChainChangesUntilFork(chain.Tip, true, cancellationToken).ToList();

                var height = 0;
                if (changes.Count != 0)
                {
                    this._logger.LogTrace("Changes count: {0}", changes.Count);

                    IndexerTrace.IndexedChainTip(changes[0].BlockId, changes[0].Height);
                    if (changes[0].Height > chain.Tip.Height)
                    {
                        IndexerTrace.InputChainIsLate();

                        this._logger.LogTrace("(-):LATE");
                        return;
                    }

                    height = changes[changes.Count - 1].Height + 1;
                    if (height > chain.Height)
                    {
                        IndexerTrace.IndexedChainIsUpToDate(chain.Tip);

                        this._logger.LogTrace("(-):UP_TO_DATE");
                        return;
                    }
                }
                else
                {
                    this._logger.LogTrace("No work found");
                    IndexerTrace.NoForkFoundWithStored();
                }

                IndexerTrace.IndexingChain(chain.GetBlock(height), chain.Tip);
                this.Index(chain, height, cancellationToken);
            }

            this._logger.LogTrace("(-)");
        }

        public int ToHeight { get; set; }
    }
}
