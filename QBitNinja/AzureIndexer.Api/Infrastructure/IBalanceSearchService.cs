using AzureIndexer.Api.Models;
using Stratis.Features.AzureIndexer;

namespace AzureIndexer.Api.Infrastructure
{
    public interface IBalanceSearchService
    {
        BalanceSummary GetBalanceSummary(
            BalanceId balanceId,
            BlockFeature at,
            bool debug,
            bool colored);

        BalanceModel GetBalanceDetails(
            BalanceId balanceId,
            BalanceLocator continuation,
            BlockFeature until,
            BlockFeature from,
            bool includeImmature,
            bool unspentOnly,
            bool colored);
    }
}