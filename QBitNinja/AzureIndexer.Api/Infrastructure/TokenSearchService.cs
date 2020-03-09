using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AzureIndexer.Api.Models.Response;
using Microsoft.WindowsAzure.Storage.Table;
using NBitcoin;
using Stratis.Bitcoin.Features.AzureIndexer;
using Stratis.SmartContracts;

namespace AzureIndexer.Api.Infrastructure
{
    public class TokenSearchService
    {
        public TokenSearchService(QBitNinjaConfiguration configuration)
        {
            this.Configuration = configuration?.Indexer ?? throw new ArgumentNullException(nameof(configuration));
        }

        public IndexerConfiguration Configuration { get; }

        /// <summary>
        /// Returns all 100 the transactions for a token, beginning at fromBlock.
        /// </summary>
        /// <param name="tokenAddress"></param>
        /// <param name="from"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public async Task<List<AddressTokenTransactionEntry>> GetTransactionsForTokenAsync(string tokenAddress, int fromBlock = 0)
        {
            var table = this.Configuration.GetTokenTransactionTable();

            // We can implement something similar to pagination that works nicer with table storage by adding block height range queries instead of entity count based queries.
            var query = new TableQuery<AddressTokenTransactionEntry>();

            var partitionQuery = TableQuery.GenerateFilterCondition(nameof(AddressTokenTransactionEntry.PartitionKey), QueryComparisons.Equal, tokenAddress);
            var fromBlockQuery = TableQuery.GenerateFilterConditionForLong(nameof(AddressTokenTransactionEntry.BlockHeight), QueryComparisons.GreaterThanOrEqual, fromBlock);
            var toBlockQuery = TableQuery.GenerateFilterConditionForLong(
                nameof(AddressTokenTransactionEntry.BlockHeight), QueryComparisons.LessThan, fromBlock + 100);
            var rangeQuery = TableQuery.CombineFilters(fromBlockQuery, TableOperators.And, toBlockQuery);

            query = query.Where(partitionQuery);

            // We don't need to query all the columns.
            query = query.Select(new[]
            {
                nameof(AddressTokenTransactionEntry.PartitionKey),
                nameof(AddressTokenTransactionEntry.RowKey),
                nameof(AddressTokenTransactionEntry.Amount),
                nameof(AddressTokenTransactionEntry.BlockHeight),
                nameof(AddressTokenTransactionEntry.AddressFrom),
                nameof(AddressTokenTransactionEntry.AddressTo)
            });

            try
            {
                var result = await table.ExecuteQuerySegmentedAsync(query, null);

                return result.Results;

            }
            catch (Exception e)
            {
                return new List<AddressTokenTransactionEntry>();
            }
        }
    }
}