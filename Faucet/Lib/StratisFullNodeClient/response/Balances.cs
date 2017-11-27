namespace stratfaucet
{
    namespace response
    {
        using System;
        using System.Net;
        using System.Collections.Generic;

        using Newtonsoft.Json;

        public partial class Balances
        {
            [JsonProperty("balances")]
            public List<Balance> BalancesList { get; set; }

        }
    }
}
