namespace Stratis.Features.AzureIndexer.Repositories
{
    using System;
    using System.Threading.Tasks;
    using NBitcoin;
    using Stratis.Features.AzureIndexer.Entities;

    public class IndexerTransactionRepository : ITransactionRepository
    {
        public IndexerConfiguration Configuration { get; }

        public IndexerTransactionRepository(IndexerConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            this.Configuration = config;
        }

        public async Task<Transaction> GetAsync(uint256 txId)
        {
            TransactionEntry tx = await this.Configuration.CreateIndexerClient().GetTransactionAsync(false, txId).ConfigureAwait(false);

            return tx?.Transaction;
        }

        public Task PutAsync(uint256 txId, Transaction tx)
        {
            this.Configuration.CreateIndexer().Index(new TransactionEntry.Entity(txId, tx, null, Configuration.Network));
            return Task.FromResult(false);
        }

    }
}
