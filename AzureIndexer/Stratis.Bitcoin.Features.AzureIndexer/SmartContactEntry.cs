namespace Stratis.Bitcoin.Features.AzureIndexer
{
    using System;
    using Microsoft.WindowsAzure.Storage.Table;
    using NBitcoin;
    using Stratis.SmartContracts.CLR;

    public class SmartContactEntry
    {
        public class Entity : IIndexed
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
                this.ContractTxData = this.transactionEntity.ContractTxData;
            }

            public DynamicTableEntity CreateTableEntity(Network configNetwork)
            {
                var entity = new DynamicTableEntity
                {
                    ETag = "*", PartitionKey = this.PartitionKey, RowKey = this.RowKey
                };
                entity.Properties.AddOrReplace("GasPrice", new EntityProperty(this.ContractTxData.GasPrice));
                entity.Properties.AddOrReplace("MethodName", new EntityProperty(this.ContractTxData.MethodName));
                entity.Properties.AddOrReplace("OpCode", new EntityProperty(this.ContractTxData.OpCodeType)); // TODO Convert to proper string name

                return entity;
            }
        }
    }
}