using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using NBitcoin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Stratis.Bitcoin.Features.AzureIndexer
{
    public class Checkpoint
    {
        private readonly string _CheckpointName;
        public string CheckpointName
        {
            get
            {
                return _CheckpointName;
            }
        }

        CloudBlockBlob _Blob;

        private readonly ILoggerFactory loggerFactory;
        private readonly ILogger logger;

        public Checkpoint(string checkpointName, Network network, Stream data, CloudBlockBlob blob, ILoggerFactory loggerFactory)
        {
            this.loggerFactory = loggerFactory;
            this.logger = loggerFactory.CreateLogger(GetType().FullName);

            if (checkpointName == null)
                throw new ArgumentNullException("checkpointName");
            _Blob = blob;
            _CheckpointName = checkpointName;
            _BlockLocator = new BlockLocator();
            if (data != null)
            {
                try
                {
                    _BlockLocator.ReadWrite(data, false);
                    return;
                }
                catch
                {
                }
            }
            var list = new List<uint256>();
            list.Add(network.GetGenesis().Header.GetHash());
            _BlockLocator = new BlockLocator();
            _BlockLocator.Blocks.AddRange(list);
        }

        public uint256 Genesis
        {
            get
            {
                return BlockLocator.Blocks[BlockLocator.Blocks.Count - 1];
            }
        }

        BlockLocator _BlockLocator;
        public BlockLocator BlockLocator
        {
            get
            {
                return _BlockLocator;
            }
        }

        public bool SaveProgress(ChainedHeader tip)
        {
            this.logger.LogTrace("()");

            bool progress = SaveProgress(tip.GetLocator());

            this.logger.LogTrace("(-):{0}", progress);
            return progress;
        }
        public bool SaveProgress(BlockLocator locator)
        {
            this.logger.LogTrace("()");

            _BlockLocator = locator;
            try
            {
                Task<bool> savingTask = Task.Run(new Func<Task<bool>>(async () =>
                {
                    this.logger.LogTrace("()");

                    var saving = SaveProgressAsync();
                    var timeout = Task.Delay(50000);

                    await Task.WhenAny(saving, timeout).ConfigureAwait(false);

                    if (saving.IsCompleted)
                    {
                        this.logger.LogTrace("Saving completed.");
                        this.logger.LogTrace("(-):{0}", saving.Result);
                        return saving.Result;
                    }

                    this.logger.LogTrace("(-):TIMEOUT");
                    return false;
                }));
                
                bool result = savingTask.GetAwaiter().GetResult();

                this.logger.LogTrace("(-):{0}", result);
                return result;
            }
            catch (Exception aex)
            {
                this.logger.LogError("Exception occured: {0}", aex.ToString());
                ExceptionDispatchInfo.Capture(aex).Throw();

                this.logger.LogTrace("(-):false");
                return false;
            }
        }

        public async Task DeleteAsync()
        {
            try
            {
                await _Blob.DeleteAsync().ConfigureAwait(false);
            }
            catch (StorageException ex)
            {
                if (ex.RequestInformation != null && ex.RequestInformation.HttpStatusCode == 404)
                    return;
                throw;
            }
        }

        private async Task<bool> SaveProgressAsync()
        {
            this.logger.LogTrace("()");

            var bytes = BlockLocator.ToBytes();
            try
            {
                this.logger.LogTrace("Uploading block locator bytes");
                await _Blob.UploadFromByteArrayAsync(bytes, 0, bytes.Length, new AccessCondition()
                {
                    IfMatchETag = _Blob.Properties.ETag
                }, null, null).ConfigureAwait(false);
            }
            catch (StorageException ex)
            {
                if (ex.RequestInformation != null && ex.RequestInformation.HttpStatusCode == 412)
                {
                    this.logger.LogTrace("(-)[STORAGE_EXCEPTION_412]:false");
                    return false;
                }
                
                this.logger.LogError("Storage exception occured: {0}", ex.ToString());

                this.logger.LogTrace("(-)[STORAGEEX]");
                throw;
            }
            catch (Exception ex)
            {
                this.logger.LogError("Exception occured: {0}", ex.ToString());
                this.logger.LogTrace("(-)[EX]");
                throw;
            }

            this.logger.LogTrace("(-):true");
            return true;
        }

        public static async Task<Checkpoint> LoadBlobAsync(CloudBlockBlob blob, Network network, ILoggerFactory loggerFactory)
        {
            var checkpointName = string.Join("/", blob.Name.Split('/').Skip(1).ToArray());
            MemoryStream ms = new MemoryStream();
            try
            {
                await blob.DownloadToStreamAsync(ms).ConfigureAwait(false);
                ms.Position = 0;
            }
            catch (StorageException ex)
            {
                if (ex.RequestInformation == null || ex.RequestInformation.HttpStatusCode != 404)
                    throw;
            }
            var checkpoint = new Checkpoint(checkpointName, network, ms, blob, loggerFactory);
            return checkpoint;
        }

        public override string ToString()
        {
            return CheckpointName;
        }
    }

}
