using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using Stratis.Bitcoin.Tests;
using Xunit;
using System.Threading.Tasks;
using NBitcoin;
using Stratis.Bitcoin.Features.RPC.Controllers;
using Stratis.Bitcoin.Connection;
using Stratis.Bitcoin.Features.RPC.Models;
using System.Collections.Concurrent;
using Stratis.Bitcoin.Utilities;

namespace Stratis.Bitcoin.Features.IndexStore.Tests
{
    public class IndexStoreRPCControllerTest : TestBase
    {
        private IndexStoreManager indexStoreManager;
        private IndexStoreRPCController controller;
        private Mock<IIndexRepository> indexRepository;

        public IndexStoreRPCControllerTest()
        {
            this.indexRepository = new Mock<IIndexRepository>();
            this.indexStoreManager = new IndexStoreManager(null, null, this.indexRepository.Object,
                null, null, null, null);
            this.controller = new IndexStoreRPCController(this.loggerFactory, this.indexStoreManager);
        }

        [Fact]
        public async Task CreateIndex_IndexCreatedSuccessFully_ReturnsTrueAsync()
        {
            this.indexRepository.Setup(i => i.CreateIndexAsync("testIndex", true, "builder", null))
                .ReturnsAsync(true);

            var result = await this.controller.CreateIndexAsync("testIndex", true, "builder", null);

            Assert.True(result);
        }

        [Fact]
        public async Task CreateIndex_IndexCreatedUnSuccessFully_ReturnsFalseAsync()
        {
            this.indexRepository.Setup(i => i.CreateIndexAsync("testIndex", true, "builder", null))
                .ReturnsAsync(false);

            var result = await this.controller.CreateIndexAsync("testIndex", true, "builder", null);

            Assert.False(result);
        }

        [Fact]
        public async Task DropIndex_IndexDroppedSuccessFully_ReturnsTrueAsync()
        {
            this.indexRepository.Setup(i => i.DropIndexAsync("testIndex"))
                .ReturnsAsync(true);

            var result = await this.controller.DropIndexAsync("testIndex");

            Assert.True(result);
        }

        [Fact]
        public async Task DropIndex_IndexDroppedUnSuccessFully_ReturnsFalseAsync()
        {
            this.indexRepository.Setup(i => i.DropIndexAsync("testIndex"))
              .ReturnsAsync(false);

            var result = await this.controller.DropIndexAsync("testIndex");

            Assert.False(result);
        }

        [Fact]
        public void ListIndexNames_IndexesExist_ReturnsIndexNames()
        {
            using (var repository = SetupRepository(Network.Main, CreateTestDir(this)))
            {
                var builder = "(t,b,n) => t.Outputs.Where(o => o.ScriptPubKey.GetDestinationAddress(n)!=null).Select((o, N) => new object[] { new uint160(o.ScriptPubKey.Hash.ToBytes()), new object[] { t.GetHash(), (uint)N } })";
                var index = new Index(repository as IndexRepository, "Script", true, builder);
                var indexes = new ConcurrentDictionary<string, Index>();
                indexes.TryAdd("test", index);

                this.indexRepository.Setup(i => i.Indexes).Returns(indexes);

                var result = this.controller.ListIndexNames();

                Assert.NotEmpty(result);
                Assert.Equal("test", result[0]);
            }
        }

        [Fact]
        public void ListIndexNames_IndexesDoNotExist_ReturnsEmptyArray()
        {
            this.indexRepository.Setup(i => i.Indexes).Returns(new ConcurrentDictionary<string, Index>());

            var result = this.controller.ListIndexNames();

            Assert.Empty(result);
        }

        [Fact]
        public void DescribeIndex_IndexDoesNotExist_ReturnsNull()
        {
            this.indexRepository.Setup(i => i.Indexes).Returns(new ConcurrentDictionary<string, Index>());

            var result = this.controller.DescribeIndex("test");

            Assert.Null(result);
        }

        [Fact]
        public void DescribeIndex_IndexExists_ReturnsIndexDescription()
        {            
            using (var repository = SetupRepository(Network.Main, CreateTestDir(this)))
            {
                var builder = "(t,b,n) => t.Outputs.Where(o => o.ScriptPubKey.GetDestinationAddress(n)!=null).Select((o, N) => new object[] { new uint160(o.ScriptPubKey.Hash.ToBytes()), new object[] { t.GetHash(), (uint)N } })";
                var index = new Index(repository as IndexRepository, "Script", true, builder);
                var indexes = new ConcurrentDictionary<string, Index>();
                indexes.TryAdd("test", index);

                this.indexRepository.Setup(i => i.Indexes).Returns(indexes);

                var result = this.controller.DescribeIndex("test");

                Assert.NotEmpty(result);
                Assert.NotEmpty(result[0]);
            }
        }

        
        [Fact]
        public async Task GetRawTransactionAsync_Verbose_TransactionInBlockStore_ReturnsVerboseTransactionModelAsync()
        {
            var transaction = new Transaction()
            {
                Version = 125,
                Time = 12,
                LockTime = LockTime.Zero
            };

            this.indexRepository.Setup(i => i.GetTrxAsync(new uint256(0)))
              .ReturnsAsync(transaction);

            var result = await this.controller.GetRawTransactionAsync(new uint256(0).ToString(), 1);

            Assert.True(result is TransactionVerboseModel);
        }
        
        [Fact]
        public async Task GetRawTransactionAsync_Verbose_TransactionNotInBlockStore_ReturnsNullAsync()
        {
            this.indexRepository.Setup(i => i.GetTrxAsync(new uint256(0)))
                .ReturnsAsync((Transaction)null);

            var result = await this.controller.GetRawTransactionAsync(new uint256(0).ToString(), 1);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetRawTransactionAsync_NonVerbose_TransactionInBlockStore_ReturnsBriefTransactionModelAsync()
        {
            var transaction = new Transaction()
            {
                Version = 125,
                Time = 12,
                LockTime = LockTime.Zero
            };

            this.indexRepository.Setup(i => i.GetTrxAsync(new uint256(0)))
              .ReturnsAsync(transaction);

            var result = await this.controller.GetRawTransactionAsync(new uint256(0).ToString(), 0);

            Assert.True(result is TransactionBriefModel);
        }

        [Fact]
        public async Task GetRawTransactionAsync_NonVerbose_TransactionNotInBlockStore_ReturnsNullAsync()
        {
            this.indexRepository.Setup(i => i.GetTrxAsync(new uint256(0)))
               .ReturnsAsync((Transaction)null);

            var result = await this.controller.GetRawTransactionAsync(new uint256(0).ToString(), 0);

            Assert.Null(result);
        }

        private Features.IndexStore.IIndexRepository SetupRepository(Network main, string dir)
        {
            var repository = new IndexRepository(main, dir, DateTimeProvider.Default, this.loggerFactory);
            repository.InitializeAsync().GetAwaiter().GetResult();

            return repository;
        }
    }
}
