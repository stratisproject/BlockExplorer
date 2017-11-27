namespace stratfaucet
{
    namespace response
    {
        using System;
        using System.Net;
        using System.Collections.Generic;

        using Newtonsoft.Json;

        public partial class Balance
        {
            [JsonProperty("accountHdPath")]
            public string AccountHdPath { get; set; }

            [JsonProperty("accountName")]
            public string AccountName { get; set; }

            [JsonProperty("amountConfirmed")]
            public decimal AmountConfirmed { get; set; }

            [JsonProperty("amountUnconfirmed")]
            public decimal AmountUnconfirmed { get; set; }

            [JsonProperty("coinType")]
            public long CoinType { get; set; }
        }
    }
}
