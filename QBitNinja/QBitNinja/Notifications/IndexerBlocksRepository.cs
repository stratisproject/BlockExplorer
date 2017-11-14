using Stratis.Bitcoin.Features.AzureIndexer.IndexTasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBitcoin;
using Stratis.Bitcoin.Features.AzureIndexer;
using System.Threading;

namespace QBitNinja.Notifications
{
    public class IndexerBlocksRepository : IBlocksRepository
    {
        IndexerClient _Client;
        public IndexerBlocksRepository(IndexerClient client)
        {
            _Client = client;
        }
        #region IBlocksRepository Members

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
