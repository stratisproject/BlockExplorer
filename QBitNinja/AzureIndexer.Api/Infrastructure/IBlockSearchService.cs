using AzureIndexer.Api.Models;
using NBitcoin;

namespace AzureIndexer.Api.Infrastructure
{
    public interface IBlockSearchService
    {
        Block GetBlock(BlockFeature blockFeature, bool headerOnly);
        BlockResponse GetBlock(BlockFeature blockFeature, bool headerOnly, bool extended);
    }
}