using System;
using System.Collections.Generic;
using System.Threading;
using NBitcoin;
using Stratis.Bitcoin.Features.AzureIndexer;
using Stratis.Bitcoin.Features.AzureIndexer.IndexTasks;
using Stratis.Bitcoin.Features.AzureIndexer.Repositories;

namespace AzureIndexer.Api.Notifications
{
    public class IndexerBlocksRepository : IBlocksRepository
    {
        IndexerClient _Client;
        public IndexerBlocksRepository(IndexerClient client)
        {
            _Client = client;
        }
        #region IBlocksRepository Members

        public Block GetStoreTip()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<NBitcoin.Block> GetBlocks(IEnumerable<NBitcoin.uint256> hashes, CancellationToken cancellation)
        {
            foreach (var h in hashes)
            {
                yield return _Client.GetBlock(h);
            }
        }

        #endregion
    }
}
