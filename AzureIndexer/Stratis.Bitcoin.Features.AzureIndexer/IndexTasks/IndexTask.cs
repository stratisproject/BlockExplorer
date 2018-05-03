using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

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
        private ILogger _logger = NullLogger.Instance;
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
            Log.Debug("Running indexer");
            ConcurrentDictionary<Task, Task> tasks = new ConcurrentDictionary<Task, Task>();
            try
            {
                SetThrottling();
                Log.Debug($"Find Service Point for {Configuration.TableClient.BaseUri}");
                if (EnsureIsSetup)
                {
                    Log.Debug($"Wait for EnsureSetup to complete");
                    EnsureSetup().Wait();
                }

                Log.Debug($"Set bulk import with partition size {PartitionSize}");
                BulkImport<TIndexed> bulk = new BulkImport<TIndexed>(PartitionSize);
                Log.Debug($"Skip to end: {SkipToEnd}");
                if (!SkipToEnd)
                {
                    try
                    {
                        Log.Debug($"Iterate through blocks");
                        foreach (var block in blockFetcher)
                        {
                            Log.Debug($"Current block {block.Height}");
                            ThrowIfException();
                            Log.Debug($"Check if block save is needed");
                            if (blockFetcher.NeedSave)
                            {
                                Log.Debug($"Check if block save progression is needed");
                                if (SaveProgression)
                                {
                                    Log.Debug($"Enqueue tasks for bulk with partition size {bulk.PartitionSize}");
                                    EnqueueTasks(tasks, bulk, true, scheduler);
                                    Log.Debug($"Save progression");
                                    Save(tasks, blockFetcher, bulk);
                                }
                            }
                            Log.Debug($"Process block {block.Height}");
                            ProcessBlock(block, bulk);
                            Log.Debug($"Check is bulk has full partition");
                            if (bulk.HasFullPartition)
                            {
                                Log.Debug($"Partition has full partition. Enqueue tasks.");
                                EnqueueTasks(tasks, bulk, false, scheduler);
                            }
                        }

                        Log.Debug($"Enqueue tasks.");
                        EnqueueTasks(tasks, bulk, true, scheduler);
                    }
                    catch(OperationCanceledException ex)
                    {
                        Log.Error(ex, $"Index failure.");
                        if (ex.CancellationToken != blockFetcher.CancellationToken)
                            throw;
                    }
                }
                else
                {
                    Log.Debug($"Skip is set to true");
                    blockFetcher.SkipToEnd();
                }

                if (SaveProgression)
                {
                    Log.Debug($"Save progression");
                    Save(tasks, blockFetcher, bulk);
                }

                Log.Debug($"Wait finished tasks");
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
                Log.Debug($"Ensure Is Setup: {_EnsureIsSetup}");
                return _EnsureIsSetup;
            }
            set
            {
                _EnsureIsSetup = value;
            }
        }

        private void SetThrottling()
        {
            Log.Debug("Set Throttling");
            Helper.SetThrottling();
            Log.Debug($"Find Service Point for {Configuration.TableClient.BaseUri}");
            ServicePoint tableServicePoint = ServicePointManager.FindServicePoint(Configuration.TableClient.BaseUri);
            tableServicePoint.ConnectionLimit = 1000;
        }
        ExponentialBackoff retry = new ExponentialBackoff(15, TimeSpan.FromMilliseconds(100),
                                                              TimeSpan.FromSeconds(10),
                                                              TimeSpan.FromMilliseconds(200));
        private void EnqueueTasks(ConcurrentDictionary<Task, Task> tasks, BulkImport<TIndexed> bulk, bool uncompletePartitions, TaskScheduler scheduler)
        {
            Log.Debug($"UncompletePartitions ({uncompletePartitions}), bulk.HasFullPartition ({bulk.HasFullPartition})");
            if (!uncompletePartitions && !bulk.HasFullPartition)
                return;
            if(uncompletePartitions)
                bulk.FlushUncompletePartitions();

            Log.Debug($"Bulk ready partitions coun is {bulk._ReadyPartitions.Count}");
            while (bulk._ReadyPartitions.Count != 0)
            {
                Log.Debug($"Dequeue ready partitions");
                var item = bulk._ReadyPartitions.Dequeue();
                var task = retry.Do(() =>
                {
                    Log.Debug($"Index Core {item.Item1} {item.Item2.Length}");
                    IndexCore(item.Item1, item.Item2);
                }, scheduler);
                Log.Debug($"Try adding a task to dictionary");
                tasks.TryAdd(task, task);
                task.ContinueWith(prev =>
                {
                    _Exception = prev.Exception ?? _Exception;
                    Log.Debug($"Setting exception to {_Exception}");
                    Log.Debug($"Try removing a task from dictionary");
                    tasks.TryRemove(prev, out prev);
                });
                Log.Debug($"Check if task count ({tasks.Count}) is greater than MaxQueued {MaxQueued}");
                if (tasks.Count > MaxQueued)
                {
                    Log.Debug($"Wait for finish ({tasks.Count}) with queued target of {MaxQueued / 2}");
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
            Log.Debug($"Sleep is tasks.Count ({tasks.Count}) > queuedTarget ({queuedTarget})");
            while(tasks.Count > queuedTarget)
            {
                Thread.Sleep(100);
            }
        }

        private void ThrowIfException()
        {
            if (_Exception != null)
            {
                Log.Error(_Exception, $"Exception is not null. Throwing");
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
