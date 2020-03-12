using Microsoft.WindowsAzure.Storage.Table;
using NBitcoin;

namespace Stratis.Bitcoin.Features.AzureIndexer
{
    public class AddressTokenTransactionEntry : TableEntity
    {
        /// <summary>
        /// The sender address.
        /// </summary>
        public string AddressFrom { get; set; }

        public string AddressTo { get; set; }

        public string TokenAddress
        {
            get => this.PartitionKey;
            set => this.PartitionKey = value;
        }

        public string TokenSymbol { get; set; }

        public string Amount { get; set; } // Use a string because of big integers

        public long BlockHeight { get; set; } // Long is the maximum supported integer value for table storage. Duplicated here as well in the rowKey for better querying.

        [IgnoreProperty]
        public string TxId
        {
            get
            {
                var split = this.RowKey?.Split(':');

                if (split == null || split.Length < 2)
                {
                    return null;
                }

                return split[1];
            }
        }

        public long Time { get; set; }

        public static string CreateRowKey(int blockHeight, uint256 txId)
        {
            // The maximum number of integers in an int string where int.MaxValue = 2,147,483,647
            const int Int32MaxIntegers = 10;

            // TableStorage returns results sorted by partition key, then rowkey.
            // We zero-pad the block height to ensure results are ordered when returned.
            // To ensure the results are returned in descending block height, we subtract the actual block height
            // from Int32.MaxValue
            return $"{GetDescendingPrefix(blockHeight)}{blockHeight.ToString()}:{txId}";
        }

        /// <summary>
        /// Returns the prefix 
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static char GetDescendingPrefix(int number)
        {
            // There should be 10 chars here, which is the maximum number of chars in an integer string.
            // We use descending order so that a number with 1 character returns 'z', a number with 2 characters returns 'y' etc.
            char[] prefixChars = { 'z', 'y', 'x', 'w', 'v', 'u', 't', 's', 'r', 'q' };

            var numChars = number.ToString().Length;

            return prefixChars[numChars - 1];
        }
    }
}