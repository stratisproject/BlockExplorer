namespace Stratis.Bitcoin.Features.AzureIndexer
{
    using System.Collections.Generic;
    using NBitcoin;

    public static class ChainChangeEntryExtensions
    {
        public static void UpdateChain(this IEnumerable<ChainBlockHeader> entries, ChainIndexer chain)
        {
            Stack<ChainBlockHeader> toApply = new Stack<ChainBlockHeader>();
            foreach (ChainBlockHeader entry in entries)
            {
                ChainedHeader prev = chain.GetHeader(entry.Header.HashPrevBlock);
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

                ChainedHeader chained = new ChainedHeader(newTip.Header, newTip.BlockId, chain.GetHeader(newTip.Header.HashPrevBlock));
                chain.SetTip(chained);
            }
        }
    }
}