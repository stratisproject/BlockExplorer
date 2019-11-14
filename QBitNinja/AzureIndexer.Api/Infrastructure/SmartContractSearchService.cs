using System.Collections.Generic;
using System.Linq;

namespace AzureIndexer.Api.Infrastructure
{
    using System.Threading.Tasks;
    using Models.Response;
    using NBitcoin;

    public class SmartContractSearchService : ISmartContractSearchService
    {
        private readonly QBitNinjaConfiguration configuration;

        public SmartContractSearchService(QBitNinjaConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<SmartContractModel> FindSmartContract(uint256 txId)
        {
            var client = this.configuration.Indexer.CreateIndexerClient();
            var smartContract = await client.GetSmartContractAsync(txId);
            if (smartContract == null)
            {
                return null;
            }

            var smartContractModel = new SmartContractModel
            {
                Hash = smartContract.Id,
                TxId = smartContract.TxId,
                OpCode = smartContract.OpCode,
                MethodName = smartContract.MethodName,
                GasPrice = new MoneyModel { Satoshi = (long?)smartContract.GasPrice },
                IsSuccessful = smartContract.IsSuccessful,
                ErrorMessage = smartContract.ErrorMessage
            };

            var smartContractDetails = await client.GetSmartContractDetailsAsync(smartContract.Id);
            if (smartContractDetails != null)
            {
                smartContractModel.Code = smartContractDetails.Code;
                smartContractModel.ContractName = smartContractDetails.ContractName;
                smartContractModel.ContractSymbol = smartContractDetails.ContractSymbol;
                smartContractModel.IsStandardToken = smartContractDetails.IsStandardToken;
            }

            return smartContractModel;
        }

        public async Task<List<SmartContractModel>> GetAllSmartContracts(int? take = 100, bool loadDetails = false)
        {
            var client = this.configuration.Indexer.CreateIndexerClient();
            var smartContracts = await client.GetAllSmartContractsAsync(take);
            if (smartContracts == null)
            {
                return null;
            }

            var smartContractModels = smartContracts.OrderByDescending(r => r.Timestamp).Select(smartContract => new SmartContractModel
            {
                Hash = smartContract.Id,
                TxId = smartContract.TxId,
                OpCode = smartContract.OpCode,
                MethodName = smartContract.MethodName,
                GasPrice = new MoneyModel { Satoshi = (long?)smartContract.GasPrice },
                IsSuccessful = smartContract.IsSuccessful,
                ErrorMessage = smartContract.ErrorMessage
            }).ToList();

            if (!loadDetails) return smartContractModels;
            foreach (var smartContractModel in smartContractModels)
            {
                var smartContractDetails = await client.GetSmartContractDetailsAsync(smartContractModel.Hash);
                if (smartContractDetails != null)
                {
                    smartContractModel.Code = smartContractDetails.Code;
                    smartContractModel.ContractName = smartContractDetails.ContractName;
                    smartContractModel.ContractSymbol = smartContractDetails.ContractSymbol;
                    smartContractModel.IsStandardToken = smartContractDetails.IsStandardToken;
                }
            }

            return smartContractModels;
        }
    }
}
