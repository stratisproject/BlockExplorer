using AzureIndexer.Api.Models;

namespace AzureIndexer.Api.Controllers
{
    using System.Threading.Tasks;
    using AutoMapper;
    using Infrastructure;
    using Microsoft.AspNetCore.Mvc;
    using Models.Response;
    using NBitcoin;

    [Route("api/v1/balances")]
    public class BalancesController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly ITransactionSearchService transactionSearchService;
        private readonly IBalanceSearchService balanceSearchService;

        public BalancesController(
            QBitNinjaConfiguration config,
            IMapper mapper,
            ITransactionSearchService transactionSearchService,
            IBalanceSearchService balanceSearchService)
        {
            this.mapper = mapper;
            this.transactionSearchService = transactionSearchService;
            this.balanceSearchService = balanceSearchService;
            this.Configuration = config;
        }

        public QBitNinjaConfiguration Configuration { get; set; }

        public Network Network => this.Configuration.Indexer.Network;

        [HttpGet]
        [Route("{balanceId}")]
        public async Task<BalanceResponseModel> AddressBalance(
            string balanceId,
            string continuation = null,
            string until = null,
            string from = null,
            bool includeImmature = false,
            bool unspentOnly = false,
            bool colored = false,
            bool loadTransactionDetails = false)
        {
            colored = colored || this.IsColoredAddress();
            BalanceModel balance = this.balanceSearchService.GetBalanceDetails(balanceId.ToBalanceId(this.Network), continuation.ToBalanceLocator(), until.ToBlockFeature(), from.ToBlockFeature(), includeImmature, unspentOnly, colored);
            var mappedBalance = this.mapper.Map<BalanceResponseModel>(balance);

            if (!loadTransactionDetails)
            {
                return mappedBalance;
            }

            foreach (var operation in mappedBalance.Operations)
            {
                var transaction = await this.transactionSearchService.FindTransaction(uint256.Parse(operation.TransactionId), false);
                operation.TransactionSummary = this.mapper.Map<TransactionSummaryModel>(transaction);
                operation.TransactionSummary.Spent = operation.Amount?.Satoshi < 0;
            }

            return mappedBalance;
        }

        [HttpGet]
        [Route("{balanceId}/summary")]
        public BalanceSummaryModel AddressBalanceSummary(
            [FromRoute] string balanceId,
            string at = null,
            bool debug = false,
            bool colored = false)
        {
            colored = colored || this.IsColoredAddress();
            var summary = this.balanceSearchService.GetBalanceSummary(balanceId.ToBalanceId(this.Network), at.ToBlockFeature(), debug, colored);
            var mappedSummary = this.mapper.Map<BalanceSummaryModel>(summary);
            return mappedSummary;
        }

        private bool IsColoredAddress()
        {
            return this.HttpContext.Items.ContainsKey("BitcoinColoredAddress");
        }
    }
}
