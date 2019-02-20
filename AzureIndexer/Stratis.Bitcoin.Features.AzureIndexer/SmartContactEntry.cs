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
            public Entity(TransactionEntry.Entity transactionEntity)
            {
                this.transactionEntity = transactionEntity;
                this.TxId = this.transactionEntity.TxId;
                this.ContractTxData = this.transactionEntity.ContractTxData;
                this.ContractCode = this.transactionEntity.ContractCode;
                this.ContractByteCode = this.transactionEntity.ContractByteCode;
                this.Child = new SmartContactDetailsEntry.Entity(this);
            }

            private readonly TransactionEntry.Entity transactionEntity;

            private string _partitionKey;

            private string _rowKey;

            private AddressGenerator addressGenerator { get; } = new AddressGenerator();

            public SmartContactDetailsEntry.Entity Child { get; set; }

            public string PartitionKey
            {
                get
                {
                    if (this._partitionKey == null && this.TxId != null)
                    {
                        if (this.ContractTxData.IsCreateContract)
                        {
                            this._partitionKey = this.addressGenerator.GenerateAddress(this.TxId, 0).ToString();
                        }
                        else
                        {
                            this._partitionKey = this.ContractTxData.ContractAddress.ToString();
                        }
                    }

                    return this._partitionKey;
                }
            }

            public string RowKey
            {
                get
                {
                    if (this._rowKey == null && this.TxId != null)
                    {
                        this._rowKey = this.TxId.ToString();
                    }

                    return this._rowKey;
                }
            }

            public DateTimeOffset Timestamp { get; set; }

            public uint256 TxId { get; set; }

            public ContractTxData ContractTxData { get; set; }

            public byte[] ContractByteCode { get; set; }

            public string ContractCode { get; set; }

            public ITableEntity CreateTableEntity()
            {
                return this.CreateTableEntity(null);
            }

            public DynamicTableEntity CreateTableEntity(Network network = null)
            {
                DynamicTableEntity entity = new DynamicTableEntity
                {
                    ETag = "*", PartitionKey = this.PartitionKey, RowKey = this.RowKey
                };

                entity.Properties.AddOrReplace("GasPrice", new EntityProperty(Convert.ToInt64(this.ContractTxData.GasPrice)));
                entity.Properties.AddOrReplace("MethodName", new EntityProperty(this.ContractTxData.MethodName));
                entity.Properties.AddOrReplace("OpCode", new EntityProperty(this.ContractTxData.OpCodeType.ToString())); // TODO Convert to proper string name

                return entity;
            }

            public ITableEntity GetChildTableEntity()
            {
                return this.Child.CreateTableEntity();
            }

            public IIndexed GetChild()
            {
                return this.Child;
            }
        }

        public SmartContactEntry()
        {
        }

        public SmartContactEntry(DynamicTableEntity entity)
        {
            this.Id = entity.PartitionKey;

            if (entity.Properties.ContainsKey(nameof(this.OpCode)))
            {
                try
                {
                    this.OpCode = entity.Properties[nameof(this.OpCode)].StringValue;
                }
                catch
                {
                    this.OpCode = entity.Properties[nameof(this.OpCode)].Int32Value?.ToString();
                }
            }

            if (entity.Properties.ContainsKey(nameof(this.MethodName)))
            {
                this.MethodName = entity.Properties[nameof(this.MethodName)].StringValue;
            }

            if (entity.Properties.ContainsKey(nameof(this.GasPrice)))
            {
                try
                {
                    this.GasPrice = entity.Properties[nameof(this.GasPrice)].DoubleValue ?? 0;
                }
                catch
                {
                    this.GasPrice = entity.Properties[nameof(this.GasPrice)].Int64Value ?? 0;
                }
            }
        }

        public string Id { get; set; }

        public string OpCode { get; set; }

        public string MethodName { get; set; }

        public double GasPrice { get; set; }
    }
}