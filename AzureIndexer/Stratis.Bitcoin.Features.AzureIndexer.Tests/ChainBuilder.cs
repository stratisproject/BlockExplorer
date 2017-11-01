using NBitcoin.BitcoinCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBitcoin.Indexer.Tests
{
    public class ChainBuilder
    {
        private IndexerTester _Tester;
        ConcurrentChain _Chain = new ConcurrentChain(Network.TestNet);

        public ConcurrentChain Chain
        {
            get
            {
                return _Chain;
            }
        }
        public ChainBuilder(IndexerTester indexerTester)
        {
            this._Tester = indexerTester;
            var genesis = indexerTester.Indexer.Configuration.Network.GetGenesis();
            _Blocks.Add(genesis.GetHash(), genesis);
        }

        Block _Current;

        public Block GetCurrentBlock()
        {
            var b = _Current = _Current ?? CreateNewBlock();
            _Chain.SetTip(b.Header);
            return b;
        }

        public bool NoRandom
        {
            get;
            set;
        }

        public Transaction EmitMoney(IDestination destination, Money amount, bool isCoinbase = true, bool indexBalance = false)
        {
            Transaction transaction = new Transaction();
            if (isCoinbase)
                transaction.AddInput(new TxIn()
                {
                    ScriptSig = new Script(NoRandom ? new uint256(0).ToBytes() : RandomUtils.GetBytes(32)),
                });
            transaction.AddOutput(new TxOut()
            {
                ScriptPubKey = destination.ScriptPubKey,
                Value = amount
            });
            Add(transaction, indexBalance);
            return transaction;
        }

        private void Add(Transaction tx, bool indexBalances)
        {
            var b = GetCurrentBlock();
            b.Transactions.Add(tx);
            if (!tx.IsCoinBase)
            {
                _Tester.Indexer.Index(new TransactionEntry.Entity(null, tx, null));
                if (indexBalances)
                    _Tester.Indexer.IndexOrderedBalance(tx);
            }
        }
        uint nonce = 0;
        private Block CreateNewBlock()
        {
            var b = new Block();
            b.Header.Nonce = nonce;
            nonce++;
            b.Header.HashPrevBlock = _Chain.Tip.HashBlock;
            b.Header.BlockTime = NoRandom ? new DateTimeOffset(1988, 07, 18, 0, 0, 0, TimeSpan.Zero) + TimeSpan.FromHours(nonce) : DateTimeOffset.UtcNow;
            return b;
        }

        public Block SubmitBlock()
        {
            var b = GetCurrentBlock();
            b.UpdateMerkleRoot();
            _Chain.SetTip(b.Header);
            _Current = null;
            _UnsyncBlocks.Add(b);
            _Blocks.Add(b.Header.GetHash(), b);
            _Mempool.Clear();
            return b;
        }

        List<Block> _UnsyncBlocks = new List<Block>();
        public void SyncIndexer()
        {
            _Tester.Indexer.IndexChain(_Chain);
            var walletRules = _Tester.Client.GetAllWalletRules();
            foreach (var b in _UnsyncBlocks)
            {
                var height = _Chain.GetBlock(b.GetHash()).Height;
                _Tester.Indexer.IndexOrderedBalance(height, b);
                foreach (var tx in b.Transactions)
                {
                    _Tester.Indexer.Index(new[] { new TransactionEntry.Entity(tx.GetHash(), tx, b.GetHash()) });
                }
                if (walletRules.Count() != 0)
                {
                    _Tester.Indexer.IndexWalletOrderedBalance(height, b, walletRules);
                }
            }
            _UnsyncBlocks.Clear();
        }

        public Transaction Emit(Transaction transaction, bool indexBalance = false)
        {
            Add(transaction, indexBalance);
            _Mempool.Add(transaction.GetHash(), transaction);
            return transaction;
        }

        public Block Generate(int count = 1)
        {
            Block b = null;
            for (int i = 0 ; i < count ; i++)
                b = SubmitBlock();
            return b;
        }


        public void Emit(IEnumerable<Transaction> transactions)
        {
            foreach (var tx in transactions)
                Emit(tx);
        }

        private readonly Dictionary<uint256, Block> _Blocks = new Dictionary<uint256, Block>();
        public Dictionary<uint256, Block> Blocks
        {
            get
            {
                return _Blocks;
            }
        }

        private readonly Dictionary<uint256, Transaction> _Mempool = new Dictionary<uint256, Transaction>();
        public Dictionary<uint256, Transaction> Mempool
        {
            get
            {
                return _Mempool;
            }
        }

        public void Load(string blockFolder)
        {
            var store = new BlockStore(blockFolder, this._Tester.Client.Configuration.Network);
            foreach (var block in store.Enumerate(false))
            {
                SubmitBlock(block.Item);
            }
        }

        public void SubmitBlock(Block block)
        {
            if (!Blocks.ContainsKey(block.GetHash()))
            {
                _Current = block;
                SubmitBlock();
            }
        }

    }
}
