using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using NBitcoin.Protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBitcoin.Indexer.Tests
{
    public class IndexerTester : IDisposable
    {
        private readonly AzureIndexer _Importer;
        public AzureIndexer Indexer
        {
            get
            {
                return _Importer;
            }
        }

		public static IndexerConfiguration CreateConfiguration()
		{
			var confBuilder = new ConfigurationBuilder();
			confBuilder.AddUserSecrets(typeof(IndexerTester).Assembly);
			var config = IndexerConfiguration.FromConfiguration(confBuilder.Build());
			return config;
		}

        string _Folder;
        public IndexerTester(string folder)
        {
            TestUtils.EnsureNew(folder);
			var config = CreateConfiguration();
            config.Network = Network.TestNet;
            config.StorageNamespace = folder;
            _Importer = config.CreateIndexer();

			List<Task> creating = new List<Task>();
            foreach (var table in config.EnumerateTables())
            {
				creating.Add(table.CreateIfNotExistsAsync());
            }

			creating.Add(config.GetBlocksContainer().CreateIfNotExistsAsync());
			Task.WaitAll(creating.ToArray());
            config.EnsureSetup();
            _Folder = folder;
        }


        #region IDisposable Members

        public void Dispose()
        {
            if (_NodeServer != null)
                _NodeServer.Dispose();
            if (!Cached)
            {
                foreach (var table in _Importer.Configuration.EnumerateTables())
                {
					table.CreateIfNotExistsAsync().GetAwaiter().GetResult();
                    var entities = table.ExecuteQueryAsync(new TableQuery()).GetAwaiter().GetResult().ToList();
                    Parallel.ForEach(entities, e =>
                    {
                        table.ExecuteAsync(TableOperation.Delete(e)).GetAwaiter().GetResult();
                    });
                }
                var container = _Importer.Configuration.GetBlocksContainer();
                var blobs = container.ListBlobsAsync("", true, BlobListingDetails.None).GetAwaiter().GetResult().ToList();

                Parallel.ForEach(blobs, b =>
                {
                    if (b is CloudPageBlob)
                        ((CloudPageBlob)b).DeleteAsync().GetAwaiter().GetResult();
                    else
                        ((CloudBlockBlob)b).DeleteAsync().GetAwaiter().GetResult();
                });
            }
        }


        #endregion

        public bool Cached
        {
            get;
            set;
        }


        public uint256 KnownBlockId = uint256.Parse("000000000933ea01ad0ee984209779baaec3ced90fa3f408719526f8d77f4943");
        public uint256 UnknownBlockId = uint256.Parse("000000000933ea01ad0ee984209779baaec3ced90fa3f408719526f8d77f4942");

        internal void ImportCachedBlocks()
        {
            CreateLocalNode().ChainBuilder.Load(@"..\..\..\Data\blocks");
            if (Client.GetBlock(KnownBlockId) == null)
            {
                Indexer.IgnoreCheckpoints = true;
                Indexer.FromHeight = 0;
                Indexer.IndexBlocks();
            }
        }

        internal void ImportCachedTransactions()
        {
            CreateLocalNode().ChainBuilder.Load(@"..\..\..\Data\blocks");
            if (Client.GetTransaction(KnownTransactionId) == null)
            {
                Indexer.IgnoreCheckpoints = true;
                Indexer.FromHeight = 0;
                Indexer.IndexTransactions();
            }
        }

        public IndexerClient _Client;
        public uint256 KnownTransactionId = uint256.Parse("4a5e1e4baab89f3a32518a88c31bc87f618f76673e2cc77ab2127b7afdeda33b");
        public uint256 UnknownTransactionId = uint256.Parse("4a5e1e4baab89f3a32518a88c31bc87f618f76673e2cc77ab2127b7afdeda33c");
        public IndexerClient Client
        {
            get
            {
                if (_Client == null)
                {
                    _Client = Indexer.Configuration.CreateIndexerClient();
                    _Client.BalancePartitionSize = 1;
                }
                return _Client;
            }
        }

        NodeServer _NodeServer;
        internal MiniNode CreateLocalNode()
        {
            NodeServer nodeServer = new NodeServer(Client.Configuration.Network, internalPort: (ushort)RandomUtils.GetInt32());
            nodeServer.Listen();
            _NodeServer = nodeServer;
            Indexer.Configuration.Node = "127.0.0.1:" + nodeServer.LocalEndpoint.Port;
            return new MiniNode(this, nodeServer);
        }

        internal ChainBuilder CreateChainBuilder()
        {
            return new ChainBuilder(this);
        }
    }
}
