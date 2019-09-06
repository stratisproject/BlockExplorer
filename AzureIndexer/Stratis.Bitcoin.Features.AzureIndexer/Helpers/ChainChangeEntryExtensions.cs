using System;
using System.Linq;
using FodyNlogAdapter;
using Microsoft.Extensions.Logging;
using NLog;
using Stratis.Bitcoin.Features.AzureIndexer.Helpers;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Stratis.Bitcoin.Features.AzureIndexer
{
    using System.Collections.Generic;
    using NBitcoin;

    public static class ChainChangeEntryExtensions
    {
        private static object lockObj = new object();

        public static void UpdateChain(this IEnumerable<ChainBlockHeader> entries, ChainIndexer chain, ILogger logger = null)
        {
            var allEntries = entries.OrderBy(e => e.Height);
            try
            {
                foreach (ChainBlockHeader entry in allEntries)
                {
                    try
                    {
                        ChainedHeader prev = chain.GetHeader(entry.Header.HashPrevBlock);
                        if (prev == null && entry.Height > 0) continue;

                        var newTip = new ChainedHeader(entry.Header, entry.BlockId, prev);
                        if (chain.GetHeader(entry.Height) == null)
                        {
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
                        else if (newTip.Height <= chain.Tip.Height)
                        {
                            logger?.LogError("New tip is {newTip} current tip is {chain.Tip}", entry);
                            while (chain.Height >= entry.Height)
                            {
                                if (chain.Height == 0) break;
                                chain.Remove(chain.Tip);
                            }

                            chain.Add(newTip);
                        }
                    }
                    catch (Exception ex)
                    {
                        chain.Initialize(chain.Genesis);
                        logger?.LogError(ex, "Failed to add chain tip {entry}", entry);
                    }
                }
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Failed to add chain tip");
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