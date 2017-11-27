namespace stratfaucet
{
    namespace response
    {
        using System;
        using System.Net;
        using System.Collections.Generic;

        using Newtonsoft.Json;

        public partial class Error
        {
            [JsonProperty("status")]
            public int Status { get; set; }

            [JsonProperty("message")]
            public int Message { get; set; }

            [JsonProperty("description")]
            public int Description { get; set; }
        }
    }
}
