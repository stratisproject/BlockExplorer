namespace Stratis.Bitcoin.Features.AzureIndexer
{
    using System;
    using Microsoft.WindowsAzure.Storage.Table;
    using NBitcoin;

    public class SmartContactDetailsEntry
    {
        public class Entity : IIndexed
        {
            private SmartContactEntry.Entity entity;

            public Entity(SmartContactEntry.Entity entity)
            {
                this.entity = entity;
            }

            public string PartitionKey => "SmartContract";

            public string RowKey => this.ContractAddress.ToString();

            public uint160 ContractAddress => this.entity.ContractTxData.ContractAddress;

            public string CShartCode => this.entity.ContractCode;

            public ITableEntity CreateTableEntity()
            {
                return this.CreateTableEntity(null);
            }

            public DynamicTableEntity CreateTableEntity(Network network)
            {
                var entity = new DynamicTableEntity
                {
                    ETag = "*", PartitionKey = this.PartitionKey, RowKey = this.RowKey
                };
                entity.Properties.AddOrReplace(nameof(CShartCode), new EntityProperty(this.CShartCode));

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
            if (entity.Properties.ContainsKey("CSharpCode"))
            {
                this.Code = entity.Properties["CSharpCode"].StringValue;
            }
        }

        public string Code { get; set; }
    }
}