using System;
using Stratis.SmartContracts.CLR;

namespace Stratis.Bitcoin.Features.AzureIndexer.IndexTasks
{
    using Microsoft.Extensions.Logging;
    using Microsoft.WindowsAzure.Storage.Table;
    using NBitcoin;

    public class IndexTransactionsTask : IndexTableEntitiesTaskBase<TransactionEntry.Entity>
    {
        private readonly ILogger logger;
        private readonly IndexerConfiguration config;

        public IndexTransactionsTask(IndexerConfiguration configuration, ILoggerFactory loggerFactory)
            : base(configuration, loggerFactory)
        {
            this.config = configuration;
            this.logger = loggerFactory.CreateLogger(GetType().FullName);
        }

        protected override void ProcessBlock(BlockInfo block, BulkImport<TransactionEntry.Entity> bulk, Network network)
        {
            this.logger.LogTrace("()");

            foreach (var transaction in block.Block.Transactions)
            {
                var indexed = new TransactionEntry.Entity(null, transaction, block.BlockId);
                if (indexed.HasSmartContract)
                {
                    var scEntity = new SmartContactEntry.Entity(indexed);

                }

                bulk.Add(indexed.PartitionKey, indexed);
            }

            this.logger.LogTrace("(-)");
        }

        protected CloudTable GetSmartContractCloudTable()
        {
            return this.Configuration.GetSmartContactTable();
        }

        protected CloudTable GetSmartContractCloudDetailTable()
        {
            return this.Configuration.GetSmartContactDetailTable();
        }

        protected override CloudTable GetCloudTable()
        {
            return this.Configuration.GetTransactionTable();
        }

        protected override ITableEntity ToTableEntity(TransactionEntry.Entity indexed)
        {
            return indexed.CreateTableEntity(config.Network);
        }

        protected ITableEntity ToTableEntity(SmartContactEntry.Entity indexed)
        {
            return indexed.CreateTableEntity(config.Network);
        }
    }

    public class SmartContactEntry
    {
        public class Entity
        {
            private readonly TransactionEntry.Entity transactionEntity;

            private string _partitionKey;

            public string PartitionKey
            {
                get
                {
                    if (this._partitionKey == null && this.TxId != null)
                    {
                        this._partitionKey = this.TxId.ToString();
                    }

                    return this._partitionKey;
                }
            }

            private string _rowKey;

            public string RowKey
            {
                get
                {
                    if (this._rowKey == null && this.TxId != null)
                    {
                        this._rowKey = this.ContractTxData.ContractAddress.ToString();
                    }

                    return this._rowKey;
                }
            }

            public DateTimeOffset Timestamp { get; set; }

            public uint256 TxId { get; set; }

            public ContractTxData ContractTxData { get; set; }

            public byte[] ContractByteCode { get; set; }

            public string ContractCode { get; set; }

            public Entity(TransactionEntry.Entity transactionEntity)
            {
                this.transactionEntity = transactionEntity;
            }

            public DynamicTableEntity CreateTableEntity(Network configNetwork)
            {
                var entity = new DynamicTableEntity
                {
                    ETag = "*", PartitionKey = this.PartitionKey, RowKey = this.RowKey
                };
                entity.Properties.AddOrReplace("GasPrice", new EntityProperty(this.ContractTxData.GasPrice.ToString()));
                entity.Properties.AddOrReplace("MethodName", new EntityProperty(this.ContractTxData.MethodName));
                entity.Properties.AddOrReplace("OpCode", new EntityProperty(this.ContractTxData.OpCodeType)); // TODO Convert to proper string name

                return entity;
            }
        }
    }
}
