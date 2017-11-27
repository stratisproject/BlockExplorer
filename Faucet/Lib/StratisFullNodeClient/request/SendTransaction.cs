using System;
using System.Net;
using System.Collections.Generic;

using Newtonsoft.Json;


namespace stratfaucet
{
    namespace request
    {
        public partial class SendTransaction
        {

            [JsonProperty("hex")]
            public string Hex { get; set; }
        }
    }
}
