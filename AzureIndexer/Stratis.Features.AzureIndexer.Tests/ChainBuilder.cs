namespace Stratis.Features.AzureIndexer.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NBitcoin;
    using Stratis.Bitcoin.Networks;
    using Stratis.Features.AzureIndexer.Entities;
    using Stratis.Features.AzureIndexer.Helpers;

    public class ChainBuilder
    {
        private IndexerTester _Tester;

        uint nonce = 0;

        ChainIndexer _Chain = new ChainIndexer(new StraxTest());

        public ChainIndexer Chain => this._Chain;

        private Block _Current;

        public bool NoRandom { get; set; }

        public ChainBuilder(IndexerTester indexerTester)
        {
            this._Tester = indexerTester;
            Block genesis = indexerTester.Indexer.Configuration.Network.GetGenesis();
            this._Blocks.Add(genesis.GetHash(), genesis);
        }

        public Block GetCurrentBlock()
        {
            Block b = this._Current = this._Current ?? this.CreateNewBlock();
            this._Chain.SetTip(b.Header);
            return b;
        }

        public Transaction EmitMoney(IDestination destination, Money amount, bool isCoinbase = true, bool indexBalance = false)
        {
            Transaction transaction = new Transaction();
            if (isCoinbase)
            {
                transaction.AddInput(new TxIn()
                {
                    ScriptSig = new Script(this.NoRandom ? new uint256(0).ToBytes() : RandomUtils.GetBytes(32)),
                });
            }

            transaction.AddOutput(new TxOut()
            {
                ScriptPubKey = destination.ScriptPubKey,
                Value = amount
            });
            this.Add(transaction, indexBalance);
            return transaction;
        }

        private void Add(Transaction tx, bool indexBalances)
        {
            Block b = this.GetCurrentBlock();
            b.Transactions.Add(tx);
            if (!tx.IsCoinBase)
            {
                this._Tester.Indexer.Index(new TransactionEntry.Entity(null, tx, null, new StraxTest()));
                if (indexBalances)
                {
                    this._Tester.Indexer.IndexOrderedBalance(tx);
                }
            }
        }

        private Block CreateNewBlock()
        {
            Block b = new Block();
            b.Header.Nonce = this.nonce;
            this.nonce++;
            b.Header.HashPrevBlock = this._Chain.Tip.HashBlock;
            b.Header.BlockTime = this.NoRandom
                ? new DateTimeOffset(1988, 07, 18, 0, 0, 0, TimeSpan.Zero) + TimeSpan.FromHours(this.nonce)
                : DateTimeOffset.UtcNow;
            return b;
        }

        public Block SubmitBlock()
        {
            Block b = this.GetCurrentBlock();
            b.UpdateMerkleRoot();
            this._Chain.SetTip(b.Header);
            this._Current = null;
            this._UnsyncBlocks.Add(b);
            this._Blocks.Add(b.Header.GetHash(), b);
            this._Mempool.Clear();
            return b;
        }

        List<Block> _UnsyncBlocks = new List<Block>();

        public void SyncIndexer()
        {
            this._Tester.Indexer.IndexChain(this._Chain);
            WalletRuleEntryCollection walletRules = this._Tester.Client.GetAllWalletRules();
            foreach (Block b in this._UnsyncBlocks)
            {
                var height = this._Chain.GetHeader(b.GetHash()).Height;
                this._Tester.Indexer.IndexOrderedBalance(height, b);
                foreach (Transaction tx in b.Transactions)
                {
                    this._Tester.Indexer.Index(new[] { new TransactionEntry.Entity(tx.GetHash(), tx, b.GetHash(), KnownNetworks.StraxTest) });
                }

                if (walletRules.Count() != 0)
                {
                    this._Tester.Indexer.IndexWalletOrderedBalance(height, b, walletRules);
                }
            }

            this._UnsyncBlocks.Clear();
        }

        public Transaction Emit(Transaction transaction, bool indexBalance = false)
        {
            this.Add(transaction, indexBalance);
            this._Mempool.Add(transaction.GetHash(), transaction);
            return transaction;
        }

        public Block Generate(int count = 1)
        {
            Block b = null;
            for (var i = 0; i < count; i++)
            {
                b = this.SubmitBlock();
            }

            return b;
        }

        public void Emit(IEnumerable<Transaction> transactions)
        {
            foreach (Transaction tx in transactions)
            {
                this.Emit(tx);
            }
        }

        private readonly Dictionary<uint256, Block> _Blocks = new Dictionary<uint256, Block>();

        public Dictionary<uint256, Block> Blocks => this._Blocks;

        private readonly Dictionary<uint256, Transaction> _Mempool = new Dictionary<uint256, Transaction>();

        public Dictionary<uint256, Transaction> Mempool => this._Mempool;

        public void SubmitBlock(Block block)
        {
            if (!this.Blocks.ContainsKey(block.GetHash()))
            {
                this._Current = block;
                this.SubmitBlock();
            }
        }
    }
}
