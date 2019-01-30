using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using NBitcoin;
// ReSharper disable All

namespace Stratis.Bitcoin.Features.AzureIndexer.IndexTasks
{
    public abstract class IndexTask<TIndexed> : IIndexTask
    {
        private readonly ILogger logger;

        public IndexTask(IndexerConfiguration configuration, ILoggerFactory loggerFactory)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            this.Configuration = configuration;
            this.SaveProgression = true;
            this.MaxQueued = 100;
            this.logger = loggerFactory.CreateLogger(this.GetType().FullName);
        }

        /// <summary>
        /// Fast forward indexing to the end (if scanning not useful)
        /// </summary>
        protected virtual bool SkipToEnd
        {
            get
            {
                return false;
            }
        }

        public void Index(BlockFetcher blockFetcher, TaskScheduler scheduler, Network network)
        {
            this.logger.LogTrace("()");

            ConcurrentDictionary<Task, Task> tasks = new ConcurrentDictionary<Task, Task>();
            try
            {
                this.SetThrottling();
                if (this.EnsureIsSetup)
                {
                    this.EnsureSetup().Wait();
                }

                var bulk = new BulkImport<TIndexed>(this.PartitionSize);
                var scBulk = new BulkImport<SmartContactEntry.Entity>(1);
                if (!this.SkipToEnd)
                {
                    try
                    {

                        foreach (var block in blockFetcher)
                        {
                            this.ThrowIfException();
                            if (blockFetcher.NeedSave)
                            {
                                if (this.SaveProgression)
                                {
                                    this.EnqueueTasks(tasks, bulk, true, scheduler);
                                    this.Save(tasks, blockFetcher, bulk);
                                }
                            }

                            if (typeof(TIndexed) == typeof(TransactionEntry.Entity))
                            {
                                this.ProcessBlock(block, bulk, network, scBulk);
                            }
                            else
                            {
                                this.ProcessBlock(block, bulk, network);
                            }

                            if (bulk.HasFullPartition)
                            {
                                this.EnqueueTasks(tasks, bulk, false, scheduler);
                            }
                        }

                        if (typeof(TIndexed) == typeof(TransactionEntry.Entity))
                        {
                            this.EnqueueTasks(tasks, bulk, true, scheduler, scBulk);
                            //this.EnqueueTasks(tasks, scBulk, true, scheduler);
                        }
                        else
                        {
                            this.EnqueueTasks(tasks, bulk, true, scheduler);
                        }
                    }
                    catch (OperationCanceledException ex)
                    {
                        if (ex.CancellationToken != blockFetcher.CancellationToken)
                        {
                            throw;
                        }
                    }
                }
                else
                {
                    this.logger.LogTrace("Skipping to end");
                    blockFetcher.SkipToEnd();
                }

                if (this.SaveProgression)
                {
                    this.Save(tasks, blockFetcher, bulk);
                }

                this.WaitFinished(tasks);
                this.ThrowIfException();
            }
            catch (AggregateException aex)
            {
                ExceptionDispatchInfo.Capture(aex.InnerException).Throw();
                throw;
            }

            this.logger.LogTrace("(-)");
        }

        bool _ensureIsSetup = true;

        public bool EnsureIsSetup
        {
            get
            {
                return this._ensureIsSetup;
            }

            set
            {
                this._ensureIsSetup = value;
            }
        }

        private ExponentialBackoff retry = new ExponentialBackoff(15, TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(10), TimeSpan.FromMilliseconds(200));

        private void SetThrottling()
        {
            this.logger.LogTrace("()");

            Helper.SetThrottling();
            ServicePoint tableServicePoint = ServicePointManager.FindServicePoint(this.Configuration.TableClient.BaseUri);
            tableServicePoint.ConnectionLimit = 1000;

            this.logger.LogTrace("(-)");
        }

        private void EnqueueTasks(ConcurrentDictionary<Task, Task> tasks, BulkImport<TIndexed> bulk, bool uncompletePartitions, TaskScheduler scheduler, BulkImport<SmartContactEntry.Entity> scBulk = null)
        {
            Task task;
            this.logger.LogTrace("()");
            var IsSmartContract = false;
            if (scBulk != null && !scBulk.IsEmpty)
            {
                IsSmartContract = true;
            }

            if (!uncompletePartitions && !bulk.HasFullPartition)
            {
                this.logger.LogTrace("(-):PARTITIONS {0}:{1},{2}:{3}", nameof(uncompletePartitions), uncompletePartitions, nameof(bulk.HasFullPartition), bulk.HasFullPartition);
                return;
            }

            if (uncompletePartitions)
            {
                bulk.FlushUncompletePartitions();
            }

            while (bulk._ReadyPartitions.Count != 0)
            {
                Tuple<string, TIndexed[]> item = bulk._ReadyPartitions.Dequeue();
                if (IsSmartContract && scBulk.HasFullPartition)
                {
                    var scItem = scBulk._ReadyPartitions.Dequeue();
                    task = this.retry.Do(() => this.IndexCore(item.Item1, item.Item2), scheduler);
                }
                else
                {
                    task = this.retry.Do(() => this.IndexCore(item.Item1, item.Item2), scheduler);
                }

                tasks.TryAdd(task, task);
                task.ContinueWith(prev =>
                {
                    this._Exception = prev.Exception ?? this._Exception;
                    tasks.TryRemove(prev, out prev);
                });

                if (tasks.Count > this.MaxQueued)
                {
                    this.WaitFinished(tasks, this.MaxQueued / 2);
                }
            }

            this.logger.LogTrace("(-)");
        }


        public int MaxQueued { get; set; }

        Exception _Exception;

        private void Save(ConcurrentDictionary<Task, Task> tasks, BlockFetcher fetcher, BulkImport<TIndexed> bulk)
        {
            this.logger.LogTrace("()");

            this.WaitFinished(tasks);
            this.ThrowIfException();
            fetcher.SaveCheckpoint();

            this.logger.LogTrace("(-)");
        }

        int[] wait = new int[] { 100, 200, 400, 800, 1600 };

        private void WaitFinished(ConcurrentDictionary<Task, Task> tasks, int queuedTarget = 0)
        {
            this.logger.LogTrace("()");

            while (tasks.Count > queuedTarget)
            {
                Thread.Sleep(100);
            }

            this.logger.LogTrace("(-)");
        }

        private void ThrowIfException()
        {
            this.logger.LogTrace("()");

            if (this._Exception != null)
            {
                ExceptionDispatchInfo.Capture(this._Exception).Throw();
            }

            this.logger.LogTrace("(-)");
        }

        protected TimeSpan _Timeout = TimeSpan.FromMinutes(5.0);

        public IndexerConfiguration Configuration
        {
            get;
            private set;
        }

        public bool SaveProgression
        {
            get;
            set;
        }

        protected abstract int PartitionSize
        {
            get;
        }

        protected abstract Task EnsureSetup();

        protected abstract void ProcessBlock(BlockInfo block, BulkImport<TIndexed> bulkImport, Network network, BulkImport<SmartContactEntry.Entity> SmartContractBulk = null);

        protected abstract void IndexCore(string partitionName, IEnumerable<TIndexed> items);

        //protected abstract void IndexCore(string partitionName, IEnumerable<TIndexed> items);
    }
}
