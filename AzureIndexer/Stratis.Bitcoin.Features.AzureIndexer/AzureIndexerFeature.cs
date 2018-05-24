using System;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NBitcoin;
using Stratis.Bitcoin.Builder;
using Stratis.Bitcoin.Builder.Feature;
using Stratis.Bitcoin.Configuration;
using Stratis.Bitcoin.Configuration.Logging;
using Stratis.Bitcoin.Interfaces;

[assembly: InternalsVisibleTo("Stratis.Bitcoin.Features.AzureIndexer.Tests")]

namespace Stratis.Bitcoin.Features.AzureIndexer
{
    /// <summary>
    /// The AzureIndexerFeature provides the ".UseAzureIndexer" extension.
    /// </summary>
    public class AzureIndexerFeature: FullNodeFeature, INodeStats
    {
        /// <summary>The loop responsible for indexing blocks to azure.</summary>
        protected readonly AzureIndexerLoop indexerLoop;

        /// <summary>The node's settings.</summary>
        protected readonly NodeSettings nodeSettings;

        /// <summary>The Azure Indexer settings.</summary>
        protected readonly AzureIndexerSettings indexerSettings;

        /// <summary>Instance logger.</summary>
        private readonly ILogger logger;

        /// <summary>The name of this feature.</summary>
        protected readonly string name;

        /// <summary>
        /// Constructs the Azure Indexer feature.
        /// </summary>
        /// <param name="azureIndexerLoop">The loop responsible for indexing blocks to azure.</param>
        /// <param name="nodeSettings">The settings of the full node.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="indexerSettings">The Azure Indexer settings.</param>
        /// <param name="name">The name of this feature.</param>
        public AzureIndexerFeature(
            AzureIndexerLoop azureIndexerLoop,
            NodeSettings nodeSettings,
            ILoggerFactory loggerFactory,
            AzureIndexerSettings indexerSettings,
            string name = "AzureIndexer")
        {
            this.name = name;
            this.indexerLoop = azureIndexerLoop;
            this.logger = loggerFactory.CreateLogger(this.GetType().FullName);
            this.nodeSettings = nodeSettings;
            this.indexerSettings = indexerSettings;
        }

        /// <summary>
        /// Displays statistics in the console.
        /// </summary>
        /// <param name="benchLogs">The sring builder to add the statistics to.</param>
        public void AddNodeStats(StringBuilder benchLogs)
        {
            var highestBlock = this.indexerLoop.StoreTip;

            if (highestBlock != null)
                benchLogs.AppendLine($"{this.name}.Height: ".PadRight(LoggingConfiguration.ColumnLength + 3) +
                    highestBlock.Height.ToString().PadRight(8) +
                    $" {this.name}.Hash: ".PadRight(LoggingConfiguration.ColumnLength + 3) +
                    highestBlock.HashBlock);
        }

        /// <summary>
        /// Starts the Azure Indexer feature.
        /// </summary>
        public override void Initialize()
        {
            this.logger.LogTrace("()");
            this.indexerLoop.Initialize();         
            this.logger.LogTrace("(-)");
        }

        public override void LoadConfiguration()
        {
            this.indexerSettings.Load(this.nodeSettings);
        }

        public static void PrintHelp(Network network)
        {
            AzureIndexerSettings.PrintHelp(network);
        }

        /// <summary>
        /// Stops the Azure Indexer feature.
        /// </summary>
        public override void Dispose()
        {
            this.logger.LogInformation("Stopping {0}...", this.name);
            this.indexerLoop.Shutdown();
            this.logger.LogTrace("(-)");
        }
    }

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