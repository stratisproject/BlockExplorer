namespace Stratis.Bitcoin.Features.AzureIndexer.IndexTasks
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.ExceptionServices;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using NBitcoin;

    public class IndexBlocksTask : IndexTask<BlockInfo>
    {
        private readonly ILogger logger;

        public IndexBlocksTask(IndexerConfiguration configuration, ILoggerFactory loggerFactory)
            : base(configuration, loggerFactory)
        {
            logger = loggerFactory.CreateLogger(GetType().FullName);
        }

        protected override int PartitionSize => 1;

        volatile int _IndexedBlocks;

        public int IndexedBlocks => _IndexedBlocks;


        public void Index(Block[] blocks, TaskScheduler taskScheduler)
        {
            logger.LogTrace("()");

            if (taskScheduler == null)
            {
                throw new ArgumentNullException("taskScheduler");
            }

            try
            {
                logger.LogTrace("Indexing...");
                IndexAsync(blocks, taskScheduler).Wait();
                logger.LogTrace("Indexing finished");
            }
            catch (AggregateException aex)
            {
                ExceptionDispatchInfo.Capture(aex.InnerException).Throw();

                logger.LogTrace("Exception occurred: {0}", aex.ToString());

                logger.LogTrace("(-):EXEPTION");
                throw;
            }

            logger.LogTrace("(-)");
        }

        public Task IndexAsync(Block[] blocks, TaskScheduler taskScheduler)
        {
            logger.LogTrace("()");

            if (taskScheduler == null)
            {
                throw new ArgumentNullException("taskScheduler");
            }

            Task[] tasks = blocks.Select(b =>
                    new Task(() => IndexCore("o", new[] {new BlockInfo() { Block = b, BlockId = b.GetHash() } })))
                .ToArray();

            logger.LogTrace("Tasks created");

            foreach (Task t in tasks)
            {
                t.Start(taskScheduler);
            }

            logger.LogTrace("(-)");
            return Task.WhenAll(tasks);
        }

        protected async override Task EnsureSetup()
        {
            await Configuration.GetBlocksContainer().CreateIfNotExistsAsync().ConfigureAwait(false);
        }

        protected override void ProcessBlock(BlockInfo block, BulkImport<BlockInfo> bulk, Network network, BulkImport<SmartContactEntry.Entity> smartContractBulk = null)
        {
            bulk.Add("o", block);
        }

        protected override void IndexCore(string partitionName, IEnumerable<BlockInfo> blocks)
        {
            logger.LogTrace("()");

            BlockInfo first = blocks.First();
            Block block = first.Block;
            var hash = first.BlockId.ToString();

            Stopwatch watch = new Stopwatch();
            watch.Start();
            while (true)
            {
                logger.LogTrace("Iteration start");

                CloudBlobContainer container = Configuration.GetBlocksContainer();
                CloudBlobClient client = container.ServiceClient;
                client.DefaultRequestOptions.SingleBlobUploadThresholdInBytes = 32 * 1024 * 1024;
                CloudPageBlob blob = container.GetPageBlobReference(hash);
                MemoryStream ms = new MemoryStream();
                block.ReadWrite(ms, true);
                byte[] blockBytes = ms.GetBuffer();

                long length = 512 - (ms.Length % 512);
                if (length == 512)
                {
                    length = 0;
                }

                Array.Resize(ref blockBytes, (int)(ms.Length + length));

                try
                {
                    blob.UploadFromByteArrayAsync(blockBytes, 0, blockBytes.Length, new AccessCondition()
                            {
                                // Will throw if already exist, save 1 call.
                                IfNotModifiedSinceTime = DateTimeOffset.MinValue
                            },
                            new BlobRequestOptions()
                            {
                                MaximumExecutionTime = _Timeout,
                                ServerTimeout = _Timeout
                            },
                            new OperationContext())
                        .GetAwaiter()
                        .GetResult();
                    watch.Stop();
                    IndexerTrace.BlockUploaded(watch.Elapsed, blockBytes.Length);
                    _IndexedBlocks++;

                    logger.LogTrace("Indexed");
                    break;
                }
                catch (StorageException ex)
                {
                    var alreadyExist = ex.RequestInformation != null && ex.RequestInformation.HttpStatusCode == 412;
                    if (!alreadyExist)
                    {
                        IndexerTrace.ErrorWhileImportingBlockToAzure(uint256.Parse(hash), ex);
                        throw;
                    }

                    watch.Stop();
                    IndexerTrace.BlockAlreadyUploaded();
                    _IndexedBlocks++;

                    logger.LogTrace("Storage exception occurred: {0}", ex.ToString());
                    break;
                }
                catch (Exception ex)
                {
                    IndexerTrace.ErrorWhileImportingBlockToAzure(uint256.Parse(hash), ex);

                    logger.LogTrace("Exception occurred: {0}", ex.ToString());
                    throw;
                }
            }

            logger.LogTrace("(-)");
        }
    }
}
