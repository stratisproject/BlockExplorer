using System;
using Microsoft.Extensions.DependencyInjection;
using Stratis.Bitcoin.Builder;
using Stratis.Bitcoin.Configuration.Logging;

namespace Stratis.Bitcoin.Features.AzureIndexer
{
    /// <summary>
    /// A class providing extension methods for <see cref="IFullNodeBuilder"/>.
    /// </summary>
    public static partial class IFullNodeBuilderExtensions
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
                        services.AddSingleton<AzureIndexerSettings>(new AzureIndexerSettings(setup));
                    });
            });

            return fullNodeBuilder;
        }
    }
}