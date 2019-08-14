using System;
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
            var allEntries = entries.OrderBy(e => e.Height);
            foreach (ChainBlockHeader entry in allEntries)
            {
                try
                {
                    ChainedHeader prev = chain.GetHeader(entry.Header.HashPrevBlock);
                    if (chain.GetHeader(entry.Height) == null)
                    {
                        var newTip = new ChainedHeader(entry.Header, entry.BlockId, prev);
                        try
                        {
                            chain.Add(newTip);
                        }
                        catch (Exception ex)
                        {
                            logger?.LogError(ex, "Failed to add chain tip {entry}", entry);
                            chain.Initialize(chain.Genesis);
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    chain.Initialize(chain.Genesis);
                    logger?.LogError(ex, "Failed to add chain tip {entry}", entry);
                }
            }

            chain.SetTip(chain.Tip);
        }

        private static void ResetChain(List<ChainBlockHeader> allEntries, ChainIndexer chain)
        {
            chain.Initialize(chain.Genesis);
            foreach (var item in allEntries)
            {
                ChainedHeader prev = chain.GetHeader(item.Header.HashPrevBlock);
                chain.Add(new ChainedHeader(item.Header, item.BlockId, prev));
            }
        }
    }
}