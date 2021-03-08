using System.Threading.Tasks;
using AzureIndexer.Api.Models.Response;
using NBitcoin;

namespace AzureIndexer.Api.Infrastructure
{
    public interface ITransactionSearchService
    {
        Task<TransactionResponseModel> FindTransaction(uint256 txId, bool colored);
    }
}