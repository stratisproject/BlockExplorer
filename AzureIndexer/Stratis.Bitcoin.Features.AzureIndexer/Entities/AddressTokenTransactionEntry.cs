using Microsoft.WindowsAzure.Storage.Table;
using NBitcoin;

namespace Stratis.Bitcoin.Features.AzureIndexer
{
    public class AddressTokenTransactionEntry : TableEntity
    {
        /// <summary>
        /// The sender address.
        /// </summary>
        public string Address
        {
            get => this.PartitionKey;
            set => this.PartitionKey = value;
        }

        public string AddressTo { get; set; }

        public string TokenAddress { get; set; }

        public string TokenSymbol { get; set; }

        public string Amount { get; set; } // Use a string because of big integers

        public long BlockHeight { get; set; } // Long is the maximum supported integer value for table storage. Duplicated here as well in the rowKey for better querying.

        public static string CreateRowKey(int blockHeight, uint256 txId)
        {
            return $"{blockHeight}:{txId}";
        }
    }
}