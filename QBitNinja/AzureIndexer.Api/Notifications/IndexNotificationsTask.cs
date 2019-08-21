using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureIndexer.Api.Infrastructure;
using AzureIndexer.Api.Models;
using Microsoft.Extensions.Logging;
using NBitcoin;
using Stratis.Bitcoin.Features.AzureIndexer;
using Stratis.Bitcoin.Features.AzureIndexer.Entities;
using Stratis.Bitcoin.Features.AzureIndexer.IndexTasks;

namespace AzureIndexer.Api.Notifications
{
    public class IndexNotificationsTask : IndexTask<Notify>
    {
        private SubscriptionCollection _Subscriptions;
        private QBitNinjaConfiguration _Conf;
        public IndexNotificationsTask(QBitNinjaConfiguration conf, SubscriptionCollection subscriptions, ILoggerFactory loggerFactory)
            : base(conf.Indexer, loggerFactory)
        {
            if(subscriptions == null)
                throw new ArgumentNullException("subscriptions");
            if(conf == null)
                throw new ArgumentNullException("conf");
            _Subscriptions = subscriptions;
            _Conf = conf;
        }
        protected override Task EnsureSetup()
        {
            return Task.FromResult(true);
        }

        protected override void ProcessBlock(BlockInfo block, BulkImport<Notify> bulkImport, Network network, BulkImport<SmartContactEntry.Entity> SmartContractBulk = null)
        {
            var notif = new NewBlockNotificationData()
            {
                Header = block.Block.Header,
                BlockId = block.BlockId,
                Height = block.Height
            };
            foreach (var newBlock in _Subscriptions.GetNewBlocks())
            {
                bulkImport.Add("o", new Notify(new Notification()
                {
                    Subscription = newBlock,
                    Data = notif
                }));
            }
        }

        protected override void IndexCore(string partitionName, IEnumerable<Notify> items)
        {
            _Conf
                .Topics
                .SendNotifications
                .AddAsync(items.First()).Wait();
        }

        protected override int PartitionSize
        {
            get
            {
                return 1;
            }
        }

        protected override bool SkipToEnd
        {
            get
            {
                return _Subscriptions.Count == 0;
            }
        }
    }
}
