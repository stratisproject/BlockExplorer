using Microsoft.Extensions.Logging;
using Stratis.Features.AzureIndexer;

namespace AzureIndexer.Api.Infrastructure
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using NBitcoin;
    using Serilog;

    public class Stats
    {
        public int BlockCount { get; set; }

        public int TxInputsCount { get; set; }
    }

    public class UpdateStatsListener : BackgroundService
    {
        private readonly IndexerClient indexer;
        private readonly ChainIndexer chain;
        private readonly ILogger logger;
        private readonly ILoggerFactory loggerFactory;
        private readonly Stats stats;
        private readonly ChainCacheProvider provider;
        private readonly IBlockSearchService blockSearchService;
        private readonly ITransactionSearchService transactionSearchService;
        private bool calculating = false;

        public UpdateStatsListener(
            IndexerClient indexer,
            ChainIndexer chain,
            ILogger logger,
            ChainCacheProvider provider,
            IBlockSearchService blockSearchService,
            ITransactionSearchService transactionSearchService,
            ILoggerFactory loggerFactory,
            Stats stats)
        {
            this.indexer = indexer;
            this.chain = chain;
            this.logger = logger;
            this.provider = provider;
            this.blockSearchService = blockSearchService;
            this.transactionSearchService = transactionSearchService;
            this.loggerFactory = loggerFactory;
            this.stats = stats;
        }

        protected virtual TimeSpan Delay => TimeSpan.FromSeconds(103);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            this.logger.Information("{UpdateStatsListener} task is starting.", nameof(UpdateStatsListener));

            stoppingToken.Register(() => this.logger.Debug("{UpdateStatsListener} background task is stopping.", nameof(UpdateStatsListener)));

            while (!stoppingToken.IsCancellationRequested)
            {
                if (!this.calculating)
                {
                    this.calculating = true;
                    try
                    {
                        this.UpdateStats();
                        this.calculating = false;
                    }
                    catch (Exception ex)
                    {
                        this.calculating = false;
                        this.logger.Warning(ex, "{ErrorMessage}", ex.Message);
                    }
                }

                await Task.Delay(this.Delay, stoppingToken);
            }
        }

        private void UpdateStats()
        {
            var currentTip = this.chain.Tip;
            var dayAgo = DateTime.UtcNow.AddHours(-24);
            var blocksCount = 0;
            var transactionInputCounts = 0;

            while (currentTip != null && currentTip.Header.BlockTime.DateTime >= dayAgo)
            {
                blocksCount++;
                if (!this.indexer.Configuration.IsSidechain)
                {
                    var blockData =
                        this.blockSearchService.GetBlock(currentTip.Height.ToString().ToBlockFeature(), false, false);
                    if (blockData?.Block?.Transactions?.Count > 0)
                    {
                        foreach (var transaction in blockData.Block.Transactions)
                        {
                            transactionInputCounts += transaction.Inputs.Count;
                        }
                    }
                }

                currentTip = currentTip.Previous;
            }

            this.stats.BlockCount = blocksCount;
            this.stats.TxInputsCount = transactionInputCounts;
        }
    }
}
