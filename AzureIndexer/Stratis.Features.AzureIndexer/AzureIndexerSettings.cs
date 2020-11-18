namespace Stratis.Features.AzureIndexer
{
    using System;
    using System.Text;
    using Microsoft.Extensions.Logging;
    using NBitcoin;
    using Stratis.Bitcoin.Configuration;

    /// <summary>
    /// Configuration related to Azure Indexer feature.
    /// </summary>
    public class AzureIndexerSettings
    {
        /// <summary>Gets or sets azure storage account.</summary>
        public string AzureAccountName { get; set; }

        /// <summary>Gets or sets azure storage account key.</summary>
        public string AzureKey { get; set; }

        /// <summary>Gets or sets a value indicating whether azure storage emulator used.</summary>
        public bool AzureEmulatorUsed { get; set; }

        /// <summary>Gets or sets checkpoint interval determines how often to record checkpoints.</summary>
        public TimeSpan CheckpointInterval { get; set; }

        /// <summary>Gets or sets checkpointset name.</summary>
        public string CheckpointsetName { get; set; }

        /// <summary>Gets or sets a value indicating whether determines whether to regard or update checkpoints.</summary>
        public bool IgnoreCheckpoints { get; set; }

        /// <summary>Gets or sets the block to start indexing from.</summary>
        public int From { get; set; }

        /// <summary>Gets or sets the last block to index.</summary>
        public int To { get; set; }

        /// <summary>Gets or sets the storage namespace to use.</summary>
        public string StorageNamespace { get; set; }

        public bool IsSidechain { get; set; }

        /// <summary>The callback used to modify settings on startup.</summary>
        private Action<AzureIndexerSettings> callback = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureIndexerSettings"/> class.
        /// </summary>
        public AzureIndexerSettings()
        {
            this.AzureEmulatorUsed = false;
            this.From = 0;
            this.To = int.MaxValue;
            this.StorageNamespace = string.Empty;
            this.CheckpointsetName = "default";
            this.CheckpointInterval = TimeSpan.Parse("00:15:00");
            this.IsSidechain = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureIndexerSettings"/> class.
        /// Initializes an instance of the object.
        /// </summary>
        /// <param name="callback">A callback for modifying the settings during startup.</param>
        public AzureIndexerSettings(Action<AzureIndexerSettings> callback)
            : this()
        {
            this.callback = callback;
        }

        /// <summary>
        /// Loads the Azure Indexer settings from the application configuration.
        /// </summary>
        /// <param name="nodeSettings">Application configuration.</param>
        private void LoadSettingsFromConfig(NodeSettings nodeSettings)
        {
            TextFileConfiguration config = nodeSettings.ConfigReader;
            this.AzureEmulatorUsed = config.GetOrDefault<bool>("azemu", false);
            if (!this.AzureEmulatorUsed)
            {
                this.AzureAccountName = config.GetOrDefault<string>("azureacc", "");
                this.AzureKey = config.GetOrDefault<string>("azurekey", "");

                // Mime-encoded-data strings should always be a multiple of 4 in length. Provide trailing '='s if omitted..
                this.AzureKey = (this.AzureKey + "===").Substring(0, AzureKey.Length + 3 - ((this.AzureKey.Length + 3) % 4));

                this.IsSidechain = config.GetOrDefault<bool>("sidechain", false);
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
            NodeSettings defaults = NodeSettings.Default(mainNet);
            StringBuilder builder = new StringBuilder();

            builder.AppendLine($"-azureacc=<string>        Azure account name.");
            builder.AppendLine($"-azurekey=<string>        Azure account key.");
            builder.AppendLine($"-sidechain=<bool>         Is SmartContracts indexer.");
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