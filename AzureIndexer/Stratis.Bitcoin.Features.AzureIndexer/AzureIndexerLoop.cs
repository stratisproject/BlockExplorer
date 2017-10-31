using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Auth;
using NBitcoin;
using Stratis.Bitcoin.Features.AzureIndexer.IndexTasks;
using Stratis.Bitcoin.Utilities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Stratis.Bitcoin.Features.AzureIndexer
{
    /// <summary>
    /// The AzureIndexerStoreLoop loads blocks from the block repository and indexes them in Azure.
    /// </summary>
    public class AzureIndexerLoop
    {
        /// <summary>Factory for creating background async loop tasks.</summary>
        private readonly IAsyncLoopFactory asyncLoopFactory;

        /// <summary>The async loop we need to wait upon before we can shut down this feature.</summary>
        private IAsyncLoop asyncLoop;

        /// <summary>Another async loop we need to wait upon before we can shut down this feature.</summary>
        private IAsyncLoop asyncLoopChain;

        /// <summary>The full node that owns the block repository that we want to index.</summary>
        public FullNode FullNode { get; }

        /// <summary>Best chain of block headers.</summary>
        internal readonly ConcurrentChain Chain;

        /// <summary>Instance logger.</summary>
        private readonly ILogger logger;

        /// <summary>The node life time let us know when the node is shutting down.</summary>
        private readonly INodeLifetime nodeLifetime;

        /// <summary>The number of blocks to index at a time.</summary>
        private const int IndexBatchSize = 100;

        /// <summary>The name of this node feature for reporting stats.</summary>
        public virtual string StoreName { get { return "AzureIndexer"; } }

        /// <summary>The Azure Indezer settings.</summary>
        private readonly AzureIndexerSettings indexerSettings;

        /// <summary>The highest block that has been indexed.</summary>
        internal ChainedBlock StoreTip { get; private set; }

        public AzureIndexer AzureIndexer { get; private set; }
        public IndexerConfiguration IndexerConfig { get; private set; }
        
        /// <summary>
        /// Constructs the AzureIndexerLoop.
        /// </summary>
        /// <param name="fullNode">The full node that will be indexed.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public AzureIndexerLoop(FullNode fullNode, ILoggerFactory loggerFactory)
        {
            this.asyncLoopFactory = fullNode.AsyncLoopFactory;
            this.FullNode = fullNode;
            this.Chain = fullNode.Chain;
            this.nodeLifetime = fullNode.NodeLifetime;
            this.indexerSettings = fullNode.NodeService<AzureIndexerSettings>();
            this.logger = loggerFactory.CreateLogger(GetType().FullName);
        }

        /// <summary>
        /// Initializes the Azure Indexer.
        /// </summary>
        public void Initialize()
        {
            this.logger.LogTrace("()");

            IndexerConfiguration indexerConfig = new IndexerConfiguration();

            indexerConfig.StorageCredentials = new StorageCredentials(
                this.indexerSettings.AzureAccountName, this.indexerSettings.AzureKey);
            indexerConfig.StorageNamespace = "";
            indexerConfig.Network = this.FullNode.Network;
            indexerConfig.CheckpointSetName = this.indexerSettings.CheckpointsetName;
            indexerConfig.AzureStorageEmulatorUsed = this.indexerSettings.AzureEmulatorUsed;

            this.IndexerConfig = indexerConfig;

            var indexer = indexerConfig.CreateIndexer();
            indexer.Configuration.EnsureSetup();
            indexer.TaskScheduler = new CustomThreadPoolTaskScheduler(30, 100);
            indexer.CheckpointInterval = this.indexerSettings.CheckpointInterval;
            indexer.IgnoreCheckpoints = this.indexerSettings.IgnoreCheckpoints;
            indexer.FromHeight = this.indexerSettings.From;
            indexer.ToHeight = this.indexerSettings.To;

            this.AzureIndexer = indexer;

            if (this.indexerSettings.IgnoreCheckpoints)
                this.SetStoreTip(this.Chain.GetBlock(this.indexerSettings.From));
            else
            {
                int minHeight = int.MaxValue;
                minHeight = Math.Min(minHeight, this.GetCheckPointBlock(IndexerCheckpoints.Blocks).Height);
                minHeight = Math.Min(minHeight, this.GetCheckPointBlock(IndexerCheckpoints.Balances).Height);
                minHeight = Math.Min(minHeight, this.GetCheckPointBlock(IndexerCheckpoints.Transactions).Height);
                minHeight = Math.Min(minHeight, this.GetCheckPointBlock(IndexerCheckpoints.Wallets).Height);

                this.SetStoreTip(this.Chain.GetBlock(minHeight));
            }

            this.StartLoop();
            
            this.logger.LogTrace("(-)");
        }

        /// <summary>
        /// Determines the block that a checkpoint is at.
        /// </summary>
        /// <param name="indexerCheckpoints">The type of checkpoint (wallets, blocks, transactions or balances).</param>
        /// <returns>The block that a checkpoint is at.</returns>
        private ChainedBlock GetCheckPointBlock(IndexerCheckpoints indexerCheckpoints)
        {
            Checkpoint checkpoint = this.AzureIndexer.GetCheckpointInternal(indexerCheckpoints);
            return this.Chain.FindFork(checkpoint.BlockLocator);
        }

        /// <summary>
        /// Starts the indexing loop.
        /// </summary>
        private void StartLoop()
        {
            this.asyncLoop = this.asyncLoopFactory.Run($"{this.StoreName}.IndexAsync", async token =>
            {
                await IndexAsync(this.nodeLifetime.ApplicationStopping);
            },
            this.nodeLifetime.ApplicationStopping,
            repeatEvery: TimeSpans.RunOnce,
            startAfter: TimeSpans.FiveSeconds);

            this.asyncLoopChain = this.asyncLoopFactory.Run($"{this.StoreName}.IndexChainAsync", async token =>
            {
                await IndexChainAsync(this.nodeLifetime.ApplicationStopping);
            },
            this.nodeLifetime.ApplicationStopping,
            repeatEvery: TimeSpans.RunOnce,
            startAfter: TimeSpans.Minute);

        }

        /// <summary>
        /// Shuts down the indexing loop.
        /// </summary>
        public void Shutdown()
        {
            this.asyncLoop.Dispose();
            this.asyncLoopChain.Dispose();
        }

        /// <summary>
        /// Gets a block fetcher that respects the given type of checkpoint.
        /// The block fetcher will return "IndexBatchSize" blocks starting at this.StoreTip + 1.
        /// If "this.AzureIndexer.IgnoreCheckpoints" is set then the checkpoints 
        /// will be ignored by "GetCheckpointInternal".
        /// </summary>
        /// <param name="indexerCheckpoints">The type of checkpoint (wallets, blocks, transactions or balances).</param>
        /// <param name="cancellationToken">The token used for cancellation.</param>
        /// <returns>A block fetcher that respects the given type of checkpoint.</returns>
        private BlockFetcher GetBlockFetcher(IndexerCheckpoints indexerCheckpoints, CancellationToken cancellationToken)
        {
            Checkpoint checkpoint = this.AzureIndexer.GetCheckpointInternal(indexerCheckpoints);
            FullNodeBlocksRepository repo = new FullNodeBlocksRepository(this.FullNode);
            return new BlockFetcher(checkpoint, repo, this.Chain)
            {
                NeedSaveInterval = this.indexerSettings.CheckpointInterval,
                FromHeight = this.StoreTip.Height + 1,
                ToHeight = Math.Min(this.StoreTip.Height + IndexBatchSize, this.indexerSettings.To),
                CancellationToken = cancellationToken
            };
        }

        /// <summary>
        /// Performs "Chain" indexing into Azure storage.
        /// </summary>
        /// <param name="cancellationToken">The token used for cancellation.</param>
        /// <returns>A task for asynchronous completion.</returns>
        private async Task IndexChainAsync(CancellationToken cancellationToken)
        {
            this.logger.LogTrace("()");

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {                  
                    if (!cancellationToken.IsCancellationRequested)
                    {
                        this.AzureIndexer.IndexChain(this.Chain, cancellationToken);
                    }
                }
                catch (Exception)
                {
                    // Try again 1 minute later
                    await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken).ContinueWith(t => { }).ConfigureAwait(false);
                }
            }

            this.logger.LogTrace("(-)");
        }

        /// <summary>
        /// Performs indexing into Azure storage.
        /// </summary>
        /// <param name="cancellationToken">The token used for cancellation.</param>
        /// <returns>A task for asynchronous completion.</returns>
        private async Task IndexAsync(CancellationToken cancellationToken)
        {
            this.logger.LogTrace("()");

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    // All indexes will progress more or less in step
                    // Use 'minHeight' to track the current indexed height
                    int minHeight = int.MaxValue;

                    // Index a batch of blocks
                    if (!cancellationToken.IsCancellationRequested)
                    {
                        var task = new IndexBlocksTask(this.IndexerConfig);
                        task.SaveProgression = !this.indexerSettings.IgnoreCheckpoints;
                        var fetcher = this.GetBlockFetcher(IndexerCheckpoints.Blocks, cancellationToken);
                        task.Index(fetcher, this.AzureIndexer.TaskScheduler);
                        minHeight = Math.Min(minHeight, fetcher._LastProcessed.Height);
                    }

                    // Index a batch of transactions
                    if (!cancellationToken.IsCancellationRequested)
                    {
                        var task = new IndexTransactionsTask(this.IndexerConfig);
                        task.SaveProgression = !this.indexerSettings.IgnoreCheckpoints;
                        var fetcher = this.GetBlockFetcher(IndexerCheckpoints.Transactions, cancellationToken);
                        task.Index(fetcher, this.AzureIndexer.TaskScheduler);
                        minHeight = Math.Min(minHeight, fetcher._LastProcessed.Height);
                    }

                    // Index a batch of balances
                    if (!cancellationToken.IsCancellationRequested)
                    {
                        var task = new IndexBalanceTask(this.IndexerConfig, null);
                        task.SaveProgression = !this.indexerSettings.IgnoreCheckpoints;
                        var fetcher = this.GetBlockFetcher(IndexerCheckpoints.Balances, cancellationToken);
                        task.Index(fetcher, this.AzureIndexer.TaskScheduler);
                        minHeight = Math.Min(minHeight, fetcher._LastProcessed.Height);
                    }

                    // Index a batch of wallets
                    if (!cancellationToken.IsCancellationRequested)
                    {
                        var task = new IndexBalanceTask(this.IndexerConfig, this.IndexerConfig.CreateIndexerClient().GetAllWalletRules());
                        task.SaveProgression = !this.indexerSettings.IgnoreCheckpoints;
                        var fetcher = this.GetBlockFetcher(IndexerCheckpoints.Wallets, cancellationToken);
                        task.Index(fetcher, this.AzureIndexer.TaskScheduler);
                        minHeight = Math.Min(minHeight, fetcher._LastProcessed.Height);
                    }

                    // Update the StoreTip value from the minHeight
                    this.SetStoreTip(this.Chain.GetBlock(Math.Min(minHeight, this.indexerSettings.To)));
                }
                catch (Exception ex)
                {
                    // If something goes wrong then try again 1 minute later
                    IndexerTrace.ErrorWhileImportingBlockToAzure(this.StoreTip.HashBlock, ex);
                    await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken).ContinueWith(t => { }).ConfigureAwait(false);
                }
            }

            this.logger.LogTrace("(-)");
        }

        /// <summary>
        /// Sets the StoreTip.
        /// </summary>
        /// <param name="chainedBlock">The block to set the store tip to.</param>
        internal void SetStoreTip(ChainedBlock chainedBlock)
        {
            this.logger.LogTrace("({0}:'{1}')", nameof(chainedBlock), chainedBlock?.HashBlock);
            Guard.NotNull(chainedBlock, nameof(chainedBlock));

            this.StoreTip = chainedBlock;

            this.logger.LogTrace("(-)");
        }
    }
}