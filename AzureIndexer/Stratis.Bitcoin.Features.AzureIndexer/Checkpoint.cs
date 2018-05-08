using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using NBitcoin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Serilog;

namespace Stratis.Bitcoin.Features.AzureIndexer
{
    public class Checkpoint
    {
        private readonly ILogger logger = Log.ForContext<Checkpoint>();
        private readonly string _CheckpointName;
        public string CheckpointName
        {
            get
            {
                return _CheckpointName;
            }
        }

        CloudBlockBlob _Blob;
        public Checkpoint(string checkpointName, Network network, Stream data, CloudBlockBlob blob)
        {
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
                catch (Exception ex)
                {
                    this.logger.Error(ex, "Checkpoint failure");
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

        public async Task<bool> SaveProgress(ChainedBlock tip)
        {
            Log.Logger.Debug($"Save Progress: {tip.Height}");
            return await SaveProgress(tip.GetLocator());
        }
        public async Task<bool> SaveProgress(BlockLocator locator)
        {
            _BlockLocator = locator;
            try
            {
                return await SaveProgressAsync();
            }
            catch (AggregateException aex)
            {
                Log.Logger.Error($"Save Progress failure", aex);
                this.logger.Error(aex, "SaveProgress failure");
                ExceptionDispatchInfo.Capture(aex.InnerException).Throw();
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
                this.logger.Error(ex, "DeleteAsync failure");
                if (ex.RequestInformation != null && ex.RequestInformation.HttpStatusCode == 404)
                    return;
                throw;
            }
        }

        private async Task<bool> SaveProgressAsync()
        {
            var bytes = BlockLocator.ToBytes();
            try
            {

                await _Blob.UploadFromByteArrayAsync(bytes, 0, bytes.Length, new AccessCondition()
                {
                    IfMatchETag = _Blob.Properties.ETag
                }, null, null).ConfigureAwait(false);
            }
            catch (StorageException ex)
            {
                this.logger.Error(ex, "SaveProgressAsync failure");
                if (ex.RequestInformation != null && ex.RequestInformation.HttpStatusCode == 412)
                    return false;
                throw;
            }
            return true;
        }

        public static async Task<Checkpoint> LoadBlobAsync(CloudBlockBlob blob, Network network)
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
                Log.Error(ex, "LoadBlobAsync failure");
                if (ex.RequestInformation == null || ex.RequestInformation.HttpStatusCode != 404)
                    throw;
            }
            var checkpoint = new Checkpoint(checkpointName, network, ms, blob);
            return checkpoint;
        }

        public override string ToString()
        {
            return CheckpointName;
        }
    }

}
