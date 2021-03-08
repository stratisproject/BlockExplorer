namespace Stratis.Features.AzureIndexer.Helpers
{
    using System;
    using NBitcoin;

    public static class ConsensusChainIndexerExtensions
    {
        /// <summary>
        /// Sets the tip of this chain based upon another block header.
        /// </summary>
        /// <param name="header">The block header to set to tip.</param>
        /// <returns>Whether the tip was set successfully.</returns>
        public static bool SetTip(this ChainIndexer chainIndexer, BlockHeader header)
        {
            ChainedHeader chainedHeader;
            return chainIndexer.TrySetTip(header, out chainedHeader);
        }

        public static bool TrySetTip(this ChainIndexer chainIndexer, BlockHeader header, out ChainedHeader chainedHeader)
        {
            if (header == null)
            {
                throw new ArgumentNullException("header");
            }

            chainedHeader = null;
            ChainedHeader prev = chainIndexer.GetHeader(header.HashPrevBlock);
            if (prev == null)
            {
                return false;
            }

            chainedHeader = new ChainedHeader(header, header.GetHash(), chainIndexer.GetHeader(header.HashPrevBlock));
            chainIndexer.SetTip(chainedHeader);
            return true;
        }

        public static ChainedHeader SetTip(this ChainIndexer chainIndexer, ChainedHeader block)
        {
            ChainedHeader fork = chainIndexer.Tip.FindFork(block);

            chainIndexer.Initialize(block);

            return fork;
        }
    }
}
