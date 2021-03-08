namespace Stratis.Features.AzureIndexer.Helpers
{
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using Stratis.Bitcoin.Builder;
    using Stratis.Bitcoin.Configuration.Logging;
    using Stratis.Features.AzureIndexer.Tokens;

    /// <summary>
    /// A class providing extension methods for <see cref="IFullNodeBuilder"/>.
    /// </summary>
    public static class IFullNodeBuilderExtensions
    {
        public static IFullNodeBuilder UseAzureIndexer(this IFullNodeBuilder fullNodeBuilder, Action<AzureIndexerSettings> setup = null)
        {
            LoggingConfiguration.RegisterFeatureNamespace<AzureIndexerFeature>("azindex");

            fullNodeBuilder.ConfigureFeature(features =>
            {
                features
                    .AddFeature<AzureIndexerFeature>()
                    .FeatureServices(services =>
                    {
                        services.AddSingleton<AzureIndexerLoop>();
                        services.AddSingleton(new AzureIndexerSettings(setup));
                    });
            });

            return fullNodeBuilder;
        }

        public static IFullNodeBuilder UseAzureIndexerOnSideChain(this IFullNodeBuilder fullNodeBuilder, Action<AzureIndexerSettings> setup = null)
        {
            LoggingConfiguration.RegisterFeatureNamespace<AzureIndexerFeature>("azindex");

            fullNodeBuilder.ConfigureFeature(features =>
            {
                features
                    .AddFeature<AzureIndexerFeature>()
                    .FeatureServices(services =>
                    {
                        services.AddSingleton<AzureIndexerLoop>();
                        services.AddSingleton(new AzureIndexerSettings(setup));
                        services.AddSingleton<LogDeserializer>();
                    });
            });

            return fullNodeBuilder;
        }
    }
}