namespace Stratis.Bitcoin.Features.AzureIndexer.IndexTasks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.ExceptionServices;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Microsoft.WindowsAzure.Storage.Table;
    using NBitcoin;

    public class IndexTableEntitiesTask : IndexTableEntitiesTaskBase<ITableEntity>
    {
        CloudTable _Table;

        public IndexTableEntitiesTask(IndexerConfiguration conf, CloudTable table, ILoggerFactory loggerFactory)
            : base(conf, loggerFactory)
        {
            _Table = table;
        }

        protected override CloudTable GetCloudTable()
        {
            return _Table;
        }

        protected override ITableEntity ToTableEntity(ITableEntity item)
        {
            return item;
        }

        protected override void ProcessBlock(BlockInfo block, BulkImport<ITableEntity> bulk, Network network, BulkImport<SmartContactEntry.Entity> smartContractBulk =null)
        {
            throw new NotSupportedException();
        }

        public void Index(IEnumerable<ITableEntity> entities, TaskScheduler taskScheduler)
        {
            try
            {
                IndexAsync(entities, taskScheduler).Wait();
            }
            catch (AggregateException aex)
            {
                ExceptionDispatchInfo.Capture(aex.InnerException).Throw();
                throw;
            }
        }

        public Task IndexAsync(IEnumerable<ITableEntity> entities, TaskScheduler taskScheduler)
        {
            taskScheduler = taskScheduler ?? TaskScheduler.Default;
            Task[] tasks = entities
                .GroupBy(e => e.PartitionKey)
                .SelectMany(group => group
                                    .Partition(PartitionSize)
                                    .Select(batch => new Task(() => IndexCore(group.Key, batch))))
                .ToArray();

            foreach (Task t in tasks)
            {
                t.Start(taskScheduler);
            }

            return Task.WhenAll(tasks);
        }
    }
}
