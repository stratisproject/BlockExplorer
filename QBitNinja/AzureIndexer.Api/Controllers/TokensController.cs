using System.Threading.Tasks;
using AutoMapper;
using AzureIndexer.Api.Infrastructure;
using AzureIndexer.Api.Models.Tokens;
using Microsoft.AspNetCore.Mvc;

namespace AzureIndexer.Api.Controllers
{
    [Route("api/v1/tokens")]
    public class TokensController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly QBitNinjaConfiguration configuration;
        private readonly ITransactionSearchService transactionSearchService;
        private readonly TokenSearchService tokenSearchService;

        public TokensController(
            IMapper mapper,
            QBitNinjaConfiguration configuration,
            ITransactionSearchService transactionSearchService,
            TokenSearchService tokenSearchService)
        {
            this.mapper = mapper;
            this.configuration = configuration;
            this.transactionSearchService = transactionSearchService;
            this.tokenSearchService = tokenSearchService;
        }

        // Get all the transactions for a particular token address
        [HttpGet]
        [Route("{tokenAddress}")]
        public async Task<IActionResult> TransactionsForToken([FromRoute] string tokenAddress, int? from = 0)
        {
            var results = await this.tokenSearchService.GetTransactionsForTokenAsync(tokenAddress, from ?? 0);

            return Ok(results);
        }

        // Get all the transactions for a particular token address
        [HttpGet]
        [Route("{tokenAddress}/recent")]
        public async Task<IActionResult> RecentTransactionsForToken([FromRoute] string tokenAddress)
        {
            var results = await this.tokenSearchService.GetMostRecentTransactionsForTokenAsync(tokenAddress);

            return Ok(results);
        }
    }
}