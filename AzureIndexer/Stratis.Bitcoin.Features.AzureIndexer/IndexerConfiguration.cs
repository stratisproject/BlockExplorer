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
    using Stratis.Bitcoin.AsyncWork;
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
        private const string TokenTransactionsTableName = "tokentxs";

        private readonly ILoggerFactory loggerFactory;

        private CloudTableClient tableClient;

        public bool IsSidechain { get; set; }

        public IAsyncProvider AsyncProvider { get; }

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

        public IndexerConfiguration(ILoggerFactory loggerFactory, IAsyncProvider asyncProvider)
        {
            this.AsyncProvider = asyncProvider;
            this.loggerFactory = loggerFactory;
            this.Network = Networks.Networks.Stratis.Mainnet();
        }

        public IndexerConfiguration(IConfiguration config, ILoggerFactory loggerFactory, IAsyncProvider asyncProvider)
        {
            this.AsyncProvider = asyncProvider;
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

            NetworkPeerFactory networkPeerFactory = new NetworkPeerFactory(this.Network, DateTimeProvider.Default, this.loggerFactory, new PayloadProvider().DiscoverPayloads(), null, null, null, this.AsyncProvider);
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
            CloudTable table = this.TableClient.GetTableReference(this.GetFullName(TransactionsTableName));
            table.CreateIfNotExistsAsync();
            return table;
        }

        public CloudTable GetSmartContactTable()
        {
            CloudTable table = this.TableClient.GetTableReference(this.GetFullName(SmartContractsTableName));
            table.CreateIfNotExistsAsync();
            return table;
        }

        public CloudTable GetSmartContactDetailTable()
        {
            CloudTable table = this.TableClient.GetTableReference(this.GetFullName(SmartContractDetailsTableName));
            table.CreateIfNotExistsAsync();
            return table;
        }

        public CloudTable GetWalletRulesTable()
        {
            CloudTable table = this.TableClient.GetTableReference(this.GetFullName(WalletsTableName));
            table.CreateIfNotExistsAsync();
            return table;
        }

        public CloudTable GetTable(string tableName)
        {
            CloudTable table = this.TableClient.GetTableReference(this.GetFullName(tableName));
            table.CreateIfNotExistsAsync();
            return table;
        }

        public CloudTable GetBalanceTable()
        {
            CloudTable table = this.TableClient.GetTableReference(this.GetFullName(BalancesTableName));
            table.CreateIfNotExistsAsync();
            return table;
        }

        public CloudTable GetChainTable()
        {
            CloudTable table = this.TableClient.GetTableReference(this.GetFullName(ChainTableName));
            table.CreateIfNotExistsAsync();
            return table;
        }

        public CloudTable GetTokenTransactionTable()
        {
            CloudTable table = this.TableClient.GetTableReference(this.GetFullName(TokenTransactionsTableName));
            table.CreateIfNotExistsAsync();
            return table;
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
