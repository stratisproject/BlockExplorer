
using System;
using System.Net;
using System.Collections.Generic;

using Newtonsoft.Json;


namespace stratfaucet
{
    namespace request
    {
        public partial class BuildTransaction
        {

            [JsonProperty("walletName")]
            public string WalletName { get; set; }

            [JsonProperty("accountName")]
            public string AccountName { get; set; }


            [JsonProperty("coinType")]
            public int CoinType { get; set; }


            [JsonProperty("password")]
            public string Password { get; set; }


            [JsonProperty("destinationAddress")]
            public string DestinationAddress { get; set; }


            [JsonProperty("amount")]
            public string Amount { get; set; }


            [JsonProperty("feeType")]
            public string FeeType { get; set; }


            [JsonProperty("allowUnconfirmed")]
            public Boolean AllowUnconfirmed { get; set; }


        }
    }
}
