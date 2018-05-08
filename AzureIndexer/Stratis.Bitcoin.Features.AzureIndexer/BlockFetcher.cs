using NBitcoin;
using Stratis.Bitcoin.Features.AzureIndexer.IndexTasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Serilog;

namespace Stratis.Bitcoin.Features.AzureIndexer
{
    public class BlockInfo
    {
        public int Height
        {
            get;
            set;
        }
        public uint256 BlockId
        {
            get;
            set;
        }
        public Block Block
        {
            get;
            set;
        }
    }
    public class BlockFetcher : IEnumerable<BlockInfo>
    {

        private readonly Checkpoint _Checkpoint;
        public Checkpoint Checkpoint => _Checkpoint;

        private readonly IBlocksRepository _BlocksRepository;
        public IBlocksRepository BlocksRepository => _BlocksRepository;

        private readonly ChainBase blockHeaders;
        public ChainBase BlockHeaders => blockHeaders;

        private void InitDefault()
        {
            NeedSaveInterval = TimeSpan.FromMinutes(15);
            ToHeight = int.MaxValue;
        }

        public BlockFetcher(Checkpoint checkpoint, IBlocksRepository blocksRepository, ChainBase chain, ChainedBlock lastProcessed)
        {
            blockHeaders = chain ?? throw new ArgumentNullException(nameof(chain));
            _BlocksRepository = blocksRepository ?? throw new ArgumentNullException(nameof(blocksRepository));
            _Checkpoint = checkpoint ?? throw new ArgumentNullException(nameof(checkpoint));
            _LastProcessed = lastProcessed;

            InitDefault();
        }

        public TimeSpan NeedSaveInterval
        {
            get;
            set;
        }

        public CancellationToken CancellationToken
        {
            get;
            set;
        }

        #region IEnumerable<BlockInfo> Members

        public ChainedBlock _LastProcessed { get; private set; }

        public IEnumerator<BlockInfo> GetEnumerator()
        {
            Queue<DateTime> lastLogs = new Queue<DateTime>();
            Queue<int> lastHeights = new Queue<int>();

            var fork = blockHeaders.FindFork(_Checkpoint.BlockLocator);
            Log.Debug($"Fork is {fork.Height}");
            var headers = blockHeaders.EnumerateAfter(fork);
            headers = headers.Where(h => h.Height <= ToHeight);
            var first = headers.FirstOrDefault();
            Log.Debug($"First header is: {first?.Height}");
            if (first == null)
                yield break;
            var height = first.Height;
            if(first.Height == 1)
            {
                headers = new[] { fork }.Concat(headers);
                height = 0;
            }

            Log.Debug($"Get blocks");
            foreach (var block in _BlocksRepository.GetBlocks(headers.Select(b => b.HashBlock), CancellationToken))
            {
                Log.Debug($"Block {block?.Header}");
                var header = blockHeaders.GetBlock(height);

                Log.Debug($"Check if block is null");
                if (block == null)
                {
                    Log.Debug($"Block is NULL. Get store tip.");
                    var storeTip = _BlocksRepository.GetStoreTip();
                    if (storeTip != null)
                    {
                        Log.Debug($"Store tip is {storeTip.Header}.");
                        // Store is caught up with Chain but the block is missing from the store.
                        if (header.Header.BlockTime <= storeTip.Header.BlockTime)
                        {
                            Log.Debug("header.Header.BlockTime <= storeTip.Header.BlockTime");
                            Log.Debug($"header.Header - height:{header.Height}, blockTime:{header.Header.BlockTime}, previousTime:{header.Previous.Header.BlockTime}");
                            Log.Debug($"storeTip.Header - blockTime:{storeTip.Header.BlockTime}");
                            throw new InvalidOperationException(
                                $"Chained block not found in store (height = {height}). Re-create the block store. header.Header.BlockTime = {header.Header.BlockTime} and storeTip.Header.BlockTime = {storeTip.Header.BlockTime}");
                        }
                    }
                    // Allow Store to catch up with Chain.
                    break;
                }

                Log.Debug($"_LastProcessed is {_LastProcessed}.");
                _LastProcessed = header;
                yield return new BlockInfo()
                {
                    Block = block,
                    BlockId = header.HashBlock,
                    Height = header.Height
                };

                Log.Logger.Processed(height, Math.Min(ToHeight, blockHeaders.Tip.Height), lastLogs, lastHeights);
                height++;
            }
        }

        internal void SkipToEnd()
        {
            var height = Math.Min(ToHeight, blockHeaders.Tip.Height);
            _LastProcessed = blockHeaders.GetBlock(height);
            Log.Logger.Information("Skipped to the end at height " + height);
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        private DateTime _LastSaved = DateTime.UtcNow;
        public bool NeedSave => (DateTime.UtcNow - _LastSaved) > NeedSaveInterval;

        public async Task SaveCheckpoint()
        {
            Log.Debug($"Save checkpoint");
            if (_LastProcessed != null)
            {
                Log.Debug($"Last Processed: {_LastProcessed.Height}");
                await _Checkpoint.SaveProgress(_LastProcessed);
                Log.Logger.CheckpointSaved(_LastProcessed, _Checkpoint.CheckpointName);
            }
            _LastSaved = DateTime.UtcNow;
        }

        public int FromHeight
        {
            get;
            set;
        }

        public int ToHeight
        {
            get;
            set;
        }
    }
}
