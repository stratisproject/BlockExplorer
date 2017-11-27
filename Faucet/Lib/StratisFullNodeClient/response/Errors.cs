namespace stratfaucet
{
    namespace response
    {
        using System;
        using System.Net;
        using System.Collections.Generic;

        using Newtonsoft.Json;

        public partial class Errors {

            [JsonProperty("errors")]
            public List<Error> ErrorList { get; set; }

        }
    }
}
