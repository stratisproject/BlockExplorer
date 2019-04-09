namespace Stratis.Bitcoin.Features.AzureIndexer
{
    using System.Text;
    using Microsoft.WindowsAzure.Storage.Table;
    using NBitcoin.DataEncoders;

    public class WalletRuleEntry
    {
        public WalletRuleEntry()
        {

        }

        public WalletRuleEntry(DynamicTableEntity entity, IndexerClient client)
        {
            this.WalletId = Encoding.UTF8.GetString(Encoders.Hex.DecodeData(entity.PartitionKey));

            if (!entity.Properties.ContainsKey("a0")) // Legacy.
            {
                this.Rule = Helper.DeserializeObject<WalletRule>(Encoding.UTF8.GetString(Encoders.Hex.DecodeData(entity.RowKey)));
            }
            else
            {
                Rule = Helper.DeserializeObject<WalletRule>(Encoding.UTF8.GetString(Helper.GetEntityProperty(entity, "a")));
            }
        }

        public WalletRuleEntry(string walletId, WalletRule rule)
        {
            this.WalletId = walletId;
            this.Rule = rule;
        }

        public string WalletId { get; set; }

        public WalletRule Rule { get; set; }

        public DynamicTableEntity CreateTableEntity()
        {
            DynamicTableEntity entity = new DynamicTableEntity();
            entity.ETag = "*";
            entity.PartitionKey = Encoders.Hex.EncodeData(Encoding.UTF8.GetBytes(this.WalletId));

            if (this.Rule != null)
            {
                entity.RowKey = this.Rule.Id;
                Helper.SetEntityProperty(entity, "a", Encoding.UTF8.GetBytes(Helper.Serialize(this.Rule)));
            }

            return entity;
        }
    }
}
