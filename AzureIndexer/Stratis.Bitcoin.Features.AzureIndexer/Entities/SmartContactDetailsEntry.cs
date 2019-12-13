namespace Stratis.Bitcoin.Features.AzureIndexer
{
    using System;
    using Microsoft.WindowsAzure.Storage.Table;
    using NBitcoin;
    using Stratis.Bitcoin.Features.AzureIndexer.Entities;
    using Stratis.Bitcoin.Features.AzureIndexer.Helpers;

    public class SmartContactDetailsEntry
    {
        public class Entity : IIndexed
        {
            private SmartContactEntry.Entity entity;

            public Entity(SmartContactEntry.Entity entity, ISmartContractOperations smartContractOperations)
            {
                this.entity = entity;

                this.LoadSmartContractDetails(smartContractOperations, entity.SmartContractAddress);
            }

            private void LoadSmartContractDetails(ISmartContractOperations smartContractOperations, uint160 smartContractAddress)
            {
                var contractDetail = smartContractOperations.GetContractDetail(smartContractAddress);

                this.CSharpCode = contractDetail.contractCode;
                this.IsStandardToken = contractDetail.isStandardToken;

                if (this.IsStandardToken)
                {
                    this.ContractName = smartContractOperations.GetStandardTokenName(smartContractAddress);
                    this.ContractSymbol = smartContractOperations.GetStandardTokenSymbol(smartContractAddress);
                }
            }

            public string PartitionKey => "SmartContract";

            public string RowKey => this.entity.PartitionKey;

            public string CSharpCode { get; private set; }

            public bool IsStandardToken { get; private set; }

            public string ContractName { get; private set; }

            public string ContractSymbol { get; private set; }

            public ITableEntity CreateTableEntity()
            {
                return this.CreateTableEntity(null);
            }

            public DynamicTableEntity CreateTableEntity(Network network)
            {
                DynamicTableEntity entity = new DynamicTableEntity
                {
                    ETag = "*",
                    PartitionKey = this.PartitionKey,
                    RowKey = this.RowKey
                };

                entity.Properties.AddOrReplace(nameof(CSharpCode), new EntityProperty(this.CSharpCode));
                entity.Properties.AddOrReplace(nameof(IsStandardToken), new EntityProperty(this.IsStandardToken));
                entity.Properties.AddOrReplace(nameof(ContractName), new EntityProperty(this.ContractName));
                entity.Properties.AddOrReplace(nameof(ContractSymbol), new EntityProperty(this.ContractSymbol));

                return entity;
            }

            public ITableEntity GetChildTableEntity()
            {
                return null;
            }

            public IIndexed GetChild()
            {
                return null;
            }
        }

        public SmartContactDetailsEntry()
        {
        }

        public SmartContactDetailsEntry(DynamicTableEntity entity)
        {
            this.Address = entity.RowKey;

            if (entity.Properties.ContainsKey("CSharpCode"))
            {
                this.Code = entity.Properties["CSharpCode"].StringValue;
            }

            if (entity.Properties.TryGetValue("IsStandardToken", out EntityProperty isStandardTokenProperty))
            {
                this.IsStandardToken = isStandardTokenProperty.BooleanValue ?? false;
            }
            else
            {
                this.IsStandardToken = false;
            }

            if (entity.Properties.ContainsKey("ContractName"))
            {
                this.ContractName = entity.Properties["ContractName"].StringValue;
            }

            if (entity.Properties.ContainsKey("ContractSymbol"))
            {
                this.ContractSymbol = entity.Properties["ContractSymbol"].StringValue;
            }
        }

        public string Code { get; set; }

        public bool IsStandardToken { get; private set; }

        public string ContractName { get; private set; }

        public string ContractSymbol { get; private set; }

        public string Address { get; set; }
    }
}