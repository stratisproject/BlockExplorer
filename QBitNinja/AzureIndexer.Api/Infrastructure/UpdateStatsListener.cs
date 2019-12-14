using Microsoft.Extensions.Logging;

namespace AzureIndexer.Api.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using NBitcoin;
    using Serilog;
    using Stratis.Bitcoin.Features.AzureIndexer;

    public class Stats
    {
        public class BlockStat
        {
            public long Size { get; set; }

            public int TransactionCount { get; set; }

            public int InputsCount { get; set; }

            public int OutputCounts { get; set; }
        }

        public List<BlockStat> BlocksStats { get; set; }

        public int TransactionCount { get => this.BlocksStats.Sum(b => b.TransactionCount); }

        public int InputsCount { get => this.BlocksStats.Sum(b => b.InputsCount); }

        public int OutputCounts { get => this.BlocksStats.Sum(b => b.OutputCounts); }

        public Stats()
        {
            this.BlocksStats = new List<BlockStat>();
        }
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

            List<Stats.BlockStat> newStats = new List<Stats.BlockStat>();

            while (currentTip != null && currentTip.Header.BlockTime.DateTime >= dayAgo)
            {
                var blockData = this.blockSearchService.GetBlock(currentTip.Height.ToString().ToBlockFeature(), false, false);

                if (blockData?.Block != null)
                {
                    Stats.BlockStat blockStat = new Stats.BlockStat();

                    newStats.Add(blockStat);

                    blockStat.Size = blockData.Block.BlockSize ?? 0;
                    blockStat.TransactionCount = blockData.Block.Transactions?.Count ?? 0;

                    if (blockStat.TransactionCount > 0)
                    {
                        foreach (Transaction transaction in blockData.Block.Transactions)
                        {
                            blockStat.InputsCount = transaction.Inputs.Count;
                            blockStat.OutputCounts = transaction.Outputs.Count;
                        }
                    }
                }

                currentTip = currentTip.Previous;
            }

            this.stats.BlocksStats = newStats;
        }
    }
}
