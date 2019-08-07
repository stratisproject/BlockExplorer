using System.Linq;
using FodyNlogAdapter;
using Microsoft.Extensions.Logging;
using NLog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Stratis.Bitcoin.Features.AzureIndexer
{
    using System.Collections.Generic;
    using NBitcoin;

    public static class ChainChangeEntryExtensions
    {
        public static void UpdateChain(this IEnumerable<ChainBlockHeader> entries, ChainIndexer chain, ILogger logger = null)
        {
            Stack<ChainBlockHeader> toApply = new Stack<ChainBlockHeader>();
            foreach (ChainBlockHeader entry in entries.OrderBy(e => e.Height))
            {
                ChainedHeader prev = chain.GetHeader(entry.Header.HashPrevBlock);
                chain.Add(new ChainedHeader(entry.Header, entry.BlockId, prev));
            }

            chain.SetTip(chain.Tip);
        }
    }
}