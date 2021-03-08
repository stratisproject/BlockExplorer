using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NBitcoin;
using Stratis.Bitcoin.Builder.Feature;
using Stratis.Bitcoin.Configuration;
using Stratis.Bitcoin.Configuration.Logging;
using Stratis.Bitcoin.Utilities;

[assembly: InternalsVisibleTo("Stratis.Features.AzureIndexer.Tests")]

namespace Stratis.Features.AzureIndexer
{
    /// <summary>
    /// The AzureIndexerFeature provides the ".UseAzureIndexer" extension.
    /// </summary>
    public class AzureIndexerFeature : FullNodeFeature
    {
        /// <summary>The loop responsible for indexing blocks to azure.</summary>
        private readonly AzureIndexerLoop indexerLoop;

        /// <summary>The node's settings.</summary>
        private readonly NodeSettings nodeSettings;

        /// <summary>The Azure Indexer settings.</summary>
        private readonly AzureIndexerSettings indexerSettings;

        /// <summary>The name of this feature.</summary>
        private readonly string name;

        /// <summary>Instance logger.</summary>
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureIndexerFeature"/> class.
        /// Constructs the Azure Indexer feature.
        /// </summary>
        /// <param name="azureIndexerLoop">The loop responsible for indexing blocks to azure.</param>
        /// <param name="nodeSettings">The settings of the full node.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="indexerSettings">The Azure Indexer settings.</param>
        /// <param name="nodeStats">FullNode stats</param>
        /// <param name="name">The name of this feature.</param>
        public AzureIndexerFeature(
            AzureIndexerLoop azureIndexerLoop,
            NodeSettings nodeSettings,
            ILoggerFactory loggerFactory,
            AzureIndexerSettings indexerSettings,
            INodeStats nodeStats,
            string name = "AzureIndexer")
        {
            this.name = name;
            this.indexerLoop = azureIndexerLoop;
            this.logger = loggerFactory.CreateLogger(this.GetType().FullName);
            this.nodeSettings = nodeSettings;
            this.indexerSettings = indexerSettings;
            nodeStats.RegisterStats(this.AddNodeStats, StatsType.Inline, name, 1000);
        }

        /// <summary>
        /// Displays statistics in the console.
        /// </summary>
        /// <param name="benchLogs">The string builder to add the statistics to.</param>
        public void AddNodeStats(StringBuilder benchLogs)
        {
            ChainedHeader highestBlock = this.indexerLoop.StoreTip;

            if (highestBlock != null)
            {
                benchLogs.AppendLine($"{this.name}.Height: ".PadRight(LoggingConfiguration.ColumnLength + 1) +
                    highestBlock.Height.ToString().PadRight(8) +
                    $" {this.name}.Hash: ".PadRight(LoggingConfiguration.ColumnLength - 1) +
                    highestBlock.HashBlock);
            }

            benchLogs.AppendLine($"IBD.Status: ".PadRight(LoggingConfiguration.ColumnLength + 1) +
                                 this.indexerLoop.InitialBlockDownloadState.ToString().PadRight(8));
        }

        /// <summary>
        /// Starts the Azure Indexer feature.
        /// </summary>
        public void Initialize()
        {
            this.indexerLoop.Initialize();
        }

        public void LoadConfiguration()
        {
            this.indexerSettings.Load(this.nodeSettings);
        }

        public static void PrintHelp(Network network)
        {
            AzureIndexerSettings.PrintHelp(network);
        }

        public override Task InitializeAsync()
        {
            this.LoadConfiguration();
            this.indexerLoop.Initialize();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Stops the Azure Indexer feature.
        /// </summary>
        public override void Dispose()
        {
            this.logger.LogInformation("Stopping {0}...", this.name);
            this.indexerLoop.Shutdown();
        }
    }
}