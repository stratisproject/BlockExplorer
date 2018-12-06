using Serilog;

namespace AzureIndexer.Api.Infrastructure
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using NBitcoin;
    using Stratis.Bitcoin.Features.AzureIndexer;

    public class UpdateChainListener : BackgroundService
    {
        private readonly IndexerClient indexer;
        private readonly ConcurrentChain chain;
        private readonly ILogger logger;

        public UpdateChainListener(
            IndexerClient indexer,
            ConcurrentChain chain,
            ILogger logger)
        {
            this.indexer = indexer;
            this.chain = chain;
            this.logger = logger;
        }

        protected virtual TimeSpan Delay => TimeSpan.FromSeconds(10);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            this.logger.Information("{UpdateChainListener} task is starting.", nameof(UpdateChainListener));

            stoppingToken.Register(() => this.logger.Debug("{UpdateChainListener} background task is stopping.", nameof(UpdateChainListener)));

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    this.UpdateChain();
                }
                catch (Exception ex)
                {
                    this.logger.Warning(ex, "{ErrorMessage}", ex.Message);
                }

                await Task.Delay(this.Delay, stoppingToken);
            }
        }

        private bool UpdateChain()
        {
            var oldTip = this.chain.Tip.HashBlock;
            var changes = this.indexer.GetChainChangesUntilFork(this.chain.Tip, false);
            changes.UpdateChain(this.chain);
            var newTip = this.chain.Tip.HashBlock;
            return newTip != oldTip;
        }
    }
}
