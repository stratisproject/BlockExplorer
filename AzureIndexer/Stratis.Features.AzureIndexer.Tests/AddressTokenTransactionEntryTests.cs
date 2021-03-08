using System.Linq;
using NBitcoin;
using Xunit;

namespace Stratis.Features.AzureIndexer.Tests
{
    public class AddressTokenTransactionEntryTests
    {
        [Fact]
        public void CreateRowKey_Success()
        {
            var txid = uint256.One;
            var blockHeight = 543421;

            var key = AddressTokenTransactionEntry.CreateRowKey(blockHeight, txid);

            Assert.Equal("u543421:0000000000000000000000000000000000000000000000000000000000000001", key);
        }

        [Fact]
        public void CreateRowKey_IsOrderable_Descending()
        {
            var txid = uint256.One;
            var blockHeight = 543421;
            var blockHeight2 = 10543421;
            var blockHeight3 = 1234;
            var blockHeight4 = int.MaxValue;
            var blockHeight5 = 543420;


            var key = AddressTokenTransactionEntry.CreateRowKey(blockHeight, txid);
            var key2 = AddressTokenTransactionEntry.CreateRowKey(blockHeight2, txid);
            var key3 = AddressTokenTransactionEntry.CreateRowKey(blockHeight3, txid);
            var key4 = AddressTokenTransactionEntry.CreateRowKey(blockHeight4, txid);
            var key5 = AddressTokenTransactionEntry.CreateRowKey(blockHeight5, txid);
            var sort = new[] { key, key2, key3, key4, key5 }.OrderBy(k => k).ToList();

            Assert.Equal(sort[0].Split(':')[0], (int.MaxValue - blockHeight4).ToString());
            Assert.Equal(sort[1].Split(':')[0], (int.MaxValue - blockHeight2).ToString());
            Assert.Equal(sort[2].Split(':')[0], (int.MaxValue - blockHeight).ToString());
            Assert.Equal(sort[3].Split(':')[0], (int.MaxValue - blockHeight5).ToString());
            Assert.Equal(sort[4].Split(':')[0], (int.MaxValue - blockHeight3).ToString());
        }
    }
}