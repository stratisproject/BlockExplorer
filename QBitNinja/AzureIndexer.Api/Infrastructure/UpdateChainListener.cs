namespace AzureIndexer.Api.Infrastructure
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using NBitcoin;
    using Stratis.Bitcoin.Features.AzureIndexer;

    public class UpdateChainListener : IHostedService
    {
        private readonly IndexerClient indexer;
        private readonly ConcurrentChain chain;
        private readonly QBitNinjaConfiguration config;
        private IDisposable subscription;

        public UpdateChainListener(IndexerClient indexer, ConcurrentChain chain, QBitNinjaConfiguration config)
        {
            this.indexer = indexer;
            this.chain = chain;
            this.config = config;
        }

        internal Timer Timer { get; set; }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            this.Timer = new Timer(_ => this.UpdateChain());
            this.Timer.Change(0, (int)TimeSpan.FromSeconds(30).TotalMilliseconds);

            this.subscription =
                this.config.Topics
                    .NewBlocks
                    .CreateConsumer("webchain", true)
                    .EnsureSubscriptionExists()
                    .OnMessage(b =>
                    {
                        this.UpdateChain();
                    });
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            this.Timer?.Dispose();

            this.subscription?.Dispose();

            return Task.CompletedTask;
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
