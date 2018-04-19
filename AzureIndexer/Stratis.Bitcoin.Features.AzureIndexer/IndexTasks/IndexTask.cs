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

namespace Stratis.Bitcoin.Features.AzureIndexer.IndexTasks
{
    public interface IIndexTask
    {
        void Index(BlockFetcher blockFetcher, TaskScheduler scheduler, ILogger logger);
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

        public void Index(BlockFetcher blockFetcher, TaskScheduler scheduler, ILogger logger)
        {
            _logger = logger;
            logger.LogTrace("Running indexer");
            ConcurrentDictionary<Task, Task> tasks = new ConcurrentDictionary<Task, Task>();
            try
            {
                SetThrottling();
                logger.LogTrace($"Find Service Point for {Configuration.TableClient.BaseUri}");
                if (EnsureIsSetup)
                {
                    logger.LogTrace($"Wait for EnsureSetup to complete");
                    EnsureSetup().Wait();
                }

                logger.LogTrace($"Set bulk import with partition size {PartitionSize}");
                BulkImport<TIndexed> bulk = new BulkImport<TIndexed>(PartitionSize);
                logger.LogTrace($"Skip to end: {SkipToEnd}");
                if (!SkipToEnd)
                {
                    try
                    {
                        logger.LogTrace($"Iterate through blocks");
                        foreach (var block in blockFetcher)
                        {
                            logger.LogTrace($"Current block {block.Height}");
                            ThrowIfException();
                            logger.LogTrace($"Check if block save is needed");
                            if (blockFetcher.NeedSave)
                            {
                                logger.LogTrace($"Check if block save progression is needed");
                                if (SaveProgression)
                                {
                                    logger.LogTrace($"Enqueue tasks for bulk with partition size {bulk.PartitionSize}");
                                    EnqueueTasks(tasks, bulk, true, scheduler);
                                    logger.LogTrace($"Save progression");
                                    Save(tasks, blockFetcher, bulk);
                                }
                            }
                            logger.LogTrace($"Process block {block.Height}");
                            ProcessBlock(block, bulk);
                            logger.LogTrace($"Check is bulk has full partition");
                            if (bulk.HasFullPartition)
                            {
                                logger.LogTrace($"Partition has full partition. Enqueue tasks.");
                                EnqueueTasks(tasks, bulk, false, scheduler);
                            }
                        }

                        logger.LogTrace($"Enqueue tasks.");
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
                    logger.LogTrace($"Skip is set to true");
                    blockFetcher.SkipToEnd();
                }

                if (SaveProgression)
                {
                    logger.LogTrace($"Save progression");
                    Save(tasks, blockFetcher, bulk);
                }

                logger.LogTrace($"Wait finished tasks");
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
                _logger.LogTrace($"Ensure Is Setup: {_EnsureIsSetup}");
                return _EnsureIsSetup;
            }
            set
            {
                _EnsureIsSetup = value;
            }
        }

        private void SetThrottling()
        {
            _logger.LogTrace("Set Throttling");
            Helper.SetThrottling();
            _logger.LogTrace($"Find Service Point for {Configuration.TableClient.BaseUri}");
            ServicePoint tableServicePoint = ServicePointManager.FindServicePoint(Configuration.TableClient.BaseUri);
            tableServicePoint.ConnectionLimit = 1000;
        }
        ExponentialBackoff retry = new ExponentialBackoff(15, TimeSpan.FromMilliseconds(100),
                                                              TimeSpan.FromSeconds(10),
                                                              TimeSpan.FromMilliseconds(200));
        private void EnqueueTasks(ConcurrentDictionary<Task, Task> tasks, BulkImport<TIndexed> bulk, bool uncompletePartitions, TaskScheduler scheduler)
        {
            _logger.LogTrace($"UncompletePartitions ({uncompletePartitions}), bulk.HasFullPartition ({bulk.HasFullPartition})");
            if (!uncompletePartitions && !bulk.HasFullPartition)
                return;
            if(uncompletePartitions)
                bulk.FlushUncompletePartitions();

            _logger.LogTrace($"Bulk ready partitions coun is {bulk._ReadyPartitions.Count}");
            while (bulk._ReadyPartitions.Count != 0)
            {
                _logger.LogTrace($"Dequeue ready partitions");
                var item = bulk._ReadyPartitions.Dequeue();
                var task = retry.Do(() =>
                {
                    _logger.LogTrace($"Index Core {item.Item1} {item.Item2.Length}");
                    IndexCore(item.Item1, item.Item2);
                }, scheduler);
                _logger.LogTrace($"Try adding a task to dictionary");
                tasks.TryAdd(task, task);
                task.ContinueWith(prev =>
                {
                    _Exception = prev.Exception ?? _Exception;
                    _logger.LogTrace($"Setting exception to {_Exception}");
                    _logger.LogTrace($"Try removing a task from dictionary");
                    tasks.TryRemove(prev, out prev);
                });
                _logger.LogTrace($"Check if task count ({tasks.Count}) is greater than MaxQueued {MaxQueued}");
                if (tasks.Count > MaxQueued)
                {
                    _logger.LogTrace($"Wait for finish ({tasks.Count}) with queued target of {MaxQueued / 2}");
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
            _logger.LogTrace($"Sleep is tasks.Count ({tasks.Count}) > queuedTarget ({queuedTarget})");
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
