namespace Stratis.Features.AzureIndexer.IndexTasks
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.ExceptionServices;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using NBitcoin;
    using Stratis.Features.AzureIndexer.Entities;

    public class IndexBlocksTask : IndexTask<BlockInfo>
    {
        private readonly ILogger logger;

        private volatile int indexedBlocks;

        public IndexBlocksTask(IndexerConfiguration configuration, ILoggerFactory loggerFactory)
            : base(configuration, loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger(this.GetType().FullName);
        }

        public int IndexedBlocks => this.indexedBlocks;

        protected override int PartitionSize => 1;

        public void Index(Block[] blocks, TaskScheduler taskScheduler)
        {
            if (taskScheduler == null)
            {
                throw new ArgumentNullException("taskScheduler");
            }

            try
            {
                this.IndexAsync(blocks, taskScheduler).Wait();
            }
            catch (AggregateException aex)
            {
                this.logger.LogError(aex, "Exception occurred: {0}", aex.ToString());
                ExceptionDispatchInfo.Capture(aex.InnerException).Throw();
            }
        }

        public Task IndexAsync(Block[] blocks, TaskScheduler taskScheduler)
        {
            if (taskScheduler == null)
            {
                throw new ArgumentNullException("taskScheduler");
            }

            Task[] tasks = blocks.Select(b =>
                    new Task(() => this.IndexCore("o", new[] { new BlockInfo() { Block = b, BlockId = b.GetHash() } })))
                .ToArray();

            foreach (Task t in tasks)
            {
                t.Start(taskScheduler);
            }

            return Task.WhenAll(tasks);
        }

        protected override async Task EnsureSetup()
        {
            await this.Configuration.GetBlocksContainer().CreateIfNotExistsAsync().ConfigureAwait(false);
        }

        protected override void ProcessBlock(BlockInfo block, BulkImport<BlockInfo> bulk, Network network, BulkImport<SmartContactEntry.Entity> smartContractBulk = null)
        {
            bulk.Add("o", block);
        }

        protected override void IndexCore(string partitionName, IEnumerable<BlockInfo> blocks)
        {
            var from = blocks.FirstOrDefault()?.Height;
            var to = blocks.LastOrDefault()?.Height;

            this.logger.LogDebug("Indexing blocks from {0} to {1}", from, to);

            BlockInfo first = blocks.First();
            Block block = first.Block;
            var hash = first.BlockId.ToString();

            //Stopwatch watch = new Stopwatch();
            //watch.Start();
            while (true)
            {
                CloudBlobContainer container = this.Configuration.GetBlocksContainer();
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
                    blob.UploadFromByteArrayAsync(blockBytes, 0, blockBytes.Length,
                            new AccessCondition()
                            {
                                // TODO Review that param.
                                IfNotModifiedSinceTime = DateTimeOffset.MinValue
                            },
                            new BlobRequestOptions() { MaximumExecutionTime = this.Timeout, ServerTimeout = this.Timeout },
                            new OperationContext())
                        .GetAwaiter()
                        .GetResult();
                    //watch.Stop();

                    //IndexerTrace.BlockUploaded(watch.Elapsed, blockBytes.Length);
                    this.indexedBlocks++;

                    this.logger.LogDebug("Completed Indexing blocks from {0} to {1}", from, to);

                    break;
                }
                catch (StorageException ex)
                {
                    var alreadyExist = ex.RequestInformation != null && ex.RequestInformation.HttpStatusCode == 412;
                    if (!alreadyExist)
                    {
                        IndexerTrace.ErrorWhileImportingBlockToAzure(uint256.Parse(hash), ex);
                    }

                    //watch.Stop();
                    // IndexerTrace.BlockAlreadyUploaded();
                    this.indexedBlocks++;

                    this.logger.LogError(ex, "Storage exception occurred: {0}", ex.ToString());
                    break;
                }
                catch (Exception ex)
                {
                    IndexerTrace.ErrorWhileImportingBlockToAzure(uint256.Parse(hash), ex);

                    this.logger.LogError(ex, "Exception occurred: {0}", ex.ToString());
                    throw;
                }
            }
        }
    }
}
