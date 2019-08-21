using Microsoft.Extensions.Logging;
using Stratis.Bitcoin.Features.AzureIndexer.Helpers;

namespace AzureIndexer.Api.Infrastructure
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using NBitcoin;
    using Serilog;
    using Stratis.Bitcoin.Features.AzureIndexer;

    public class UpdateChainListener : BackgroundService
    {
        private readonly IndexerClient indexer;
        private readonly ChainIndexer chain;
        private readonly ILogger logger;
        private readonly ILoggerFactory loggerFactory;
        private readonly ChainCacheProvider provider;

        public UpdateChainListener(
            IndexerClient indexer,
            ChainIndexer chain,
            ILogger logger,
            ChainCacheProvider provider,
            ILoggerFactory loggerFactory)
        {
            this.indexer = indexer;
            this.chain = chain;
            this.logger = logger;
            this.provider = provider;
            this.loggerFactory = loggerFactory;
        }

        protected virtual TimeSpan Delay => TimeSpan.FromSeconds(13);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            this.logger.Information("{UpdateChainListener} task is starting.", nameof(UpdateChainListener));

            stoppingToken.Register(() => this.logger.Debug("{UpdateChainListener} background task is stopping.", nameof(UpdateChainListener)));

            while (!stoppingToken.IsCancellationRequested)
            {
                if (this.provider.IsCacheAvailable)
                {
                    try
                    {
                        this.UpdateChain();
                    }
                    catch (Exception ex)
                    {
                        this.logger.Warning(ex, "{ErrorMessage}", ex.Message);
                    }
                }

                await Task.Delay(this.Delay, stoppingToken);
            }
        }

        private bool UpdateChain()
        {
            var oldTip = this.chain.Tip.HashBlock;
            var changes = this.indexer.GetChainChangesUntilFork(this.chain.Tip, false);
            changes.UpdateChain(this.chain, this.loggerFactory.CreateLogger<UpdateChainListener>());
            var newTip = this.chain.Tip.HashBlock;
            return newTip != oldTip;
        }
    }
}
