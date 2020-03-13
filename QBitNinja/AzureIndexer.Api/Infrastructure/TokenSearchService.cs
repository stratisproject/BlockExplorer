using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AzureIndexer.Api.Models.Response;
using NBitcoin;
using Stratis.Bitcoin.Features.AzureIndexer;
using Stratis.SmartContracts;

namespace AzureIndexer.Api.Infrastructure
{
    /// <summary>
    /// Implements repository-like behaviour for querying the token transactions store.
    /// </summary>
    public class TokenSearchService
    {
        public TokenSearchService(QBitNinjaConfiguration configuration)
        {
            this.Configuration = configuration?.Indexer ?? throw new ArgumentNullException(nameof(configuration));
        }

        public IndexerConfiguration Configuration { get; }

        /// <summary>
        /// Returns the most recent transactions for a token.
        /// </summary>
        /// <param name="tokenAddress"></param>
        /// <param name="count">The number of recent transactions to return. Must be less than 1000.</param>
        /// <returns></returns>
        public async Task<List<AddressTokenTransactionEntry>> GetMostRecentTransactionsForTokenAsync(
            string tokenAddress, int count = 25)
        {
            if (count >= 1000)
            {
                throw new ArgumentException("Count must be less than 1000");
            }

            var table = this.Configuration.GetTokenTransactionTable();
            var query = TokenQueries.LatestTokenTransactions(tokenAddress, count);

            try
            {
                // Segmented Query can return a maximum of 1000 results
                var result = await table.ExecuteQuerySegmentedAsync(query, null);

                return result.Results;
            }
            catch (Exception e)
            {
                return new List<AddressTokenTransactionEntry>();
            }
        }

        /// <summary>
        /// Returns all 100 the transactions for a token, beginning at fromBlock.
        /// </summary>
        /// <param name="tokenAddress"></param>
        /// <param name="from"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public async Task<List<AddressTokenTransactionEntry>> GetTransactionsForTokenAsync(string tokenAddress, string filterAddress = null, int fromBlock = 0)
        {
            var table = this.Configuration.GetTokenTransactionTable();

            // We can implement something similar to pagination that works nicer with table storage by adding block height range queries instead of entity count based queries.
            var query = TokenQueries.PaginatedTokenTransactions(tokenAddress, fromBlock, filterAddress, 1000);

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