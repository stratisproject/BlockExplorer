namespace Stratis.Bitcoin.Features.AzureIndexer.Entities
{
    using System;
    using System.Collections.Generic;
    using Microsoft.WindowsAzure.Storage.Table;
    using NBitcoin;
    using Newtonsoft.Json;
    using Stratis.Bitcoin.Features.AzureIndexer.Helpers;
    using Stratis.SmartContracts.CLR;
    using Stratis.SmartContracts.Core;
    using Stratis.SmartContracts.Core.Receipts;

    public class SmartContactEntry
    {
        public class Entity : IIndexed
        {
            public Entity(uint256 txId, ContractTxData contractTxData, bool isSuccessful, string errorMessage, uint160 smartContractAddress, ISmartContractOperations smartContractOperations, Receipt receipt)
            {
                this.TxId = txId;
                this.ContractTxData = contractTxData;
                this.IsSuccessful = isSuccessful;
                this.ErrorMessage = errorMessage;
                this.SmartContractAddress = smartContractAddress;

                // if it's a contract creation, stores its detail too.
                if (isSuccessful && contractTxData.IsCreateContract)
                {
                    // ensures the creation was successful
                    this.Child = new SmartContactDetailsEntry.Entity(this, smartContractOperations);
                }

                // if receipt contains logs, parse them and store in Json format
                if (receipt?.Logs?.Length > 0)
                {
                    List<LogResponse> logs = smartContractOperations.MapLogResponses(receipt);

                    if (logs != null)
                    {
                        this.SmartContractOperationLog = JsonConvert.SerializeObject(logs);
                    }
                }
            }

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
                        if (this.ContractTxData?.IsCreateContract == true)
                        {
                            this._partitionKey = this.addressGenerator.GenerateAddress(this.TxId, 0).ToString();
                        }
                        else
                        {
                            this._partitionKey = this.ContractTxData?.ContractAddress.ToString();
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

            public bool IsSuccessful { get; }

            public string ErrorMessage { get; }

            public uint160 SmartContractAddress { get; }

            public string SmartContractOperationLog { get; }

            public ITableEntity CreateTableEntity()
            {
                return this.CreateTableEntity(null);
            }

            public DynamicTableEntity CreateTableEntity(Network network = null)
            {
                DynamicTableEntity entity = new DynamicTableEntity
                {
                    ETag = "*",
                    PartitionKey = this.PartitionKey,
                    RowKey = this.RowKey
                };

                entity.Properties.AddOrReplace("GasPrice", new EntityProperty(Convert.ToInt64(this.ContractTxData.GasPrice)));
                entity.Properties.AddOrReplace("MethodName", new EntityProperty(this.ContractTxData.MethodName));
                entity.Properties.AddOrReplace("OpCode", new EntityProperty(((ScOpcodeType)this.ContractTxData.OpCodeType).ToString()));
                entity.Properties.AddOrReplace("IsSuccessful", new EntityProperty(this.IsSuccessful));
                entity.Properties.AddOrReplace("SmartContractAddress", new EntityProperty(this.SmartContractAddress?.ToString()));
                entity.Properties.AddOrReplace("ErrorMessage", new EntityProperty(this.ErrorMessage));
                entity.Properties.AddOrReplace("SmartContractOperationLog", new EntityProperty(this.SmartContractOperationLog));

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

        /// <inheritdoc />
        public SmartContactEntry()
        {
        }

        public SmartContactEntry(DynamicTableEntity entity)
        {
            this.Id = entity.PartitionKey;
            this.TxId = entity.RowKey;
            this.Timestamp = entity.Timestamp;

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

            if (entity.Properties.TryGetValue(nameof(this.IsSuccessful), out EntityProperty isSuccessfulProperty))
            {
                this.IsSuccessful = isSuccessfulProperty.BooleanValue ?? false;
            }
            else
            {
                this.IsSuccessful = false;
            }

            if (entity.Properties.ContainsKey(nameof(this.SmartContractAddress)))
            {
                this.SmartContractAddress = entity.Properties[nameof(this.SmartContractAddress)].StringValue;
            }

            if (entity.Properties.ContainsKey(nameof(this.ErrorMessage)))
            {
                this.ErrorMessage = entity.Properties[nameof(this.ErrorMessage)].StringValue;
            }

            if (entity.Properties.ContainsKey(nameof(this.SmartContractOperationLog)))
            {
                this.SmartContractOperationLog = entity.Properties[nameof(this.SmartContractOperationLog)].StringValue;
            }
        }

        public string Id { get; set; }

        public string TxId { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public string OpCode { get; set; }

        public string MethodName { get; set; }

        public double GasPrice { get; set; }

        public bool IsSuccessful { get; set; }

        public string SmartContractAddress { get; private set; }

        public string ErrorMessage { get; private set; }

        public string SmartContractOperationLog { get; private set; }
    }
}