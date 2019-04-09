#pragma warning disable 618
namespace AzureIndexer.Api.Infrastructure
{
    using System.IO;
    using System.Linq;
    using System.Net;
    using AutoMapper;
    using Models;
    using NBitcoin;
    using Stratis.Bitcoin.Features.AzureIndexer;

    public class BlockSearchService : IBlockSearchService
    {
        private readonly ConcurrentChain chain;
        private readonly QBitNinjaConfiguration configuration;
        private readonly IMapper mapper;

        public BlockSearchService(ConcurrentChain chain, QBitNinjaConfiguration configuration, IMapper mapper)
        {
            this.chain = chain;
            this.configuration = configuration;
            this.mapper = mapper;
        }

        public BlockResponse GetBlock(BlockFeature blockFeature, bool headerOnly, bool extended)
        {
            var block = this.GetBlock(blockFeature, headerOnly);
            if (block == null)
            {
                throw new HttpResponseException("Block not found", HttpStatusCode.NotFound);
            }

            var response = new BlockResponse
            {
                AdditionalInformation = this.FetchBlockInformation(new[] { block.Header.GetHash() }) ?? new BlockInformation(block.Header),
                ExtendedInformation = extended ? this.FetchExtendedBlockInformation(blockFeature, block) : null,
                Block = headerOnly ? null : block
            };

            return response;
        }

        public Block GetBlock(BlockFeature blockFeature, bool headerOnly)
        {
            var chainedBlock = blockFeature.GetChainedBlock(this.chain);
            var hash = chainedBlock == null ? blockFeature.BlockId : chainedBlock.HashBlock;
            if (hash == null)
            {
                return null;
            }

            if (chainedBlock != null && chainedBlock.Height == 0)
            {
                return headerOnly ? new Block(this.configuration.Indexer.Network.GetGenesis().Header) : this.configuration.Indexer.Network.GetGenesis();
            }

            var client = this.configuration.Indexer.CreateIndexerClient();
            return headerOnly ? this.GetHeader(hash, client) : client.GetBlock(hash);
        }

        private ExtendedBlockInformation FetchExtendedBlockInformation(BlockFeature blockFeature, Block block)
        {
            var id = block.Header.GetHash().ToString();
            var extendedInfo = this.configuration.GetCacheTable<ExtendedBlockInformation>().ReadOne(id);
            if (extendedInfo != null)
            {
                return extendedInfo;
            }

            ChainedHeader chainedBlock = blockFeature.GetChainedBlock(this.chain);
            if (chainedBlock == null)
            {
                return null;
            }

            if (block.Transactions.Count == 0)
            {
                block = this.GetBlock(blockFeature, false);
                if (block == null || block.Transactions.Count == 0)
                {
                    return null;
                }
            }

            extendedInfo = new ExtendedBlockInformation()
            {
                BlockReward = block.Transactions[0].TotalOut,
                BlockSubsidy = this.GetBlockSubsidy(chainedBlock.Height),
                Size = this.GetSize(block, TransactionOptions.All),
                StrippedSize = this.GetSize(block, TransactionOptions.None),
                TransactionCount = block.Transactions.Count
            };

            this.configuration.GetCacheTable<ExtendedBlockInformation>().Create(id, extendedInfo);
            return extendedInfo;
        }

        private int GetSize(IBitcoinSerializable data, TransactionOptions options)
        {
            var bms = new BitcoinStream(Stream.Null, true) { TransactionOptions = options };
            data.ReadWrite(bms);
            return (int)bms.Counter.WrittenBytes;
        }

        private Money GetBlockSubsidy(int nHeight)
        {
            int halvings = nHeight / this.configuration.Indexer.Network.Consensus.SubsidyHalvingInterval;

            // Force block reward to zero when right shift is undefined.
            if (halvings >= 64)
            {
                return 0;
            }

            Money nSubsidy = Money.Coins(50);

            // Subsidy is cut in half every 210,000 blocks which will occur approximately every 4 years.
            nSubsidy >>= halvings;
            return nSubsidy;
        }

        private Block GetHeader(uint256 hash, IndexerClient client)
        {
            var header = this.chain.GetBlock(hash);
            if (header == null)
            {
                var b = client.GetBlock(hash);
                if (b == null)
                {
                    return null;
                }

                return new Block(b.Header);
            }

            return new Block(header.Header);
        }

        private BlockInformation FetchBlockInformation(uint256[] blockIds)
        {
            var confirmed = blockIds.Select(b => this.chain.GetBlock(b)).FirstOrDefault(b => b != null);
            if (confirmed == null)
            {
                return null;
            }

            return new BlockInformation
            {
                BlockId = confirmed.HashBlock,
                BlockHeader = confirmed.Header,
                Confirmations = this.chain.Tip.Height - confirmed.Height + 1,
                Height = confirmed.Height,
                MedianTimePast = confirmed.GetMedianTimePast(),
                BlockTime = confirmed.Header.BlockTime
            };
        }
    }
}
