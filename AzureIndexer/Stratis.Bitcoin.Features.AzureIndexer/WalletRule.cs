namespace Stratis.Bitcoin.Features.AzureIndexer
{
    using Newtonsoft.Json;
    using Stratis.Bitcoin.Features.AzureIndexer.Helpers;

    public abstract class WalletRule
    {
        public WalletRule()
        {
        }

        [JsonIgnore]
        public abstract string Id
        {
            get;
        }

        [JsonProperty(DefaultValueHandling=DefaultValueHandling.Ignore)]
        public string CustomData
        {
            get;
            set;
        }

        public override string ToString()
        {
            return Helper.Serialize(this);
        }
    }
}
