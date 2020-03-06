using System.Linq;
using Stratis.Bitcoin.Features.AzureIndexer.Tokens;
using Stratis.SmartContracts.Core;
using Stratis.SmartContracts.Core.Receipts;
using Stratis.SmartContracts.Core.State;

namespace Stratis.Bitcoin.Features.AzureIndexer.IndexTasks
{
    using System.Collections.Generic;
    using Microsoft.Extensions.Logging;
    using Microsoft.WindowsAzure.Storage.Table;
    using NBitcoin;
    using Stratis.Bitcoin.Features.AzureIndexer.Entities;

    public class IndexTokensTask : IndexTableEntitiesTaskBase<AddressTokenTransactionEntry>
    {
        private readonly IReceiptRepository receiptRepository;
        private readonly IStateRepositoryRoot state;
        private readonly LogDeserializer logDeserializer;
        private readonly ILogger logger;

        public IndexTokensTask(IndexerConfiguration configuration, ILoggerFactory loggerFactory,
            IReceiptRepository receiptRepository, IStateRepositoryRoot state, LogDeserializer logDeserializer) 
            : base(configuration, loggerFactory)
        {
            this.receiptRepository = receiptRepository;
            this.state = state;
            this.logDeserializer = logDeserializer;
            this.logger = loggerFactory.CreateLogger(this.GetType().FullName);
        }

        protected override CloudTable GetCloudTable()
        {
            return this.Configuration.GetAddressTokenTransactionTable();
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

                // Hang on, there's actually three addresses involved here:
                // The sender of the tokens
                // The recipient of the tokens
                // The token contract address

                foreach (var tokenDetail in tokenDetails)
                {
                    var fromEntity = new AddressTokenTransactionEntry
                    {
                        PartitionKey = tokenDetail.From,
                        RowKey = AddressTokenTransactionEntry.CreateRowKey(blockHeight, txHash),
                        BlockHeight = blockHeight,
                        AddressTo = tokenDetail.To,
                        Amount = $"-{tokenDetail.Amount}",  // TODO does this make sense? Relative to the from address the balance has decreased by -amount tokens
                        TokenAddress = tokenDetail.TokenAddress,
                        TokenSymbol = tokenDetail.TokenSymbol
                    };

                    bulk.Add(fromEntity.PartitionKey, fromEntity);

                    // Add this entity so we can also perform the reverse lookup on the recipient's address
                    var toEntity = new AddressTokenTransactionEntry
                    {
                        PartitionKey = tokenDetail.To,
                        RowKey = AddressTokenTransactionEntry.CreateRowKey(blockHeight, txHash),
                        BlockHeight = blockHeight,
                        AddressTo = tokenDetail.From,
                        Amount = tokenDetail.Amount,
                        TokenAddress = tokenDetail.TokenAddress,
                        TokenSymbol = tokenDetail.TokenSymbol
                    };

                    bulk.Add(toEntity.PartitionKey, toEntity);
                }
            }
        }

        protected override ITableEntity ToTableEntity(AddressTokenTransactionEntry item)
        {
            return item;
        }
    }
}