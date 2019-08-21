using Stratis.Bitcoin.Features.AzureIndexer.Repositories;

namespace Stratis.Bitcoin.Features.AzureIndexer
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Microsoft.WindowsAzure.Storage.Auth;
    using NBitcoin;
    using Stratis.Bitcoin.AsyncWork;
    using Stratis.Bitcoin.Features.AzureIndexer.IndexTasks;
    using Stratis.Bitcoin.Utilities;

    /// <summary>
    /// The AzureIndexerStoreLoop loads blocks from the block repository and indexes them in Azure.
    /// </summary>
    public class AzureIndexerLoop
    {
        /// <summary>The number of blocks to index at a time.</summary>
        private const int IndexBatchSize = 100;

        /// <summary>Best chain of block headers.</summary>
        private readonly ChainIndexer chainIndexer;

        /// <summary>Factory for creating background async loop tasks.</summary>
        private readonly IAsyncProvider asyncProvider;

        /// <summary>Instance logger.</summary>
        private readonly ILogger logger;

        /// <summary>The node life time let us know when the node is shutting down.</summary>
        private readonly INodeLifetime nodeLifetime;

        private readonly ILoggerFactory loggerFactory;

        /// <summary>The Azure Indexer settings.</summary>
        private readonly AzureIndexerSettings indexerSettings;

        /// <summary>The async loop we need to wait upon before we can shut down this feature.</summary>
        private IAsyncLoop asyncLoop;

        /// <summary>Another async loop we need to wait upon before we can shut down this feature.</summary>
        private IAsyncLoop asyncLoopChain;

        /// <summary>Gets the full node that owns the block repository that we want to index.</summary>
        public FullNode FullNode { get; }

        /// <summary>Gets the name of this node feature for reporting stats.</summary>
        public virtual string StoreName => "AzureIndexer";

        /// <summary>Gets the Azure Indexer.</summary>
        public AzureIndexer AzureIndexer { get; private set; }

        public IndexerClient AzureIndexerClient { get; private set; }

        public BlockFetcher BlocksFetcher { get; private set; }

        public BlockFetcher TransactionsFetcher { get; private set; }

        public BlockFetcher BalancesFetcher { get; private set; }

        public BlockFetcher WalletsFetcher { get; private set; }

        /// <summary>Gets the Indexer Configuration.</summary>
        public IndexerConfiguration IndexerConfig { get; private set; }

        public bool InitialBlockDownloadState { get; set; }

        public Dictionary<string, int> StatsDictionary { get; set; } = new Dictionary<string, int>();

        /// <summary>Gets the highest block that has been indexed.</summary>
        internal ChainedHeader StoreTip { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureIndexerLoop"/> class.
        /// Constructs the AzureIndexerLoop.
        /// </summary>
        /// <param name="fullNode">The full node that will be indexed.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public AzureIndexerLoop(FullNode fullNode, ILoggerFactory loggerFactory)
        {
            this.asyncProvider = fullNode.AsyncProvider;
            this.FullNode = fullNode;
            this.chainIndexer = fullNode.ChainIndexer;
            this.nodeLifetime = fullNode.NodeLifetime;
            this.InitialBlockDownloadState = fullNode.InitialBlockDownloadState.IsInitialBlockDownload();
            this.indexerSettings = fullNode.NodeService<AzureIndexerSettings>();
            this.loggerFactory = loggerFactory;
            this.logger = loggerFactory.CreateLogger(this.GetType().FullName);
        }

        /// <summary>
        /// Derives an IndexerConfiguration object from the proviced AzureIndexerSettings object and network.
        /// </summary>
        /// <param name="indexerSettings">The AzureIndexerSettings object to use.</param>
        /// <param name="network">The network to use.</param>
        /// <param name="loggerFactory">logger factory</param>
        /// <param name="asyncProvider">current chain</param>
        /// <returns>An IndexerConfiguration object derived from the AzureIndexerSettings object and network.</returns>
        public static IndexerConfiguration IndexerConfigFromSettings(AzureIndexerSettings indexerSettings, Network network, ILoggerFactory loggerFactory, IAsyncProvider asyncProvider)
        {
            IndexerConfiguration indexerConfig = new IndexerConfiguration(loggerFactory, asyncProvider)
            {
                StorageNamespace = indexerSettings.StorageNamespace,
                Network = network,
                CheckpointSetName = indexerSettings.CheckpointsetName,
                AzureStorageEmulatorUsed = indexerSettings.AzureEmulatorUsed,
                StorageCredentials = indexerSettings.AzureEmulatorUsed ? null : new StorageCredentials(indexerSettings.AzureAccountName, indexerSettings.AzureKey),
                IsSidechain = indexerSettings.IsSidechain
            };
            return indexerConfig;
        }

        /// <summary>
        /// Initializes the Azure Indexer.
        /// </summary>
        public void Initialize()
        {
            this.IndexerConfig = IndexerConfigFromSettings(this.indexerSettings, this.FullNode.Network, this.loggerFactory, this.asyncProvider);

            AzureIndexer indexer = this.IndexerConfig.CreateIndexer();
            IndexerClient indexerClient = this.IndexerConfig.CreateIndexerClient();
            indexer.Configuration.EnsureSetup();
            indexer.TaskScheduler = new CustomThreadPoolTaskScheduler(30, 100);
            indexer.CheckpointInterval = this.indexerSettings.CheckpointInterval;
            indexer.IgnoreCheckpoints = this.indexerSettings.IgnoreCheckpoints;
            indexer.FromHeight = this.indexerSettings.From;
            indexer.ToHeight = this.indexerSettings.To;

            this.AzureIndexer = indexer;
            this.AzureIndexerClient = indexerClient;

            if (this.indexerSettings.IgnoreCheckpoints)
            {
                this.SetStoreTip(this.chainIndexer.GetHeader(indexer.FromHeight));
            }
            else
            {
                this.UpdateStoreTip();
            }

            this.StartLoop();
        }

        /// <summary>
        /// Shuts down the indexing loop.
        /// </summary>
        public void Shutdown()
        {
            this.asyncLoop.Dispose();

            this.logger.LogTrace("AsyncLoop disposed");

            this.asyncLoopChain.Dispose();
        }

        /// <summary>
        /// Update the store tip to match the last common processed block across all checkpoint types.
        /// </summary>
        private void UpdateStoreTip()
        {
            ChainedHeader lastBlocks = this.LastProcessed(IndexerCheckpoints.Blocks);
            ChainedHeader lastTransactions = this.LastProcessed(IndexerCheckpoints.Transactions);
            ChainedHeader lastBalances = this.LastProcessed(IndexerCheckpoints.Balances);
            ChainedHeader lastWallets = this.LastProcessed(IndexerCheckpoints.Wallets);

            if (!this.StatsDictionary.ContainsKey(IndexerCheckpoints.Blocks.ToString()))
            {
                this.StatsDictionary.Add(IndexerCheckpoints.Blocks.ToString(), lastBlocks?.Height ?? 0);
                this.StatsDictionary.Add(IndexerCheckpoints.Transactions.ToString(), lastTransactions?.Height ?? 0);
                this.StatsDictionary.Add(IndexerCheckpoints.Balances.ToString(), lastBalances?.Height ?? 0);
                this.StatsDictionary.Add(IndexerCheckpoints.Wallets.ToString(), lastWallets?.Height ?? 0);
            }
            else
            {
                this.StatsDictionary[IndexerCheckpoints.Blocks.ToString()] = lastBlocks?.Height ?? 0;
                this.StatsDictionary[IndexerCheckpoints.Transactions.ToString()] = lastTransactions?.Height ?? 0;
                this.StatsDictionary[IndexerCheckpoints.Balances.ToString()] = lastBalances?.Height ?? 0;
                this.StatsDictionary[IndexerCheckpoints.Wallets.ToString()] = lastWallets?.Height ?? 0;
            }

            var minHeight = lastBlocks?.Height ?? 0;
            minHeight = Math.Min(minHeight, lastTransactions?.Height ?? 0);
            minHeight = Math.Min(minHeight, lastBalances?.Height ?? 0);
            minHeight = Math.Min(minHeight, lastWallets?.Height ?? 0);
            this.SetStoreTip(this.chainIndexer.GetHeader(minHeight));
        }

        /// <summary>
        /// Determines the block that a checkpoint is at.
        /// </summary>
        /// <param name="indexerCheckpoints">The type of checkpoint (wallets, blocks, transactions or balances).</param>
        /// <returns>The block that a checkpoint is at.</returns>
        private ChainedHeader GetCheckPointBlock(IndexerCheckpoints indexerCheckpoints)
        {
            Checkpoint checkpoint = this.AzureIndexer.GetCheckpointInternal(indexerCheckpoints);
            ChainedHeader fork = this.chainIndexer.FindFork(checkpoint.BlockLocator);

            return fork;
        }

        /// <summary>
        /// Starts the indexing loop.
        /// </summary>
        private void StartLoop()
        {
            this.asyncLoop = this.asyncProvider.CreateAndRunAsyncLoop($"{this.StoreName}.IndexAsync", async token =>
                {
                    await this.IndexAsync(this.nodeLifetime.ApplicationStopping);
                },
                this.nodeLifetime.ApplicationStopping,
                repeatEvery: TimeSpans.RunOnce,
                startAfter: TimeSpans.FiveSeconds);

            this.asyncLoopChain = this.asyncProvider.CreateAndRunAsyncLoop($"{this.StoreName}.IndexChainAsync", async token =>
                {
                    await this.IndexChainAsync(this.nodeLifetime.ApplicationStopping);
                },
                this.nodeLifetime.ApplicationStopping,
                repeatEvery: TimeSpans.RunOnce,
                startAfter: TimeSpans.Minute);
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
            Checkpoint checkpoint = this.AzureIndexer.GetCheckpointInternal(indexerCheckpoints);
            FullNodeBlocksRepository repo = new FullNodeBlocksRepository(this.FullNode);

            BlockFetcher fetcher = new BlockFetcher(checkpoint, repo, this.chainIndexer, this.chainIndexer.FindFork(checkpoint.BlockLocator), this.loggerFactory)
            {
                NeedSaveInterval = this.indexerSettings.CheckpointInterval,
                FromHeight = this.StoreTip.Height + 1,
                ToHeight = Math.Min(this.StoreTip.Height + IndexBatchSize, this.indexerSettings.To),
                CancellationToken = this.nodeLifetime.ApplicationStopping
            };

            return fetcher;
        }

        /// <summary>
        /// Performs "Chain" indexing into Azure storage.
        /// </summary>
        /// <param name="cancellationToken">The token used for cancellation.</param>
        /// <returns>A task for asynchronous completion.</returns>
        private async Task IndexChainAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    this.AzureIndexer.IndexChain(this.chainIndexer, cancellationToken);
                    await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken).ContinueWith(t => { }).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    this.logger.LogTrace("Exception occurred: {0}", ex.ToString());

                    // If something goes wrong then try again 1 minute later.
                    IndexerTrace.ErrorWhileImportingBlockToAzure(this.StoreTip.HashBlock, ex);
                    await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken).ContinueWith(t => { }).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Get the last processed block for a checkpoint type.
        /// </summary>
        /// <param name="type">The type of checkpoint.</param>
        /// <returns>The last processed block.</returns>
        private ChainedHeader LastProcessed(IndexerCheckpoints type)
        {
            Checkpoint checkpoint = this.AzureIndexer.GetCheckpointInternal(type);
            BlockLocator blockLocator = checkpoint.BlockLocator;
            return this.chainIndexer.FindFork(blockLocator);
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
                if (toHeight > (fetcher._LastProcessed?.Height ?? 0))
                {
                    fetcher.FromHeight = Math.Max(fetcher._LastProcessed?.Height ?? 0 + 1, fromHeight);
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
            while (this.StoreTip.Height < this.indexerSettings.To && !cancellationToken.IsCancellationRequested)
            {
                this.InitialBlockDownloadState = this.FullNode.InitialBlockDownloadState.IsInitialBlockDownload();
                try
                {
                    // All indexes will progress more or less in step
                    var fromHeight = this.StoreTip.Height + 1;
                    var toHeight = Math.Min(this.StoreTip.Height + IndexBatchSize, this.indexerSettings.To);

                    // Index a batch of blocks
                    this.PerformIndexing(IndexerCheckpoints.Blocks, fromHeight, toHeight);

                    // Index a batch of transactions
                    this.PerformIndexing(IndexerCheckpoints.Transactions, fromHeight, toHeight);

                    // Index a batch of balances
                    this.PerformIndexing(IndexerCheckpoints.Balances, fromHeight, toHeight);

                    // Index a batch of wallets
                    this.PerformIndexing(IndexerCheckpoints.Wallets, fromHeight, toHeight);

                    // Update the StoreTip
                    this.UpdateStoreTip();
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
        }

        /// <summary>
        /// Sets the StoreTip.
        /// </summary>
        /// <param name="chainedHeader">The block to set the store tip to.</param>
        private void SetStoreTip(ChainedHeader chainedHeader)
        {
            Guard.NotNull(chainedHeader, nameof(chainedHeader));

            this.StoreTip = chainedHeader;
        }
    }
}