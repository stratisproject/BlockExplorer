using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AzureIndexer.Api.Models;
using AzureIndexer.Api.Notifications;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using NBitcoin;
using NBitcoin.Crypto;
using Stratis.Bitcoin.AsyncWork;
using Stratis.Bitcoin.P2P.Protocol.Payloads;
using Stratis.Features.AzureIndexer;

namespace AzureIndexer.Api.Infrastructure
{
    public class QBitNinjaConfiguration
    {
        public QBitNinjaConfiguration(IConfiguration configuration, ILoggerFactory loggerFactory, IAsyncProvider asyncProvider)
        {
            this.CoinbaseMaturity = 100;
            this.Indexer = new IndexerConfiguration(configuration, loggerFactory, asyncProvider);
            this.LocalChain = configuration["LocalChain"];
            this.ServiceBus = configuration["ServiceBus"];
        }

        public IndexerConfiguration Indexer
        {
            get;
            set;
        }

        public string LocalChain
        {
            get;
            set;
        }

        public void EnsureSetup()
        {
            var tasks = new[]
            {
                this.GetCallbackTable(),
                this.GetChainCacheCloudTable(),
                this.GetCrudTable(),
                this.GetRejectTable().Table,
                this.GetSubscriptionsTable().Table
            }.Select(t => t.CreateIfNotExistsAsync()).OfType<Task>().ToList();

            tasks.Add(this.Indexer.EnsureSetupAsync());

            // tasks.Add(Topics.EnsureSetupAsync());
            Task.WaitAll(tasks.ToArray());
        }

        public CrudTable<Subscription> GetSubscriptionsTable()
        {
            return GetCrudTableFactory().GetTable<Subscription>("subscriptions");
        }

        public CrudTable<RejectPayload> GetRejectTable()
        {
            return GetCrudTableFactory().GetTable<RejectPayload>("rejectedbroadcasted");
        }

        public CloudTable GetCallbackTable()
        {
            var table = this.Indexer.GetTable("callbacks");
            table.CreateIfNotExistsAsync().GetAwaiter().GetResult();
            return table;
        }

        private CloudTable GetCrudTable()
        {
            var table = this.Indexer.GetTable("crudtable");
            table.CreateIfNotExistsAsync().GetAwaiter().GetResult();
            return table;
        }

        private CloudTable GetChainCacheCloudTable()
        {
            var table = this.Indexer.GetTable("chain");
            table.CreateIfNotExistsAsync().GetAwaiter().GetResult();
            return table;
        }


        //////TODO: These methods will need to be in a "RapidUserConfiguration" that need to know about the user for data isolation (using CrudTable.Scope)

        public CrudTable<T> GetCacheTable<T>(Scope scope = null)
        {
            return GetCrudTableFactory(scope).GetTable<T>("cache");
        }

        public CrudTableFactory GetCrudTableFactory(Scope scope = null)
        {
            return new CrudTableFactory(GetCrudTable, scope);
        }

        public WalletRepository CreateWalletRepository(Scope scope = null)
        {
            return new WalletRepository(
                    Indexer.CreateIndexerClient(),
                    GetChainCacheTable<BalanceSummary>,
                    GetCrudTableFactory(scope));
        }

        public ChainTable<T> GetChainCacheTable<T>(Scope scope)
        {
            return new ChainTable<T>(GetChainCacheCloudTable())
            {
                Scope = scope
            };
        }

        ///////

        public long CoinbaseMaturity
        {
            get;
            set;
        }

        public string ServiceBus
        {
            get;
            set;
        }
    }
}
