using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NBitcoin;

namespace Stratis.Bitcoin.Features.AzureIndexer.IndexTasks
{
    public interface IIndexTask
    {
        void Index(BlockFetcher blockFetcher, TaskScheduler scheduler, Network network);
        bool SaveProgression
        {
            get;
            set;
        }
        bool EnsureIsSetup
        {
            get;
            set;
        }
    }

    public abstract class IndexTask<TIndexed> : IIndexTask
    {
        private readonly ILogger logger;

        public IndexTask(IndexerConfiguration configuration, ILoggerFactory loggerFactory)
        {
            if (configuration == null)
                throw new ArgumentNullException("configuration");
            this.Configuration = configuration;
            SaveProgression = true;
            MaxQueued = 100;
            this.logger = loggerFactory.CreateLogger(GetType().FullName);
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
                SetThrottling();
                if(EnsureIsSetup)
                    EnsureSetup().Wait();

                BulkImport<TIndexed> bulk = new BulkImport<TIndexed>(PartitionSize);
                if(!SkipToEnd)
                {
                    try
                    {

                        foreach(var block in blockFetcher)
                        {
                            ThrowIfException();
                            if(blockFetcher.NeedSave)
                            {
                                if(SaveProgression)
                                {
                                    EnqueueTasks(tasks, bulk, true, scheduler);
                                    Save(tasks, blockFetcher, bulk);
                                }
                            }
                            ProcessBlock(block, bulk, network);
                            if(bulk.HasFullPartition)
                            {
                                EnqueueTasks(tasks, bulk, false, scheduler);
                            }
                        }
                        EnqueueTasks(tasks, bulk, true, scheduler);
                    }
                    catch(OperationCanceledException ex)
                    {
                        if(ex.CancellationToken != blockFetcher.CancellationToken)
                            throw;
                    }
                }
                else
                {
                    this.logger.LogTrace("Skipping to end");
                    blockFetcher.SkipToEnd();
                }

                if(SaveProgression)
                    Save(tasks, blockFetcher, bulk);
                WaitFinished(tasks);
                ThrowIfException();
            }
            catch(AggregateException aex)
            {
                ExceptionDispatchInfo.Capture(aex.InnerException).Throw();
                throw;
            }

            this.logger.LogTrace("(-)");
        }

        bool _EnsureIsSetup = true;
        public bool EnsureIsSetup
        {
            get
            {
                return _EnsureIsSetup;
            }
            set
            {
                _EnsureIsSetup = value;
            }
        }

        private void SetThrottling()
        {
            this.logger.LogTrace("()");

            Helper.SetThrottling();
            ServicePoint tableServicePoint = ServicePointManager.FindServicePoint(Configuration.TableClient.BaseUri);
            tableServicePoint.ConnectionLimit = 1000;

            this.logger.LogTrace("(-)");
        }
        ExponentialBackoff retry = new ExponentialBackoff(15, TimeSpan.FromMilliseconds(100),
                                                              TimeSpan.FromSeconds(10),
                                                              TimeSpan.FromMilliseconds(200));
        private void EnqueueTasks(ConcurrentDictionary<Task, Task> tasks, BulkImport<TIndexed> bulk, bool uncompletePartitions, TaskScheduler scheduler)
        {
            this.logger.LogTrace("()");

            if (!uncompletePartitions && !bulk.HasFullPartition)
            {
                this.logger.LogTrace("(-):PARTITIONS {0}:{1},{2}:{3}", nameof(uncompletePartitions), uncompletePartitions, nameof(bulk.HasFullPartition), bulk.HasFullPartition);
                return;
            }

            if(uncompletePartitions)
                bulk.FlushUncompletePartitions();

            while(bulk._ReadyPartitions.Count != 0)
            {
                var item = bulk._ReadyPartitions.Dequeue();
                var task = retry.Do(() => IndexCore(item.Item1, item.Item2), scheduler);
                tasks.TryAdd(task, task);
                task.ContinueWith(prev =>
                {
                    _Exception = prev.Exception ?? _Exception;
                    tasks.TryRemove(prev, out prev);
                });

                if(tasks.Count > MaxQueued)
                {
                    WaitFinished(tasks, MaxQueued / 2);
                }
            }

            this.logger.LogTrace("(-)");
        }

        public int MaxQueued
        {
            get;
            set;
        }

        Exception _Exception;

        private void Save(ConcurrentDictionary<Task, Task> tasks, BlockFetcher fetcher, BulkImport<TIndexed> bulk)
        {
            this.logger.LogTrace("()");

            WaitFinished(tasks);
            ThrowIfException();
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

            if (_Exception != null)
                ExceptionDispatchInfo.Capture(_Exception).Throw();

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
        protected abstract void ProcessBlock(BlockInfo block, BulkImport<TIndexed> bulk, Network network);
        protected abstract void IndexCore(string partitionName, IEnumerable<TIndexed> items);
    }
}
