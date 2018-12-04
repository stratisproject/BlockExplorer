
using System;
using System.Net;
using System.Collections.Generic;

using Newtonsoft.Json;


namespace FaucetSite
{
    namespace request
    {
        public partial class BuildTransaction
        {
            public string FeeAmount { get; set; }
            public string Password { get; set; }
            public string OpReturnData { get; set; }
            public string WalletName { get; set; }
            public string AccountName { get; set; }
            public Outpoint[] Outpoints { get; set; }
            public Recipient[] Recipients { get; set; }
            public string FeeType { get; set; }
            public bool AllowUnconfirmed { get; set; }
            public bool ShuffleOutputs { get; set; }
        }

        public class Outpoint
        {
            public string TransactionId { get; set; }
            public long Index { get; set; }
        }

        public class Recipient
        {
            public string DestinationAddress { get; set; }
            public decimal Amount { get; set; }
        }
    }
}
