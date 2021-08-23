namespace Stratis.Features.AzureIndexer.Repositories
{
    using NBitcoin;
    using Stratis.Bitcoin;
    using Stratis.Bitcoin.Features.BlockStore;
    using Stratis.Bitcoin.Features.BlockStore.Repositories;
    using System.Collections.Generic;
    using System.Threading;

    public interface IBlocksRepository
    {
        Block GetStoreTip();

        IEnumerable<Block> GetBlocks(IEnumerable<uint256> hashes, CancellationToken cancellationToken);
    }

    public class FullNodeBlocksRepository : IBlocksRepository
    {
        private readonly FullNode node;
        private readonly LevelDbBlockRepository repo;

        public FullNodeBlocksRepository(FullNode node)
        {
            this.node = node;
            this.repo = node.NodeService<IBlockRepository>() as LevelDbBlockRepository;
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
