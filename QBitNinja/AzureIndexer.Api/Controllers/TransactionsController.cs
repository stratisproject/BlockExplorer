namespace AzureIndexer.Api.Controllers
{
    using System.Threading.Tasks;
    using AutoMapper;
    using Infrastructure;
    using Microsoft.AspNetCore.Mvc;
    using Models.Response;
    using NBitcoin;

    [Route("api/v1/transactions")]
    public class TransactionsController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly ITransactionSearchService transactionSearchService;

        public TransactionsController(
            ConcurrentChain chain,
            QBitNinjaConfiguration config,
            IMapper mapper,
            ITransactionSearchService transactionSearchService)
        {
            this.mapper = mapper;
            this.transactionSearchService = transactionSearchService;
            this.Configuration = config;
            this.Chain = chain;
        }

        public ConcurrentChain Chain { get; set; }

        public QBitNinjaConfiguration Configuration { get; set; }

        public Network Network => this.Configuration.Indexer.Network;

        [HttpGet]
        [Route("{txId}")]
        public async Task<TransactionSummaryModel> Transaction(string txId, bool colored = false)
        {
            var response = await this.transactionSearchService.FindTransaction(uint256.Parse(txId), colored);
            var mappedResponse = this.mapper.Map<TransactionSummaryModel>(response);
            return mappedResponse;
        }
    }
}
