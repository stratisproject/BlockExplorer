using System.Linq;

namespace AzureIndexer.Api.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoMapper;
    using Infrastructure;
    using Microsoft.AspNetCore.Mvc;
    using Models.Response;
    using NBitcoin;

    [Route("api/v1/smart-contracts")]
    public class SmartContractsController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly QBitNinjaConfiguration configuration;
        private readonly ITransactionSearchService transactionSearchService;
        private readonly ISmartContractSearchService smartContractSearchService;

        public SmartContractsController(
            IMapper mapper,
            QBitNinjaConfiguration configuration,
            ITransactionSearchService transactionSearchService,
            ISmartContractSearchService smartContractSearchService)
        {
            this.mapper = mapper;
            this.configuration = configuration;
            this.transactionSearchService = transactionSearchService;
            this.smartContractSearchService = smartContractSearchService;
        }

        [HttpGet]
        [Route("")]
        public async Task<List<TransactionSummaryModel>> SmartContracts(bool loadDetails = false, int take = 10)
        {
            var txs = new List<TransactionSummaryModel>();
            if (!this.configuration.Indexer.IsSidechain) return txs;
            var lastSmartContracts = await this.smartContractSearchService.GetAllSmartContracts(null, loadDetails);
            if (lastSmartContracts == null)
            {
                return txs;
            }

            foreach (var smartContractModel in lastSmartContracts.Take(take))
            {
                var response = await this.transactionSearchService.FindTransaction(uint256.Parse(smartContractModel.TxId), false);
                var mappedResponse = this.mapper.Map<TransactionSummaryModel>(response);
                mappedResponse.SmartContract = smartContractModel;
                txs.Add(mappedResponse);
            }

            return txs;
        }
    }
}
