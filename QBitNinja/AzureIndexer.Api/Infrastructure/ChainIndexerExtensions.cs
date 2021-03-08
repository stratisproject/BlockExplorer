using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NBitcoin;

namespace AzureIndexer.Api.Infrastructure
{
    public static class ChainIndexerExtensions
    {
        public static bool Contains(this ChainIndexer indexer, uint256 id)
        {
            return indexer.GetHeader(id) != null;
        }

        public static bool Contains(this ChainIndexer indexer, int height)
        {
            return indexer.GetHeader(height) != null;
        }

        public static ChainedHeader GetBlock(this ChainIndexer indexer, uint256 id)
        {
            return indexer.GetHeader(id);
        }

        public static ChainedHeader GetBlock(this ChainIndexer indexer, int height)
        {
            try
            {
                return indexer.GetHeader(height);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
