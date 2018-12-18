using AzureIndexer.Api.Models.Response;

namespace AzureIndexer.Api.Controllers
{
    using AutoMapper;
    using Infrastructure;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using NBitcoin;

    [Route("api/v1/blocks")]
    public class BlocksController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IBlockSearchService blockSearchService;

        public BlocksController(
            ConcurrentChain chain,
            QBitNinjaConfiguration config,
            IMapper mapper,
            IBlockSearchService blockSearchService)
        {
            this.mapper = mapper;
            this.blockSearchService = blockSearchService;
            this.Configuration = config;
            this.Chain = chain;
        }

        public ConcurrentChain Chain { get; set; }

        public QBitNinjaConfiguration Configuration { get; set; }

        public Network Network => this.Configuration.Indexer.Network;

        [HttpGet]
        [Route("blocks/{block}")]
        public BlockResponseModel Block(string block, bool headerOnly = false, bool extended = false)
        {
            var blockData = this.blockSearchService.GetBlock(block.ToBlockFeature(), headerOnly, extended);
            return this.mapper.Map<BlockResponseModel>(blockData);
        }

        [HttpGet]
        [Route("blocks/{block}/header")]
        public BlockHeaderResponseModel BlockHeader(string block)
        {
            var blockData = this.blockSearchService.GetBlock(block.ToBlockFeature(), true);
            return this.mapper.Map<BlockHeaderResponseModel>(new BlockHeaderResponse(blockData.Header));
        }
    }
}
