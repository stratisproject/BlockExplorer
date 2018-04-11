using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Stratis.Bitcoin.Features.AzureIndexer.IndexTasks
{
    public interface IIndexTask
    {
        void Index(BlockFetcher blockFetcher, TaskScheduler scheduler);
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

        public void Index(BlockFetcher blockFetcher, TaskScheduler scheduler)
        {
            IndexerTrace.Trace("Running indexer");
            ConcurrentDictionary<Task, Task> tasks = new ConcurrentDictionary<Task, Task>();
            try
            {
                SetThrottling();
                IndexerTrace.Trace($"Find Service Point for {Configuration.TableClient.BaseUri}");
                if (EnsureIsSetup)
                {
                    IndexerTrace.Trace($"Wait for EnsureSetup to complete");
                    EnsureSetup().Wait();
                }

                IndexerTrace.Trace($"Set bulk import with partition size {PartitionSize}");
                BulkImport<TIndexed> bulk = new BulkImport<TIndexed>(PartitionSize);
                IndexerTrace.Trace($"Skip to end: {SkipToEnd}");
                if (!SkipToEnd)
                {
                    try
                    {
                        IndexerTrace.Trace($"Iterate through blocks");
                        foreach (var block in blockFetcher)
                        {
                            IndexerTrace.Trace($"Current block {block.Height}");
                            ThrowIfException();
                            IndexerTrace.Trace($"Check if block save is needed");
                            if (blockFetcher.NeedSave)
                            {
                                IndexerTrace.Trace($"Check if block save progression is needed");
                                if (SaveProgression)
                                {
                                    IndexerTrace.Trace($"Enqueue tasks for bulk with partition size {bulk.PartitionSize}");
                                    EnqueueTasks(tasks, bulk, true, scheduler);
                                    IndexerTrace.Trace($"Save progression");
                                    Save(tasks, blockFetcher, bulk);
                                }
                            }
                            IndexerTrace.Trace($"Process block {block.Height}");
                            ProcessBlock(block, bulk);
                            IndexerTrace.Trace($"Check is bulk has full partition");
                            if (bulk.HasFullPartition)
                            {
                                IndexerTrace.Trace($"Partition has full partition. Enqueue tasks.");
                                EnqueueTasks(tasks, bulk, false, scheduler);
                            }
                        }

                        IndexerTrace.Trace($"Enqueue tasks.");
                        EnqueueTasks(tasks, bulk, true, scheduler);
                    }
                    catch(OperationCanceledException ex)
                    {
                        IndexerTrace.Error($"Index failure.", ex);
                        if (ex.CancellationToken != blockFetcher.CancellationToken)
                            throw;
                    }
                }
                else
                {
                    IndexerTrace.Trace($"Skip is set to true");
                    blockFetcher.SkipToEnd();
                }

                if (SaveProgression)
                {
                    IndexerTrace.Trace($"Save progression");
                    Save(tasks, blockFetcher, bulk);
                }

                IndexerTrace.Trace($"Wait finished tasks");
                WaitFinished(tasks);
                ThrowIfException();
            }
            catch(AggregateException aex)
            {
                ExceptionDispatchInfo.Capture(aex.InnerException).Throw();
                throw;
            }
        }

        bool _EnsureIsSetup = true;
        public bool EnsureIsSetup
        {
            get
            {
                IndexerTrace.Trace($"Ensure Is Setup: {this.EnsureIsSetup}");
                return _EnsureIsSetup;
            }
            set
            {
                _EnsureIsSetup = value;
            }
        }

        private void SetThrottling()
        {
            IndexerTrace.Trace("Set Throttling");
            Helper.SetThrottling();
            IndexerTrace.Trace($"Find Service Point for {Configuration.TableClient.BaseUri}");
            ServicePoint tableServicePoint = ServicePointManager.FindServicePoint(Configuration.TableClient.BaseUri);
            tableServicePoint.ConnectionLimit = 1000;
        }
        ExponentialBackoff retry = new ExponentialBackoff(15, TimeSpan.FromMilliseconds(100),
                                                              TimeSpan.FromSeconds(10),
                                                              TimeSpan.FromMilliseconds(200));
        private void EnqueueTasks(ConcurrentDictionary<Task, Task> tasks, BulkImport<TIndexed> bulk, bool uncompletePartitions, TaskScheduler scheduler)
        {
            IndexerTrace.Trace($"UncompletePartitions ({uncompletePartitions}), bulk.HasFullPartition ({bulk.HasFullPartition})");
            if (!uncompletePartitions && !bulk.HasFullPartition)
                return;
            if(uncompletePartitions)
                bulk.FlushUncompletePartitions();

            IndexerTrace.Trace($"Bulk ready partitions coun is {bulk._ReadyPartitions.Count}");
            while (bulk._ReadyPartitions.Count != 0)
            {
                IndexerTrace.Trace($"Dequeue ready partitions");
                var item = bulk._ReadyPartitions.Dequeue();
                var task = retry.Do(() =>
                {
                    IndexerTrace.Trace($"Index Core {item.Item1} {item.Item2.Length}");
                    IndexCore(item.Item1, item.Item2);
                }, scheduler);
                IndexerTrace.Trace($"Try adding a task to dictionary");
                tasks.TryAdd(task, task);
                task.ContinueWith(prev =>
                {
                    _Exception = prev.Exception ?? _Exception;
                    IndexerTrace.Trace($"Setting exception to {_Exception}");
                    IndexerTrace.Trace($"Try removing a task from dictionary");
                    tasks.TryRemove(prev, out prev);
                });
                IndexerTrace.Trace($"Check if task count ({tasks.Count}) is greater than MaxQueued {MaxQueued}");
                if (tasks.Count > MaxQueued)
                {
                    IndexerTrace.Trace($"Wait for finish ({tasks.Count}) with queued target of {MaxQueued / 2}");
                    WaitFinished(tasks, MaxQueued / 2);
                }
            }
        }

        public int MaxQueued
        {
            get;
            set;
        }

        Exception _Exception;

        private void Save(ConcurrentDictionary<Task, Task> tasks, BlockFetcher fetcher, BulkImport<TIndexed> bulk)
        {
            WaitFinished(tasks);
            ThrowIfException();
            fetcher.SaveCheckpoint();
        }

        int[] wait = new int[] { 100, 200, 400, 800, 1600 };
        private void WaitFinished(ConcurrentDictionary<Task, Task> tasks, int queuedTarget = 0)
        {
            IndexerTrace.Trace($"Sleep is tasks.Count ({tasks.Count}) > queuedTarget ({queuedTarget})");
            while(tasks.Count > queuedTarget)
            {
                Thread.Sleep(100);
            }
        }

        private void ThrowIfException()
        {
            if (_Exception != null)
            {
                IndexerTrace.Error($"Exception is not null. Throwing", _Exception);
                ExceptionDispatchInfo.Capture(_Exception).Throw();
            }
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
        protected abstract void ProcessBlock(BlockInfo block, BulkImport<TIndexed> bulk);
        protected abstract void IndexCore(string partitionName, IEnumerable<TIndexed> items);

        public IndexTask(IndexerConfiguration configuration)
        {
            if(configuration == null)
                throw new ArgumentNullException("configuration");
            this.Configuration = configuration;
            SaveProgression = true;
            MaxQueued = 100;
        }
    }
}
