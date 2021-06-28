using Microsoft.WindowsAzure.Storage.Table;
using Stratis.Features.AzureIndexer;

namespace AzureIndexer.Api.Infrastructure
{
    public static class TokenQueries
    {
        public static TableQuery<AddressTokenTransactionEntry> LatestTokenTransactions(string tokenAddress, int take = 25)
        {
            var query = new TableQuery<AddressTokenTransactionEntry>();

            var partitionQuery = TableQuery.GenerateFilterCondition(nameof(AddressTokenTransactionEntry.PartitionKey), QueryComparisons.Equal, tokenAddress);

            query.Where(partitionQuery);
            query.Take(take);

            return query;
        }

        public static TableQuery<AddressTokenTransactionEntry> PaginatedTokenTransactions(string tokenAddress, int fromBlock, string filterAddress = null, int take = 50)
        {
            var query = new TableQuery<AddressTokenTransactionEntry>();

            var partitionQuery = TableQuery.GenerateFilterCondition(nameof(AddressTokenTransactionEntry.PartitionKey), QueryComparisons.Equal, tokenAddress);
            //var fromBlockQuery = TableQuery.GenerateFilterConditionForLong(nameof(AddressTokenTransactionEntry.BlockHeight), QueryComparisons.GreaterThanOrEqual, fromBlock);
            //var toBlockQuery = TableQuery.GenerateFilterConditionForLong(
            //    nameof(AddressTokenTransactionEntry.BlockHeight), QueryComparisons.LessThan, fromBlock + take);
            //var rangeQuery = TableQuery.CombineFilters(fromBlockQuery, TableOperators.And, toBlockQuery);

            if (!string.IsNullOrWhiteSpace(filterAddress))
            {
                var addressFromFilter = TableQuery.GenerateFilterCondition(nameof(AddressTokenTransactionEntry.AddressFrom), QueryComparisons.Equal, filterAddress);
                var addressToFilter = TableQuery.GenerateFilterCondition(nameof(AddressTokenTransactionEntry.AddressTo), QueryComparisons.Equal, filterAddress);

                var addressCondition = TableQuery.CombineFilters(addressFromFilter, TableOperators.Or, addressToFilter);

                query = query.Where(TableQuery.CombineFilters(partitionQuery, TableOperators.And, addressCondition));
            }
            else
            {
                query = query.Where(partitionQuery);
            }

            query = query.Take(take);

            // We don't need to query all the columns.
            query = query.Select(new[]
            {
                nameof(AddressTokenTransactionEntry.PartitionKey),
                nameof(AddressTokenTransactionEntry.RowKey),
                nameof(AddressTokenTransactionEntry.Amount),
                nameof(AddressTokenTransactionEntry.BlockHeight),
                nameof(AddressTokenTransactionEntry.AddressFrom),
                nameof(AddressTokenTransactionEntry.AddressTo),
                nameof(AddressTokenTransactionEntry.Time)
            });

            return query;
        }
    }
}