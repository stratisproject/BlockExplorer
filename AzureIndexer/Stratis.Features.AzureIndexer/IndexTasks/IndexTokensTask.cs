using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using NBitcoin;
using Stratis.Features.AzureIndexer.Entities;
using Stratis.Features.AzureIndexer.Tokens;
using Stratis.SmartContracts.Core.Receipts;
using Stratis.SmartContracts.Core.State;

namespace Stratis.Features.AzureIndexer.IndexTasks
{
    public class IndexTokensTask : IndexTableEntitiesTaskBase<AddressTokenTransactionEntry>
    {
        private readonly IReceiptRepository receiptRepository;
        private readonly IStateRepositoryRoot state;
        private readonly LogDeserializer logDeserializer;

        public IndexTokensTask(IndexerConfiguration configuration, ILoggerFactory loggerFactory, IReceiptRepository receiptRepository, IStateRepositoryRoot state, LogDeserializer logDeserializer)
            : base(configuration, loggerFactory)
        {
            this.receiptRepository = receiptRepository;
            this.state = state;
            this.logDeserializer = logDeserializer;
        }

        protected override CloudTable GetCloudTable()
        {
            return this.Configuration.GetTokenTransactionTable();
        }

        protected override void ProcessBlock(BlockInfo block, BulkImport<AddressTokenTransactionEntry> bulk, Network network, BulkImport<SmartContactEntry.Entity> smartContractBulk = null)
        {
            var blockHeight = block.Height;

            foreach (Transaction tx in block.Block.Transactions)
            {
                var txHash = tx.GetHash();

                Receipt receipt = this.receiptRepository.Retrieve(txHash);

                // Ignore non-SC transactions or failed transactions
                if (receipt == null || receipt.Success == false)
                {
                    continue;
                }

                // TODO DI
                var tokenEntityMapper = new TokenDetailProvider(this.receiptRepository, this.state, this.logDeserializer);

                var tokenDetails = tokenEntityMapper.Get(txHash);

                // Ignore non-tokens
                if (!tokenDetails.Any())
                    continue;

                foreach (var tokenDetail in tokenDetails)
                {
                    var fromEntity = new AddressTokenTransactionEntry
                    {
                        AddressFrom = tokenDetail.From,
                        RowKey = AddressTokenTransactionEntry.CreateRowKey(blockHeight, txHash),
                        BlockHeight = blockHeight,
                        AddressTo = tokenDetail.To,
                        Amount = tokenDetail.Amount,
                        TokenAddress = tokenDetail.TokenAddress,
                        TokenSymbol = tokenDetail.TokenSymbol,
                        Time = block.Block.Header.Time
                    };

                    bulk.Add(fromEntity.PartitionKey, fromEntity);
                }
            }
        }

        protected override ITableEntity ToTableEntity(AddressTokenTransactionEntry item)
        {
            return item;
        }
    }
}