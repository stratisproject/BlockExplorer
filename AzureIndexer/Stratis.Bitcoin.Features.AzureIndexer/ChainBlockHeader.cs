using System.Collections.Generic;
using NBitcoin;

namespace Stratis.Bitcoin.Features.AzureIndexer
{
    public static class ChainChangeEntryExtensions
    {
        public static void UpdateChain(this IEnumerable<ChainBlockHeader> entries, ChainBase chain)
        {
            Stack<ChainBlockHeader> toApply = new Stack<ChainBlockHeader>();
            foreach (ChainBlockHeader entry in entries)
            {
                ChainedHeader prev = chain.GetBlock(entry.Header.HashPrevBlock);
                if (prev == null)
                {
                    toApply.Push(entry);
                }
                else
                {
                    toApply.Push(entry);
                    break;
                }
            }

            while (toApply.Count > 0)
            {
                ChainBlockHeader newTip = toApply.Pop();

                ChainedHeader chained = new ChainedHeader(newTip.Header, newTip.BlockId, chain.GetBlock(newTip.Header.HashPrevBlock));
                chain.SetTip(chained);
            }
        }
    }

    public class ChainBlockHeader
    {
        public uint256 BlockId
        {
            get;
            set;
        }

        public int Height
        {
            get;
            set;
        }

        public BlockHeader Header
        {
            get;
            set;
        }

        public override string ToString()
        {
            return Height + "-" + BlockId;
        }
    }
}
