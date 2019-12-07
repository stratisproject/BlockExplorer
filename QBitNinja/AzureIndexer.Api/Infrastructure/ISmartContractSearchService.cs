using System.Collections.Generic;
using System.Threading.Tasks;
using AzureIndexer.Api.Models.Response;
using NBitcoin;

namespace AzureIndexer.Api.Infrastructure
{
    public interface ISmartContractSearchService
    {
        Task<SmartContractModel> FindSmartContract(uint256 txId, bool includeDetails);

        Task<List<SmartContractModel>> GetAllSmartContracts(int? take = 100, bool loadDetails = false);

        Task<List<SmartContractStandardTokenModel>> GetAllSmartContractStandardTokens(int? from = 0, int? take = 100);

        Task<SmartContractStandardTokenModel> GetSmartContractStandardToken(uint256 contractAddress);
    }
}