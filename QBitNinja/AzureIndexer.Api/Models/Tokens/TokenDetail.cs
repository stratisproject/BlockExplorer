using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureIndexer.Api.Models.Tokens
{
    public class TokenDetail
    {
        public string Address { get; set; }

        public string Name { get; set; }

        public string Symbol { get; set; }

        public ulong Supply { get; set; }

        public int Decimals { get; set; }
    }
}
