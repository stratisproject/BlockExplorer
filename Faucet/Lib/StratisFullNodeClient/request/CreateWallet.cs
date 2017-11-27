using System;
using System.Net;
using System.Collections.Generic;

using Newtonsoft.Json;


namespace stratfaucet
{
    namespace request
    {

        public partial class CreateWallet
        {
            [JsonProperty("password")]
            public string Password { get; set; }

            [JsonProperty("network")]
            public string Network { get; set; }

            [JsonProperty("name")]
            public String Name { get; set; }
        }
    }
}
