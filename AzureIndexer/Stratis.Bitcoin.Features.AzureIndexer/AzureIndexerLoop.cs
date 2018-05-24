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
        internal ChainedHeader StoreTip { get; private set; }

        /// <summary>The Azure Indexer.</summary>
        public AzureIndexer AzureIndexer { get; private set; }

        public BlockFetcher BlocksFetcher { get; private set; }
        public BlockFetcher TransactionsFetcher { get; private set; }
        public BlockFetcher BalancesFetcher { get; private set; }
        public BlockFetcher WalletsFetcher { get; private set; }

        /// <summary>The Indexer Configuration.</summary>
        public IndexerConfiguration IndexerConfig { get; private set; }


        private readonly ILoggerFactory loggerFactory;

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
            this.loggerFactory = loggerFactory;
            this.logger = loggerFactory.CreateLogger(GetType().FullName);
        }

        /// <summary>
        /// Derives an IndexerConfiguration object from the proviced AzureIndexerSettings object and network.
        /// </summary>
        /// <param name="indexerSettings">The AzureIndexerSettings object to use.</param>
        /// <param name="network">The network to use.</param>
        /// <returns>An IndexerConfiguration object derived from the AzureIndexerSettings object and network.</returns>
        public static IndexerConfiguration IndexerConfigFromSettings(AzureIndexerSettings indexerSettings, Network network, ILoggerFactory loggerFactory)
        {
            IndexerConfiguration indexerConfig = new IndexerConfiguration(loggerFactory)
            {
                StorageNamespace = indexerSettings.StorageNamespace,
                Network = network,
                CheckpointSetName = indexerSettings.CheckpointsetName,
                AzureStorageEmulatorUsed = indexerSettings.AzureEmulatorUsed,
                StorageCredentials = indexerSettings.AzureEmulatorUsed ? null : new StorageCredentials(indexerSettings.AzureAccountName, indexerSettings.AzureKey)
            };
            return indexerConfig;
        }

        /// <summary>
        /// Initializes the Azure Indexer.
        /// </summary>
        public void Initialize()
        {
            this.logger.LogTrace("()");

            this.IndexerConfig = IndexerConfigFromSettings(this.indexerSettings, this.FullNode.Network, this.loggerFactory);

            var indexer = this.IndexerConfig.CreateIndexer();
            indexer.Configuration.EnsureSetup();
            indexer.TaskScheduler = new CustomThreadPoolTaskScheduler(30, 100);
            indexer.CheckpointInterval = this.indexerSettings.CheckpointInterval;
            indexer.IgnoreCheckpoints = this.indexerSettings.IgnoreCheckpoints;
            indexer.FromHeight = this.indexerSettings.From;
            indexer.ToHeight = this.indexerSettings.To;

            this.AzureIndexer = indexer;

            if (this.indexerSettings.IgnoreCheckpoints)
                this.SetStoreTip(this.Chain.GetBlock(indexer.FromHeight));
            else
                this.UpdateStoreTip();

            this.StartLoop();
            
            this.logger.LogTrace("(-)");
        }

        /// <summary>
        /// Update the store tip to match the last common processed block across all checkpoint types.
        /// </summary>
        private void UpdateStoreTip()
        {
            int minHeight = LastProcessed(IndexerCheckpoints.Blocks).Height;
            minHeight = Math.Min(minHeight, LastProcessed(IndexerCheckpoints.Transactions).Height);
            minHeight = Math.Min(minHeight, LastProcessed(IndexerCheckpoints.Balances).Height);
            minHeight = Math.Min(minHeight, LastProcessed(IndexerCheckpoints.Wallets).Height);
            this.SetStoreTip(this.Chain.GetBlock(minHeight));
        }

        /// <summary>
        /// Determines the block that a checkpoint is at.
        /// </summary>
        /// <param name="indexerCheckpoints">The type of checkpoint (wallets, blocks, transactions or balances).</param>
        /// <returns>The block that a checkpoint is at.</returns>
        private ChainedHeader GetCheckPointBlock(IndexerCheckpoints indexerCheckpoints)
        {
            this.logger.LogTrace("()");

            Checkpoint checkpoint = this.AzureIndexer.GetCheckpointInternal(indexerCheckpoints);
            ChainedHeader fork = this.Chain.FindFork(checkpoint.BlockLocator);

            this.logger.LogTrace("(-):{0}", fork?.ToString());
            return fork;
        }

        /// <summary>
        /// Starts the indexing loop.
        /// </summary>
        private void StartLoop()
        {
            this.logger.LogTrace("()");

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

            this.logger.LogTrace("(-)");
        }

        /// <summary>
        /// Shuts down the indexing loop.
        /// </summary>
        public void Shutdown()
        {
            this.logger.LogTrace("()");

            this.asyncLoop.Dispose();

            this.logger.LogTrace("AsyncLoop disposed");

            this.asyncLoopChain.Dispose();

            this.logger.LogTrace("(-)");
        }

        /// <summary>
        /// Gets a block fetcher that respects the given type of checkpoint.
        /// The block fetcher will return "IndexBatchSize" blocks starting at this.StoreTip + 1.
        /// If "this.AzureIndexer.IgnoreCheckpoints" is set then the checkpoints 
        /// will be ignored by "GetCheckpointInternal".
        /// </summary>
        /// <param name="indexerCheckpoints">The type of checkpoint (wallets, blocks, transactions or balances).</param>
        /// <returns>A block fetcher that respects the given type of checkpoint.</returns>
        private BlockFetcher GetBlockFetcher(IndexerCheckpoints indexerCheckpoints)
        {
            this.logger.LogTrace("()");

            Checkpoint checkpoint = this.AzureIndexer.GetCheckpointInternal(indexerCheckpoints);
            FullNodeBlocksRepository repo = new FullNodeBlocksRepository(this.FullNode);

            var fetcher = new BlockFetcher(checkpoint, repo, this.Chain, this.Chain.FindFork(checkpoint.BlockLocator), this.loggerFactory)
            {
                NeedSaveInterval = this.indexerSettings.CheckpointInterval,
                FromHeight = this.StoreTip.Height + 1,
                ToHeight = Math.Min(this.StoreTip.Height + IndexBatchSize, this.indexerSettings.To),
                CancellationToken = this.nodeLifetime.ApplicationStopping
            };

            this.logger.LogTrace("(-)");
            return fetcher;
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
                    this.AzureIndexer.IndexChain(this.Chain, cancellationToken);
                    await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken).ContinueWith(t => { }).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    this.logger.LogTrace("Exception occurred: {0}", ex.ToString());

                    // If something goes wrong then try again 1 minute later
                    IndexerTrace.ErrorWhileImportingBlockToAzure(this.StoreTip.HashBlock, ex);
                    await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken).ContinueWith(t => { }).ConfigureAwait(false);
                }
            }

            this.logger.LogTrace("(-)");
        }

        /// <summary>
        /// Get the last processed block for a checkpoint type.
        /// </summary>
        /// <param name="type">The type of checkpoint.</param>
        /// <returns>The last processed block.</returns>
        private ChainedHeader LastProcessed(IndexerCheckpoints type)
        {
            return this.Chain.FindFork(this.AzureIndexer.GetCheckpointInternal(type).BlockLocator);
        }

        /// <summary>
        /// Perform indexing for a given checkpoint type.
        /// </summary>
        /// <param name="type">The checkpoint type to index.</param>
        /// <param name="fromHeight">The height to index from.</param>
        /// <param name="toHeight">The height to index to.</param>
        private void PerformIndexing(IndexerCheckpoints type, int fromHeight, int toHeight)
        {
            if (!this.nodeLifetime.ApplicationStopping.IsCancellationRequested)
            {
                // Index a batch of blocks/transactions/balances/wallets
                var fetcher = this.GetBlockFetcher(type);
                if (toHeight > fetcher._LastProcessed.Height)
                {
                    fetcher.FromHeight = Math.Max(fetcher._LastProcessed.Height + 1, fromHeight);
                    fetcher.ToHeight = toHeight;
                    IIndexTask task = null;
                    switch (type)
                    {
                        case IndexerCheckpoints.Blocks:
                            task = new IndexBlocksTask(this.IndexerConfig, this.loggerFactory);
                            break;
                        case IndexerCheckpoints.Transactions:
                            task = new IndexTransactionsTask(this.IndexerConfig, this.loggerFactory);
                            break;
                        case IndexerCheckpoints.Balances:
                            task = new IndexBalanceTask(this.IndexerConfig, null, this.loggerFactory);
                            break;
                        case IndexerCheckpoints.Wallets:
                            task = new IndexBalanceTask(this.IndexerConfig, this.IndexerConfig.CreateIndexerClient().GetAllWalletRules(), this.loggerFactory);
                            break;
                    }
                    task.SaveProgression = !this.indexerSettings.IgnoreCheckpoints;
                    task.Index(fetcher, this.AzureIndexer.TaskScheduler, this.FullNode.Network);
                }
            }
        }

        /// <summary>
        /// Performs indexing into Azure storage.
        /// </summary>
        /// <param name="cancellationToken">The token used for cancellation.</param>
        /// <returns>A task for asynchronous completion.</returns>
        private async Task IndexAsync(CancellationToken cancellationToken)
        {
            this.logger.LogTrace("()");

            while (this.StoreTip.Height < indexerSettings.To && !cancellationToken.IsCancellationRequested)
            {
                try
                {
                    // All indexes will progress more or less in step
                    int fromHeight = this.StoreTip.Height + 1;
                    int toHeight = Math.Min(this.StoreTip.Height + IndexBatchSize, this.indexerSettings.To);

                    this.logger.LogTrace("Starting indexing from {0}, to {1}", fromHeight, toHeight);

                    // Index a batch of blocks
                    PerformIndexing(IndexerCheckpoints.Blocks, fromHeight, toHeight);

                    // Index a batch of transactions
                    PerformIndexing(IndexerCheckpoints.Transactions, fromHeight, toHeight);

                    // Index a batch of balances
                    PerformIndexing(IndexerCheckpoints.Balances, fromHeight, toHeight);

                    // Index a batch of wallets
                    PerformIndexing(IndexerCheckpoints.Wallets, fromHeight, toHeight);

                    // Update the StoreTip 
                    UpdateStoreTip();
                    this.logger.LogTrace("Indexing iteration finished");
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    // If something goes wrong then try again 1 minute later
                    this.logger.LogError("Exception occurred: {0}", ex.ToString());
                    IndexerTrace.ErrorWhileImportingBlockToAzure(this.StoreTip.HashBlock, ex);
                    await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken).ContinueWith(t => { }).ConfigureAwait(false);
                }
            }

            this.logger.LogTrace("(-)");
        }

        /// <summary>
        /// Sets the StoreTip.
        /// </summary>
        /// <param name="ChainedHeader">The block to set the store tip to.</param>
        internal void SetStoreTip(ChainedHeader ChainedHeader)
        {
            this.logger.LogTrace("({0}:'{1}')", nameof(ChainedHeader), ChainedHeader?.HashBlock);
            Guard.NotNull(ChainedHeader, nameof(ChainedHeader));

            this.StoreTip = ChainedHeader;

            this.logger.LogTrace("(-)");
        }
    }
}