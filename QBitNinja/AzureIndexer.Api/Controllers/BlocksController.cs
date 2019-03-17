using System.Collections.Generic;
using System.Threading.Tasks;
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
        private readonly ITransactionSearchService transactionSearchService;

        public BlocksController(
            ConcurrentChain chain,
            QBitNinjaConfiguration config,
            IMapper mapper,
            IBlockSearchService blockSearchService,
            ITransactionSearchService transactionSearchService)
        {
            this.mapper = mapper;
            this.blockSearchService = blockSearchService;
            this.transactionSearchService = transactionSearchService;
            this.Configuration = config;
            this.Chain = chain;
        }

        public ConcurrentChain Chain { get; set; }

        public QBitNinjaConfiguration Configuration { get; set; }

        public Network Network => this.Configuration.Indexer.Network;

        [HttpGet]
        [Route("{block}")]
        public async Task<BlockResponseModel> Block(string block, bool headerOnly = false, bool extended = false)
        {
            var blockData = this.blockSearchService.GetBlock(block.ToBlockFeature(), headerOnly, extended);

            var mappedBlock =  this.mapper.Map<BlockResponseModel>(blockData);
            mappedBlock.Block.Transactions = new List<TransactionSummaryModel>();
            foreach (var transactionId in mappedBlock.Block.TransactionIds)
            {
                var transaction = await this.transactionSearchService.FindTransaction(uint256.Parse(transactionId), false);
                var transactionSummary = this.mapper.Map<TransactionSummaryModel>(transaction);
                transactionSummary.Spent = false;
                mappedBlock.Block.Transactions.Add(transactionSummary);
            }

            return mappedBlock;
        }

        [HttpGet]
        [Route("{block}/header")]
        public BlockHeaderResponseModel BlockHeader(string block)
        {
            var blockData = this.blockSearchService.GetBlock(block.ToBlockFeature(), true);
            return this.mapper.Map<BlockHeaderResponseModel>(new BlockHeaderResponse(blockData.Header));
        }

        [HttpGet]
        [Route("top")]
        public async Task<BlockResponseModel[]> Blocks(int top = 10)
        {
            var blocks = new List<BlockResponseModel>();
            var currentTip = this.Chain.Tip;
            for (int i = 0; i < top; i++)
            {
                if (currentTip == null) break;
                var blockData = this.blockSearchService.GetBlock(currentTip.Height.ToString().ToBlockFeature(), true, false);
                if (blockData != null)
                {
                    var mappedBlock = this.mapper.Map<BlockResponseModel>(blockData);
                    blocks.Add(mappedBlock);
                }

                currentTip = currentTip.Previous;
            }

            return blocks.ToArray();
        }
    }
}
