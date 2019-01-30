using NBitcoin;
using System;
using System.Threading.Tasks;

namespace Stratis.Bitcoin.Features.AzureIndexer
{
    public class IndexerTransactionRepository : ITransactionRepository
    {
        private readonly IndexerConfiguration _Configuration;

        public IndexerConfiguration Configuration
        {
            get
            {
                return _Configuration;
            }
        }

        public IndexerTransactionRepository(IndexerConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            this._Configuration = config;
        }

        public async Task<Transaction> GetAsync(uint256 txId)
        {
            TransactionEntry tx = await _Configuration.CreateIndexerClient().GetTransactionAsync(false, txId).ConfigureAwait(false);

            return tx?.Transaction;
        }

        public Task PutAsync(uint256 txId, Transaction tx)
        {
            this._Configuration.CreateIndexer().Index(new TransactionEntry.Entity(txId, tx, null, Configuration.Network));
            return Task.FromResult(false);
        }

    }
}
