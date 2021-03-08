namespace Stratis.Features.AzureIndexer.Repositories
{
    using System.Collections.Generic;
    using System.Threading;
    using NBitcoin;
    using Stratis.Bitcoin;
    using Stratis.Bitcoin.Features.BlockStore;

    public interface IBlocksRepository
    {
        Block GetStoreTip();

        IEnumerable<Block> GetBlocks(IEnumerable<uint256> hashes, CancellationToken cancellationToken);
    }

    public class FullNodeBlocksRepository : IBlocksRepository
    {
        private readonly FullNode node;
        private readonly BlockRepository repo;

        public FullNodeBlocksRepository(FullNode node)
        {
            this.node = node;
            this.repo = node.NodeService<IBlockRepository>() as BlockRepository;
        }

        public Block GetStoreTip()
        {
            return this.repo.GetBlock(this.repo.TipHashAndHeight.Hash);
        }

        public IEnumerable<Block> GetBlocks(IEnumerable<uint256> hashes, CancellationToken cancellationToken)
        {
            foreach (uint256 hash in hashes)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                if (hash == this.node.Network.GenesisHash)
                {
                    yield return this.node.Network.GetGenesis();
                }
                else
                {
                    yield return this.repo.GetBlock(hash);
                }
            }
        }
    }
}
