namespace Stratis.Bitcoin.Features.AzureIndexer.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using NBitcoin;
    using ILogger = Microsoft.Extensions.Logging.ILogger;

    public static class ChainChangeEntryExtensions
    {
        public static void UpdateChain(this IEnumerable<ChainBlockHeader> entries, ChainIndexer chain, ILogger logger = null)
        {
            foreach (ChainBlockHeader entry in entries.OrderBy(e => e.Height))
            {
                ChainedHeader prev = chain.GetHeader(entry.Header.HashPrevBlock);
                chain.Add(new ChainedHeader(entry.Header, entry.BlockId, prev));
            }

            chain.SetTip(chain.Tip);
        }
    }
}