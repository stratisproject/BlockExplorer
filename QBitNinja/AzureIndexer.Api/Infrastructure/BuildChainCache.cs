namespace AzureIndexer.Api.Infrastructure
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Serilog;

    public class BuildChainCache : BackgroundService
    {
        private readonly ChainCacheProvider provider;
        private readonly ILogger logger;
        private bool loaded = false;

        public BuildChainCache(ChainCacheProvider provider, ILogger logger)
        {
            this.provider = provider;
            this.logger = logger;
        }

        protected virtual TimeSpan Delay => TimeSpan.FromSeconds(10);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            this.logger.Information("{BuildChainCache} task is starting.", nameof(BuildChainCache));

            stoppingToken.Register(() => this.logger.Debug("{BuildChainCache} background task is stopping.", nameof(BuildChainCache)));

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (!this.provider.IsCacheAvailable || !this.loaded)
                    {
                        this.logger.Information("Loading and building cache.");
                        await this.provider.BuildCache();
                        this.loaded = true;
                    }
                }
                catch (Exception ex)
                {
                    this.logger.Warning(ex, "{ErrorMessage}", ex.Message);
                }

                await Task.Delay(this.Delay, stoppingToken);
            }
        }
    }
}
