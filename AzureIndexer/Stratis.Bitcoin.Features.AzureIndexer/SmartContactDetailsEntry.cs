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

            public string RowKey => this.entity.PartitionKey;

            public string CSharpCode => this.entity.ContractCode;

            public ITableEntity CreateTableEntity()
            {
                return this.CreateTableEntity(null);
            }

            public DynamicTableEntity CreateTableEntity(Network network)
            {
                DynamicTableEntity entity = new DynamicTableEntity
                {
                    ETag = "*", PartitionKey = this.PartitionKey, RowKey = this.RowKey
                };
                entity.Properties.AddOrReplace(nameof(CSharpCode), new EntityProperty(this.CSharpCode));

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

            // TODO: remove once typo is fixed
            if (entity.Properties.ContainsKey("CShartCode"))
            {
                this.Code = entity.Properties["CShartCode"].StringValue;
            }
        }

        public string Code { get; set; }
    }
}