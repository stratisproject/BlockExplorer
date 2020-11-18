namespace Stratis.Features.AzureIndexer.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.WindowsAzure.Storage.Blob;
    using Microsoft.WindowsAzure.Storage.Table;
    using NBitcoin;
    using Stratis.Bitcoin.AsyncWork;
    using Stratis.Bitcoin.Networks;
    using Stratis.Features.AzureIndexer.Helpers;
    using Stratis.Features.AzureIndexer.Repositories;

    public class IndexerTester : IDisposable
    {
        private readonly AzureIndexer _Importer;

        public AzureIndexer Indexer => this._Importer;

        public static IndexerConfiguration CreateConfiguration(ILoggerFactory loggerFactory, IAsyncProvider asyncProvider)
        {
            ConfigurationBuilder confBuilder = new ConfigurationBuilder();
            IndexerConfiguration config = new IndexerConfiguration(confBuilder.Build(), loggerFactory, asyncProvider);
            return config;
        }

        string _Folder;

        public IndexerTester(string folder)
        {
            TestUtils.EnsureNew(folder);

            var config = AzureIndexerLoop.IndexerConfigFromSettings(new AzureIndexerSettings() { StorageNamespace = folder }, new StraxTest(), new LoggerFactory(), this.Indexer.Configuration.AsyncProvider);

            config.EnsureSetup();

            this._Importer = config.CreateIndexer();

            List<Task> creating = new List<Task>();
            foreach (var table in config.EnumerateTables())
            {
                creating.Add(table.CreateIfNotExistsAsync());
            }

            creating.Add(config.GetBlocksContainer().CreateIfNotExistsAsync());
            Task.WaitAll(creating.ToArray());

            this._Folder = folder;
        }

        #region IDisposable Members

        public void Dispose()
        {
            // TODO: Find a NodeServer replacement and fix this code
            /*
            if (_NodeServer != null)
                _NodeServer.Dispose();
            */
            if (!this.Cached)
            {
                foreach (var table in this._Importer.Configuration.EnumerateTables())
                {
                    table.CreateIfNotExistsAsync().GetAwaiter().GetResult();
                    var entities = table.ExecuteQuery(new TableQuery()).ToList();
                    Parallel.ForEach(entities, e =>
                    {
                        table.ExecuteAsync(TableOperation.Delete(e)).GetAwaiter().GetResult();
                    });
                }
                var container = this._Importer.Configuration.GetBlocksContainer();
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

        public bool Cached { get; set; }

        public uint256 KnownBlockId = uint256.Parse("000000000933ea01ad0ee984209779baaec3ced90fa3f408719526f8d77f4943");
        public uint256 UnknownBlockId = uint256.Parse("000000000933ea01ad0ee984209779baaec3ced90fa3f408719526f8d77f4942");

        // TODO: Fix IndexBlocks and this code
        /*
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
        */

        public IndexerClient _Client;
        public uint256 KnownTransactionId = uint256.Parse("4a5e1e4baab89f3a32518a88c31bc87f618f76673e2cc77ab2127b7afdeda33b");
        public uint256 UnknownTransactionId = uint256.Parse("4a5e1e4baab89f3a32518a88c31bc87f618f76673e2cc77ab2127b7afdeda33c");

        public IndexerClient Client
        {
            get
            {
                if (this._Client == null)
                {
                    this._Client = this.Indexer.Configuration.CreateIndexerClient();
                    this._Client.BalancePartitionSize = 1;
                }
                return this._Client;
            }
        }

        // TODO: Find a NodeServer replacement and fix this code
        /*
        NodeServer _NodeServer;
        internal MiniNode CreateLocalNode()
        {
            NodeServer nodeServer = new NodeServer(Client.Configuration.Network, internalPort: (ushort)RandomUtils.GetInt32());
            nodeServer.Listen();
            _NodeServer = nodeServer;
            Indexer.Configuration.Node = "127.0.0.1:" + nodeServer.LocalEndpoint.Port;
            return new MiniNode(this, nodeServer);
        }
        */
        internal ChainBuilder CreateChainBuilder()
        {
            return new ChainBuilder(this);
        }
    }
}
