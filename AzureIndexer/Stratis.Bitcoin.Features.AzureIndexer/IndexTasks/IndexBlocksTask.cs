using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using NBitcoin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Serilog;

namespace Stratis.Bitcoin.Features.AzureIndexer.IndexTasks
{
    public class IndexBlocksTask : IndexTask<BlockInfo>
    {
        private readonly ILogger logger = Log.ForContext<IndexBlocksTask>();
        public IndexBlocksTask(IndexerConfiguration configuration)
            : base(configuration)
        {
        }
        protected override int PartitionSize
        {
            get
            {
                return 1;
            }
        }

        volatile int _IndexedBlocks;
        public int IndexedBlocks
        {
            get
            {
                return _IndexedBlocks;
            }
        }


        public void Index(Block[] blocks, TaskScheduler taskScheduler)
        {
            if (taskScheduler == null)
                throw new ArgumentNullException("taskScheduler");
            try
            {
                IndexAsync(blocks, taskScheduler).Wait();
            }
            catch (AggregateException aex)
            {
                this.logger.Error(aex, "Index");
                ExceptionDispatchInfo.Capture(aex.InnerException).Throw();
                throw;
            }
        }

        public Task IndexAsync(Block[] blocks, TaskScheduler taskScheduler)
        {
            if (taskScheduler == null)
                throw new ArgumentNullException("taskScheduler");
            var tasks = blocks
                .Select(b => new Task(() => IndexCore("o", new[]{new BlockInfo()
                {
                    Block = b,
                    BlockId = b.GetHash()
                }})))
                .ToArray();
            foreach (var t in tasks)
            {
                this.logger.Debug("IndexAsync: run task {task}", t);
                t.Start(taskScheduler);
            }

            return Task.WhenAll(tasks);
        }

        protected async override Task EnsureSetup()
        {
            await Configuration.GetBlocksContainer().CreateIfNotExistsAsync().ConfigureAwait(false);
        }
        protected override void ProcessBlock(BlockInfo block, BulkImport<BlockInfo> bulk)
        {
            bulk.Add("o", block);
        }

        protected override void IndexCore(string partitionName, IEnumerable<BlockInfo> blocks)
        {
            var first = blocks.First();
            var block = first.Block;
            var hash = first.BlockId.ToString();

            Stopwatch watch = new Stopwatch();
            watch.Start();
            while (true)
            {
                this.logger.Debug("IndexCore loop");
                var container = Configuration.GetBlocksContainer();
                var client = container.ServiceClient;
                client.DefaultRequestOptions.SingleBlobUploadThresholdInBytes = 32 * 1024 * 1024;
                var blob = container.GetPageBlobReference(hash);
                MemoryStream ms = new MemoryStream();
                block.ReadWrite(ms, true);
                var blockBytes = ms.GetBuffer();

                long length = 512 - (ms.Length % 512);
                if (length == 512)
                    length = 0;
                Array.Resize(ref blockBytes, (int)(ms.Length + length));

                try
                {
                    blob.
                        UploadFromByteArrayAsync(blockBytes, 0, blockBytes.Length, new AccessCondition()
                        {
                            //Will throw if already exist, save 1 call
                            IfNotModifiedSinceTime = DateTimeOffset.MinValue
                        }, new BlobRequestOptions()
                        {
                            MaximumExecutionTime = _Timeout,
                            ServerTimeout = _Timeout
                        }
                    , new OperationContext()).GetAwaiter().GetResult();
                    watch.Stop();
                    Log.Logger.BlockUploaded(watch.Elapsed, blockBytes.Length);
                    _IndexedBlocks++;
                    break;
                }
                catch (StorageException ex)
                {
                    this.logger.Error(ex, "Error While Importing Block To Azure");
                    var alreadyExist = ex.RequestInformation != null && ex.RequestInformation.HttpStatusCode == 412;
                    if (!alreadyExist)
                    {
                        Log.Logger.ErrorWhileImportingBlockToAzure(uint256.Parse(hash), ex);
                        throw;
                    }
                    watch.Stop();
                    Log.Logger.BlockAlreadyUploaded();
                    _IndexedBlocks++;
                    break;
                }
                catch (Exception ex)
                {
                    this.logger.Error(ex, "Generic Exception While Importing Block To Azure");
                    Log.Logger.ErrorWhileImportingBlockToAzure(uint256.Parse(hash), ex);
                    throw;
                }
            }
        }
    }
}
