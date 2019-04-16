namespace Stratis.Bitcoin.Features.AzureIndexer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.ExceptionServices;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Auth;
    using Microsoft.WindowsAzure.Storage.Blob;
    using Microsoft.WindowsAzure.Storage.Table;
    using NBitcoin;
    using NBitcoin.Protocol;
    using Stratis.Bitcoin.P2P.Peer;
    using Stratis.Bitcoin.P2P.Protocol.Payloads;
    using Utilities;

    public class IndexerConfiguration
    {
        private const string IndexerBlobContainerName = "indexer";
        private const string TransactionsTableName = "transactions";
        private const string SmartContractsTableName = "smartcontracts";
        private const string SmartContractDetailsTableName = "smartcontractdetails";
        private const string BalancesTableName = "balances";
        private const string ChainTableName = "chain";
        private const string WalletsTableName = "wallets";

        private readonly ILoggerFactory loggerFactory;

        private CloudTableClient tableClient;

        public bool IsSidechain;

        public Network Network { get; set; }

        public bool AzureStorageEmulatorUsed { get; set; }

        public string Node { get; set; }

        public string CheckpointSetName { get; set; }

        public StorageCredentials StorageCredentials { get; set; }

        public string StorageNamespace { get; set; }

        public CloudStorageAccount StorageAccount { get; set; }

        public CloudTableClient TableClient
        {
            get
            {
                if (this.tableClient != null)
                {
                    return this.tableClient;
                }

                this.tableClient = this.StorageAccount.CreateCloudTableClient();
                return this.tableClient;
            }
            set => this.tableClient = value;
        }

        private CloudBlobClient blobClient;

        public CloudBlobClient BlobClient
        {
            get
            {
                if (this.blobClient != null)
                {
                    return this.blobClient;
                }

                this.blobClient = this.StorageAccount.CreateCloudBlobClient();
                return this.blobClient;
            }
            set => this.blobClient = value;
        }

        public IndexerConfiguration(ILoggerFactory loggerFactory)
        {
            this.loggerFactory = loggerFactory;
            this.Network = Networks.Networks.Stratis.Mainnet();
        }

        public IndexerConfiguration(IConfiguration config, ILoggerFactory loggerFactory)
        {
            this.loggerFactory = loggerFactory;

            var account = GetValue(config, "Azure.AccountName", true);
            var key = GetValue(config, "Azure.Key", true);
            this.StorageNamespace = GetValue(config, "StorageNamespace", false);
            var network = GetValue(config, "Network", false) ?? "Main";

            var sidechain = GetValue(config, "Sidechain", true) ?? "false";

            if (!string.IsNullOrEmpty(sidechain))
            {
                this.IsSidechain = bool.Parse(sidechain);
            }

            this.Network = NetworkHelpers.GetNetwork(network);
            if (this.Network == null)
            {
                throw new IndexerConfigurationErrorsException($"Invalid value {network} in appSettings (expecting Main, Test or Seg)");
            }

            this.Node = GetValue(config, "Node", false);
            this.CheckpointSetName = GetValue(config, "CheckpointSetName", false);
            if (string.IsNullOrWhiteSpace(this.CheckpointSetName))
            {
                this.CheckpointSetName = $"default";
            }

            var emulator = GetValue(config, "AzureStorageEmulatorUsed", false);

            if (!string.IsNullOrWhiteSpace(emulator))
            {
                this.AzureStorageEmulatorUsed = bool.Parse(emulator);
            }

            this.StorageCredentials = this.AzureStorageEmulatorUsed ? null : new StorageCredentials(account, key);
        }

        public NetworkPeer ConnectToNode(bool isRelay)
        {
            if (string.IsNullOrEmpty(this.Node))
            {
                throw new IndexerConfigurationErrorsException("Node setting is not configured");
            }

            NetworkPeerFactory networkPeerFactory = new NetworkPeerFactory(this.Network, DateTimeProvider.Default, new LoggerFactory(), new PayloadProvider().DiscoverPayloads(), null, null, null); // TODO: fix last 3 parameters
            return (NetworkPeer)networkPeerFactory.CreateConnectedNetworkPeerAsync(this.Node, ProtocolVersion.PROTOCOL_VERSION, isRelay: isRelay).Result;
        }

        public Task EnsureSetupAsync()
        {
            List<Task> tasks = this.EnumerateTables()
                .Select(t => t.CreateIfNotExistsAsync())
                .OfType<Task>()
                .ToList();
            tasks.Add(this.GetBlocksContainer().CreateIfNotExistsAsync());
            return Task.WhenAll(tasks.ToArray());
        }

        public void EnsureSetup()
        {
            try
            {
                this.StorageAccount = this.AzureStorageEmulatorUsed ?
                    CloudStorageAccount.Parse("UseDevelopmentStorage=true;") :
                    new CloudStorageAccount(this.StorageCredentials, true);

                this.EnsureSetupAsync().Wait();
            }
            catch (AggregateException aex)
            {
                ExceptionDispatchInfo.Capture(aex).Throw();
            }
        }

        public IEnumerable<CloudTable> EnumerateTables()
        {
            yield return this.GetTransactionTable();
            yield return this.GetSmartContactTable();
            yield return this.GetSmartContactDetailTable();
            yield return this.GetBalanceTable();
            yield return this.GetChainTable();
            yield return this.GetWalletRulesTable();
        }

        public AzureIndexer CreateIndexer()
        {
            return new AzureIndexer(this, this.loggerFactory);
        }

        public IndexerClient CreateIndexerClient()
        {
            return new IndexerClient(this);
        }

        public CloudTable GetTransactionTable()
        {
            return this.TableClient.GetTableReference(this.GetFullName(TransactionsTableName));
        }

        public CloudTable GetSmartContactTable()
        {
            return this.TableClient.GetTableReference(this.GetFullName(SmartContractsTableName));
        }

        public CloudTable GetSmartContactDetailTable()
        {
            return this.TableClient.GetTableReference(this.GetFullName(SmartContractDetailsTableName));
        }

        public CloudTable GetWalletRulesTable()
        {
            return this.TableClient.GetTableReference(this.GetFullName(WalletsTableName));
        }

        public CloudTable GetTable(string tableName)
        {
            return this.TableClient.GetTableReference(this.GetFullName(tableName));
        }

        public CloudTable GetBalanceTable()
        {
            return this.TableClient.GetTableReference(this.GetFullName(BalancesTableName));
        }

        public CloudTable GetChainTable()
        {
            return this.TableClient.GetTableReference(this.GetFullName(ChainTableName));
        }

        public CloudBlobContainer GetBlocksContainer()
        {
            return this.BlobClient.GetContainerReference(this.GetFullName(IndexerBlobContainerName));
        }

        protected static string GetValue(IConfiguration config, string setting, bool required)
        {
            var result = config[setting];
            result = string.IsNullOrWhiteSpace(result) ? null : result;
            if (result == null && required)
            {
                throw new IndexerConfigurationErrorsException("AppSetting " + setting + " not found");
            }

            return result;
        }

        private string GetFullName(string storageObjectName)
        {
            return (this.StorageNamespace + storageObjectName).ToLowerInvariant();
        }
    }
}
