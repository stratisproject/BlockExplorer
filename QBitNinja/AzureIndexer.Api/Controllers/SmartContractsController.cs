using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AzureIndexer.Api.Infrastructure;
using AzureIndexer.Api.Models.Response;
using Microsoft.AspNetCore.Mvc;
using NBitcoin;

namespace AzureIndexer.Api.Controllers
{
    [Route("api/v1/smartcontracts")]
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
        public async Task<IEnumerable<SmartContractStandardTokenModel>> SmartContracts(int from = 0, int take = 10)
        {
            if (!this.configuration.Indexer.IsSidechain)
            {
                return Enumerable.Empty<SmartContractStandardTokenModel>();
            }

            var standardTokens = await this.smartContractSearchService.GetAllSmartContractStandardTokens(from, take);
            return standardTokens ?? Enumerable.Empty<SmartContractStandardTokenModel>();
        }

        [HttpGet]
        [Route("{address}")]
        public async Task<SmartContractStandardTokenModel> SmartContract(string address)
        {
            if (!this.configuration.Indexer.IsSidechain)
            {
                return null;
            }

            return await this.smartContractSearchService.GetSmartContractStandardToken(uint256.Parse(address));
        }

        [HttpGet]
        [Route("action/{txId}")]
        public async Task<SmartContractActionModel> SmartContractAction(string txId, bool includeDetails = false)
        {
            if (!this.configuration.Indexer.IsSidechain)
            {
                return null;
            }

            return await this.smartContractSearchService.FindSmartContract(uint256.Parse(txId), includeDetails);
        }

        //TODO
        //[HttpGet]
        //[Route("")]
        //public async Task<IEnumerable<SmartContractModel>> SmartContractActions(int from = 0, int take = 10)
        //{
        //    if (!this.configuration.Indexer.IsSidechain)
        //    {
        //        return Enumerable.Empty<SmartContractModel>();
        //    }

        //    var standardTokens = await this.smartContractSearchService.FindSmartContracts(from, take);
        //    return standardTokens ?? Enumerable.Empty<SmartContractModel>();
        //}
    }
}
