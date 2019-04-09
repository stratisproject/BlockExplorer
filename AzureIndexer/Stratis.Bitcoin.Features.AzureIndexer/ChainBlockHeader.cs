namespace Stratis.Bitcoin.Features.AzureIndexer
{
    using NBitcoin;

    public class ChainBlockHeader
    {
        public uint256 BlockId { get; set; }

        public int Height { get; set; }

        public BlockHeader Header { get; set; }

        public override string ToString()
        {
            return Height + "-" + BlockId;
        }
    }
}
