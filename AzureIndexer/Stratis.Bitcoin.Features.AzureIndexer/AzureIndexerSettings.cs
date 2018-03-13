using Microsoft.Extensions.Logging;
using NBitcoin;
using Stratis.Bitcoin.Configuration;
using System;
using System.Text;

namespace Stratis.Bitcoin.Features.AzureIndexer
{
    /// <summary>
    /// Configuration related to Azure Indexer feature.
    /// </summary>
    public class AzureIndexerSettings
    {
        /// <summary>Azure storage account.</summary>
        public string AzureAccountName { get; set; }

        /// <summary>Azure storage account key.</summary>
        public string AzureKey { get; set; }

        /// <summary>Azure storage emulator used.</summary>
        public bool AzureEmulatorUsed { get; set; }

        /// <summary>Checkpoint interval determines how often to record checkpoints.</summary>
        public TimeSpan CheckpointInterval { get; set; }

        /// <summary>Checkpointset name.</summary>
        public string CheckpointsetName { get; set; }

        /// <summary>Determines whether to regard or update checkpoints.</summary>
        public bool IgnoreCheckpoints { get; set; }

        /// <summary>The block to start indexing from.</summary>
        public int From { get; set; }

        /// <summary>The last block to index.</summary>
        public int To { get; set; }

        /// <summary>The storage namespace to use.</summary>
        public string StorageNamespace { get; set; }

        /// <summary>The callback used to modify settings on startup.</summary>
        private Action<AzureIndexerSettings> callback = null;

        /// <summary>
        /// Initializes an instance of the object.
        /// </summary>
        public AzureIndexerSettings()
        {
            this.AzureEmulatorUsed = true;
            this.From = 0;
            this.To = int.MaxValue;
            this.StorageNamespace = "";
            this.CheckpointsetName = "default";
            this.CheckpointInterval = TimeSpan.Parse("00:15:00");
            
        }

        /// <summary>
        /// Initializes an instance of the object.
        /// </summary>
        /// <param name="callback">A callback for modifying the settings during startup.</param>
        public AzureIndexerSettings(Action<AzureIndexerSettings> callback) : this()
        {
            this.callback = callback;
        }

        /// <summary>
        /// Loads the Azure Indexer settings from the application configuration.
        /// </summary>
        /// <param name="nodeSettings">Application configuration.</param>
        private void LoadSettingsFromConfig(NodeSettings nodeSettings)
        {
            var config = nodeSettings.ConfigReader;
            this.AzureEmulatorUsed = config.GetOrDefault<bool>("azemu", false);
            if (!this.AzureEmulatorUsed)
            {
                this.AzureAccountName = config.GetOrDefault<string>("azureacc", "");
                this.AzureKey = config.GetOrDefault<string>("azurekey", "");
                // Mime-encoded-data strings should always be a multiple of 4 in length. Provide trailing '='s if omitted..
                this.AzureKey = (this.AzureKey + "===").Substring(0, AzureKey.Length + 3 - ((this.AzureKey.Length + 3) % 4));
            }
            this.CheckpointInterval = TimeSpan.Parse(config.GetOrDefault<string>("chkptint", "00:15:00"));
            this.IgnoreCheckpoints = config.GetOrDefault<bool>("nochkpts", false);
            this.From = int.Parse(config.GetOrDefault<string>("indexfrom", "0"));
            this.To = int.Parse(config.GetOrDefault<string>("indexto", int.MaxValue.ToString()));
            this.CheckpointsetName = config.GetOrDefault<string>("chkptset", "default");
            this.StorageNamespace = config.GetOrDefault<string>("indexprefix", "");
        }

        /// <summary>
        /// Loads the Azure Indexer settings from the application configuration.
        /// Allows the callback to override those settings.
        /// </summary>
        /// <param name="nodeSettings">Application configuration.</param>
        public void Load(NodeSettings nodeSettings)
        {
            // Get values from config
            this.LoadSettingsFromConfig(nodeSettings);

            // Invoke callback
            this.callback?.Invoke(this);
        }

        /// <summary>
        /// Prints command line help.
        /// </summary>
        /// <param name="mainNet">Used for network-specific help (if any).</param>
        public static void PrintHelp(Network mainNet)
        {
            var defaults = NodeSettings.Default();
            var builder = new StringBuilder();

            builder.AppendLine($"-azureacc=<string>        Azure account name.");
            builder.AppendLine($"-azurekey=<string>        Azure account key.");
            builder.AppendLine($"-azemu                    Azure storage emulator used. Default is not to use the emulator.");
            builder.AppendLine($"-chkptint=<hh:mm:ss>      Indexing checkpoint interval.");
            builder.AppendLine($"-nochkpts                 Do not use checkpoints. Default is to use checkpoints.");
            builder.AppendLine($"-indexfrom=<int (0 to N)> Block height to start indexing from.");
            builder.AppendLine($"-indexto=<int (0 to N)>   Maximum block height to index.");
            builder.AppendLine($"-chkptset=<string>        Checkpointset name. Default is 'default'.");
            builder.AppendLine($"-indexprefix=<string>     Name prefix for index tables and blob container.");

            defaults.Logger.LogInformation(builder.ToString());
        }
    }
}