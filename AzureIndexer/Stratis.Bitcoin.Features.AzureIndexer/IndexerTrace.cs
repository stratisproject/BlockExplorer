using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using NBitcoin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Serilog;
using Serilog.Context;
using ILogger = Serilog.ILogger;

namespace Stratis.Bitcoin.Features.AzureIndexer
{
    public static class IndexerTrace
    {
        internal static void ErrorWhileImportingBlockToAzure(this ILogger logger, uint256 id, Exception ex)
        {
            logger.Error(ex, "Error while importing {id} in azure blob", id);
        }


        internal static void BlockAlreadyUploaded(this ILogger logger)
        {
			logger.Debug("Block already uploaded");
        }

        internal static void BlockUploaded(this ILogger logger, TimeSpan time, int bytes)
        {
            if (time.TotalSeconds == 0.0)
                time = TimeSpan.FromMilliseconds(10);
            double speed = ((double)bytes / 1024.0) / time.TotalSeconds;
			logger.Debug("Block uploaded successfully (" + speed.ToString("0.00") + " KB/S)");
        }

        internal static IDisposable NewCorrelation(string activityName)
        {
			return LogContext.PushProperty("ActivityName", activityName);
        }

        internal static void CheckpointLoaded(this ILogger logger, ChainedBlock block, string checkpointName)
        {
			logger.Information("Checkpoint {checkpointName} loaded at {block}", checkpointName, ToString(block));
        }

        internal static void CheckpointSaved(this ILogger logger, ChainedBlock block, string checkpointName)
        {
			logger.Information("Checkpoint {checkpointName} saved at {block}", checkpointName, ToString(block));
        }


        internal static void ErrorWhileImportingEntitiesToAzure(this ILogger logger, ITableEntity[] entities, Exception ex)
        {
            StringBuilder builder = new StringBuilder();
            int i = 0;
            foreach (var entity in entities)
            {
                builder.AppendLine("[" + i + "] " + entity.RowKey);
                i++;
            }
            logger.Error(ex, "Error while importing entities (len: {entitiesLength}\r\n {builder}", entities.Length, builder.ToString());
        }

        internal static void RetryWorked(this ILogger logger)
        {
            logger.Information("Retry worked");
        }

        public static string Pretty(TimeSpan span)
        {
            if (span == TimeSpan.Zero)
                return "0m";

            var sb = new StringBuilder();
            if (span.Days > 0)
                sb.AppendFormat("{0}d ", span.Days);
            if (span.Hours > 0)
                sb.AppendFormat("{0}h ", span.Hours);
            if (span.Minutes > 0)
                sb.AppendFormat("{0}m", span.Minutes);
            var result = sb.ToString();
            if (result == string.Empty)
                return "< 1min";
            return result;
        }

        internal static void TaskCount(this ILogger logger, int count)
        {
			logger.Information("Upload thread count : " + count);
        }

        internal static void ErrorWhileImportingBalancesToAzure(this ILogger logger, Exception ex, uint256 txid)
        {
			logger.Error(ex, "Error while importing balances on {txId}", txid);
        }

        internal static void MissingTransactionFromDatabase(this ILogger logger, uint256 txid)
        {
			logger.Error("Missing transaction from index while fetching outputs " + txid);
        }


        internal static void InputChainTip(this ILogger logger, ChainedBlock block)
        {
            logger.Information("The input chain tip is at height " + ToString(block));
        }

        private static string ToString(uint256 blockId, int height)
        {
            return height.ToString();
        }

        internal static void IndexedChainTip(this ILogger logger, uint256 blockId, int height)
        {
			logger.Information("Indexed chain is at height {blockInfo}", ToString(blockId, height));
        }

        internal static void InputChainIsLate(this ILogger logger)
        {
			logger.Information("The input chain is late compared to the indexed one");
        }

        public static void IndexingChain(this ILogger logger, ChainedBlock from, ChainedBlock to)
        {
			logger.Information("Indexing blocks from " + ToString(from) + " to " + ToString(to) + " (both included)");
        }

        private static string ToString(ChainedBlock chainedBlock)
        {
            if (chainedBlock == null)
                return "(null)";
            return ToString(chainedBlock.HashBlock, chainedBlock.Height);
        }

        internal static void RemainingBlockChain(this ILogger logger, int height, int maxHeight)
        {
            int remaining = height - maxHeight;
            if (remaining % 1000 == 0 && remaining != 0)
            {
				logger.Information("Remaining chain block to index : " + remaining + " (" + height + "/" + maxHeight + ")");
            }
        }

        internal static void IndexedChainIsUpToDate(this ILogger logger, ChainedBlock block)
        {
			logger.Information("Indexed chain is up to date at height " + ToString(block));
        }

        public static void Information(this ILogger logger, string message)
        {
            logger.Information(message);
        }

        public static void Trace(this ILogger logger, string message)
        {
            logger.Debug(message);
        }

        public static void Error(this ILogger logger, string message, Exception ex)
        {
            logger.Error(ex, message);
        }

        internal static void NoForkFoundWithStored(this ILogger logger)
        {
			logger.Information("No fork found with the stored chain");
        }

        public static void Processed(this ILogger logger, int height, int totalHeight, Queue<DateTime> lastLogs, Queue<int> lastHeights)
        {
            var lastLog = lastLogs.LastOrDefault();
            if (DateTime.UtcNow - lastLog > TimeSpan.FromSeconds(10))
            {
                if (lastHeights.Count > 0)
                {
                    var lastHeight = lastHeights.Peek();
                    var time = DateTimeOffset.UtcNow - lastLogs.Peek();

                    var downloadedSize = GetSize(lastHeight, height);
                    var remainingSize = GetSize(height, totalHeight);
                    var estimatedTime = downloadedSize < 1.0m ? TimeSpan.FromDays(999.0)
                        : TimeSpan.FromTicks((long)((remainingSize / downloadedSize) * time.Ticks));
					logger.Information("Blocks {0}/{1} (estimate : {2})", height, totalHeight, Pretty(estimatedTime));
                }
                lastLogs.Enqueue(DateTime.UtcNow);
                lastHeights.Enqueue(height);
                while (lastLogs.Count > 20)
                {
                    lastLogs.Dequeue();
                    lastHeights.Dequeue();
                }
            }
        }

        private static decimal GetSize(int t1, int t2)
        {
            decimal cumul = 0.0m;
            for (int i = t1 ; i < t2 ; i++)
            {
				var size = EstimateSize(i);
                cumul += (decimal)size;
            }
            return cumul;
        }

		static int OneMBHeight = 390000;

		private static decimal EstimateSize(decimal height)
        {
			if(height > OneMBHeight)
				return 1.0m;
            return (decimal)Math.Exp((double)(a * height + b));
        }

        static decimal a = 0.0000221438236661323m;
        static decimal b = -8.492328726823666132321613096m;

    }
}
