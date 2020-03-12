using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AzureIndexer.Api.Infrastructure;
using AzureIndexer.Api.Models.Tokens;
using Microsoft.AspNetCore.Mvc;
using Stratis.Bitcoin.Features.AzureIndexer;

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
        public async Task<IEnumerable<TokenTransactionResponse>> TransactionsForToken([FromRoute] string tokenAddress, int? from = 0)
        {
            var results = await this.tokenSearchService.GetTransactionsForTokenAsync(tokenAddress, from ?? 0);

            return MapViewModel(results);
        }

        // Get all the transactions for a particular token address
        [HttpGet]
        [Route("{tokenAddress}/recent")]
        public async Task<IEnumerable<TokenTransactionResponse>> RecentTransactionsForToken([FromRoute] string tokenAddress)
        {
            var results = await this.tokenSearchService.GetMostRecentTransactionsForTokenAsync(tokenAddress);

            return MapViewModel(results);
        }

        private static IEnumerable<TokenTransactionResponse> MapViewModel(List<AddressTokenTransactionEntry> results)
        {
            var mapped = results.Select(r => new TokenTransactionResponse
            {
                Amount = r.Amount,
                FromAddress = r.AddressFrom,
                ToAddress = r.AddressTo,
                TokenAddress = r.TokenAddress,
                TxId = r.TxId,
                Time = (uint)r.Time
            });

            return mapped;
        }
    }
}