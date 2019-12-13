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
        private readonly Network network;

        public SmartContractSearchService(QBitNinjaConfiguration configuration, Network network)
        {
            this.configuration = configuration;
            this.network = network;
        }

        public async Task<SmartContractActionModel> FindSmartContract(uint256 txId, bool includeDetails)
        {
            var client = this.configuration.Indexer.CreateIndexerClient();
            var smartContract = await client.GetSmartContractAsync(txId);
            if (smartContract == null)
            {
                return null;
            }

            var smartContractModel = new SmartContractActionModel
            {
                Address = smartContract.Id,
                AddressBase58 = uint160.Parse(smartContract.Id).ToBase58Address(this.network),
                TxId = smartContract.TxId,
                OpCode = smartContract.OpCode,
                MethodName = smartContract.MethodName,
                GasPrice = (long)smartContract.GasPrice,
                IsSuccessful = smartContract.IsSuccessful,
                ErrorMessage = smartContract.ErrorMessage,
                Logs = smartContract.SmartContractOperationLog
            };

            if (includeDetails)
            {
                var smartContractDetails = await client.GetSmartContractDetailsAsync(smartContract.Id);
                if (smartContractDetails != null)
                {
                    smartContractModel.Code = smartContractDetails.Code;
                    smartContractModel.ContractName = smartContractDetails.ContractName;
                    smartContractModel.ContractSymbol = smartContractDetails.ContractSymbol;
                    smartContractModel.IsStandardToken = smartContractDetails.IsStandardToken;
                }
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

            if (!loadDetails)
            {
                return smartContractModels;
            }

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

        public async Task<List<SmartContractStandardTokenModel>> GetAllSmartContractStandardTokens(int? from = 0, int? take = 100)
        {
            //todo. pagination isn't used right now

            var client = this.configuration.Indexer.CreateIndexerClient();
            var smartContractDetails = await client.GetAllSmartContractDetailsAsync(take.Value);
            if (smartContractDetails == null)
            {
                return null;
            }

            var smartContractModels = smartContractDetails
                .OrderByDescending(r => r.ContractName)
                .Select(smartContract => new SmartContractStandardTokenModel
                {
                    Address = smartContract.Address,
                    Name = smartContract.ContractName,
                    Symbol = smartContract.ContractSymbol,
                    Code = null // code shouldn't be sent by default because it's too big
                }).ToList();

            return smartContractModels;
        }

        public async Task<SmartContractStandardTokenModel> GetSmartContractStandardToken(uint256 contractAddress)
        {
            var client = this.configuration.Indexer.CreateIndexerClient();
            var smartContract = await client.GetSmartContractDetailsAsync(contractAddress.ToString());
            if (smartContract == null)
            {
                return null;
            }

            return new SmartContractStandardTokenModel
            {
                Address = smartContract.Address,
                Name = smartContract.ContractName,
                Symbol = smartContract.ContractSymbol,
                Code = smartContract.Code
            };
        }
    }
}
