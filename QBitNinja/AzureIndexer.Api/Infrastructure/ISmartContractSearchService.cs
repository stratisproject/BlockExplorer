using System.Threading.Tasks;
using AzureIndexer.Api.Models.Response;
using NBitcoin;

namespace AzureIndexer.Api.Infrastructure
{
    public interface ISmartContractSearchService
    {
        Task<SmartContractModel> FindSmartContract(uint256 txId);
    }
}