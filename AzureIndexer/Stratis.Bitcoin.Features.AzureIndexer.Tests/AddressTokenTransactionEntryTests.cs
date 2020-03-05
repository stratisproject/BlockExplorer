using NBitcoin;
using Xunit;

namespace Stratis.Bitcoin.Features.AzureIndexer.Tests
{
    public class AddressTokenTransactionEntryTests
    {
        [Fact]
        public void CreateRowKey_Success()
        {
            var txid = uint256.One;
            var blockHeight = 543421;

            var key = AddressTokenTransactionEntry.CreateRowKey(blockHeight, txid);

            Assert.Equal("543421:0000000000000000000000000000000000000000000000000000000000000001", key);
        }
    }
}