using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AzureIndexer.Api.Infrastructure;
using AzureIndexer.Api.Models.Tokens;
using Microsoft.AspNetCore.Mvc;
using NBitcoin;
using Stratis.Bitcoin.Features.AzureIndexer;
using Stratis.Bitcoin.Utilities.JsonConverters;

namespace AzureIndexer.Api.Controllers
{
    [Route("api/v1/tokens")]
    public class TokensController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly QBitNinjaConfiguration configuration;
        private readonly ITransactionSearchService transactionSearchService;
        private readonly TokenSearchService tokenSearchService;

        public Dictionary<string, TokenDetail> KnownTokens = new Dictionary<string, TokenDetail>
        {
            {"CUwkBGkXrQpMnZeWW2SpAv1Vu9zPvjWNFS", new TokenDetail
                {
                    Name = "Mediconnect",
                    Symbol = "MEDI",
                    Address = "CUwkBGkXrQpMnZeWW2SpAv1Vu9zPvjWNFS",
                    Decimals = 8,
                    Supply = 500000000
                }
            }
        };

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

        [HttpGet]
        [Route("{tokenAddress}")]
        public async Task<IActionResult> TokenDetail([FromRoute] string tokenAddress)
        {
            var known = this.KnownTokens.ContainsKey(tokenAddress);

            if (!known)
            {
                return BadRequest();
            }

            return Ok(this.KnownTokens[tokenAddress]);
        }

        // Get all the transactions for a particular token address
        [HttpGet]
        [Route("{tokenAddress}/transactions")]
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
                Time = Utils.UnixTimeToDateTime(r.Time)
            });

            return mapped;
        }
    }
}