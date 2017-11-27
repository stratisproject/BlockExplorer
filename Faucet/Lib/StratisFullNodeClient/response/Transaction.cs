namespace stratfaucet
{
    namespace response
    {
        using System;
        using System.Net;
        using System.Collections.Generic;

        using Newtonsoft.Json;

        public partial class Transaction
        {
            [JsonProperty("fee")]
            public string Fee { get; set; }

            [JsonProperty("hex")]
            public string Hex { get; set; }


            [JsonProperty("transactionId")]
            public string TransactionId { get; set; }

        }
    }
}

