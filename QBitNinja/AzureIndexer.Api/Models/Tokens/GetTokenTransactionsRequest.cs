using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureIndexer.Api.Models.Tokens
{
    public class GetTokenTransactionsRequest
    {
        public string TokenAddress { get; set; }

        public int From { get; set; }

        public int Count { get; set; }
    }

    public class TokenTransactionResponse
    {
        public string TokenAddress { get; set; }

        public string FromAddress { get; set; }

        public string ToAddress { get; set; }

        public string TxId { get; set; }

        public string Amount { get; set; }

        public DateTimeOffset Time { get; set; }
    }
}
