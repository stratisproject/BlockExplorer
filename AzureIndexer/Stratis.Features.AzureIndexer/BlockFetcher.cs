using Stratis.Features.AzureIndexer.Repositories;

namespace Stratis.Features.AzureIndexer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Microsoft.Extensions.Logging;
    using NBitcoin;

    public class BlockInfo
    {
        public int Height { get; set; }

        public uint256 BlockId { get; set; }

        public Block Block { get; set; }
    }

    public class BlockFetcher : IEnumerable<BlockInfo>
    {
        private readonly Checkpoint _Checkpoint;

        public Checkpoint Checkpoint
        {
            get
            {
                return this._Checkpoint;
            }
        }

        private readonly IBlocksRepository _BlocksRepository;

        public IBlocksRepository BlocksRepository
        {
            get
            {
                return this._BlocksRepository;
            }
        }

        private readonly ILoggerFactory loggerFactory;

        private readonly ILogger logger;

        private readonly ChainIndexer BlockHeaders;

        private void InitDefault()
        {
            this.NeedSaveInterval = TimeSpan.FromMinutes(15);
            this.ToHeight = int.MaxValue;
        }

        public BlockFetcher(Checkpoint checkpoint, IBlocksRepository blocksRepository, ChainIndexer chain, ChainedHeader lastProcessed, ILoggerFactory loggerFactory)
        {
            this.loggerFactory = loggerFactory;
            this.logger = this.loggerFactory.CreateLogger(this.GetType().FullName);

            if (blocksRepository == null)
            {
                throw new ArgumentNullException("blocksRepository");
            }

            if (chain == null)
            {
                throw new ArgumentNullException("blockHeaders");
            }

            if (checkpoint == null)
            {
                throw new ArgumentNullException("checkpoint");
            }

            this.BlockHeaders = chain;
            this._BlocksRepository = blocksRepository;
            this._Checkpoint = checkpoint;
            this._LastProcessed = lastProcessed;

            this.InitDefault();
        }

        public TimeSpan NeedSaveInterval { get; set; }

        public CancellationToken CancellationToken { get; set; }

        public ChainedHeader _LastProcessed { get; private set; }

        public int FromHeight { get; set; }

        public int ToHeight { get; set; }

        public IEnumerator<BlockInfo> GetEnumerator()
        {
            Queue<DateTime> lastLogs = new Queue<DateTime>();
            Queue<int> lastHeights = new Queue<int>();

            ChainedHeader fork = this.BlockHeaders.FindFork(this._Checkpoint.BlockLocator);

            if (fork == null)
            {
                yield break;
            }

            IEnumerable<ChainedHeader> headers = this.BlockHeaders.EnumerateAfter(fork);
            headers = headers?.Where(h => h.Height <= this.ToHeight);
            ChainedHeader first = headers?.FirstOrDefault();
            if (first == null)
            {
                yield break;
            }

            var height = first.Height;
            if (first.Height == 1)
            {
                headers = new[] { fork }.Concat(headers);
                height = 0;
            }

            foreach (Block block in this._BlocksRepository.GetBlocks(headers.Select(b => b.HashBlock), this.CancellationToken))
            {
                ChainedHeader header = this.BlockHeaders.GetHeader(height);

                if (block == null)
                {
                    Block storeTip = this._BlocksRepository.GetStoreTip();
                    if (storeTip != null)
                    {
                        // Store is caught up with Chain but the block is missing from the store.
                        if (header.Header.BlockTime <= storeTip.Header.BlockTime)
                        {
                            throw new InvalidOperationException($"Chained block not found in store (height = { height }). Re-create the block store.");
                        }
                    }

                    // Allow Store to catch up with Chain.
                    break;
                }

                this._LastProcessed = header;
                yield return new BlockInfo()
                {
                    Block = block,
                    BlockId = header.HashBlock,
                    Height = header.Height
                };

                IndexerTrace.Processed(height, Math.Min(this.ToHeight, this.BlockHeaders.Tip.Height), lastLogs, lastHeights);
                height++;
            }
        }

        internal void SkipToEnd()
        {
            var height = Math.Min(this.ToHeight, this.BlockHeaders.Tip.Height);
            this._LastProcessed = this.BlockHeaders.GetHeader(height);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private DateTime _LastSaved = DateTime.UtcNow;

        public bool NeedSave
        {
            get
            {
                return (DateTime.UtcNow - this._LastSaved) > this.NeedSaveInterval;
            }
        }

        public void SaveCheckpoint()
        {
            this.logger.LogTrace("()");

            if (this._LastProcessed != null)
            {
                this.logger.LogTrace("Saving checkpoints");

                this._Checkpoint.SaveProgress(this._LastProcessed);
                IndexerTrace.CheckpointSaved(this._LastProcessed, this._Checkpoint.CheckpointName);
            }

            this._LastSaved = DateTime.UtcNow;

            this.logger.LogTrace("(-)");
        }


    }
}
