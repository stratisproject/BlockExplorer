namespace Stratis.Features.AzureIndexer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Storage.Table;
    using NBitcoin;
    using NBitcoin.OpenAsset;
    using NBitcoin.Protocol;
    using Stratis.Features.AzureIndexer.Entities;
    using Stratis.Features.AzureIndexer.Helpers;

    public class OrderedBalanceChange
    {
        public static IEnumerable<OrderedBalanceChange> ExtractScriptBalances(uint256 txId, Transaction transaction, uint256 blockId, BlockHeader blockHeader, int height, Network network)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException("transaction");
            }

            if (txId == null)
            {
                txId = transaction.GetHash();
            }

            if (blockId == null && blockHeader != null)
            {
                blockId = blockHeader.GetHash();
            }

            var changeByScriptPubKey = new Dictionary<Script, OrderedBalanceChange>();
            uint i = 0;
            foreach (TxIn input in transaction.Inputs)
            {
                if (transaction.IsCoinBase)
                {
                    i++;
                    break;
                }

                TxDestination signer = null;

                signer = input.ScriptSig.Length != 0 ? input.ScriptSig.GetSigner(network) : GetSigner(input.WitScript, network);

                if (signer != null)
                {
                    OrderedBalanceChange entry = null;
                    if (!changeByScriptPubKey.TryGetValue(signer.ScriptPubKey, out entry))
                    {
                        entry = new OrderedBalanceChange(txId, signer.ScriptPubKey, blockId, blockHeader, height);
                        changeByScriptPubKey.Add(signer.ScriptPubKey, entry);
                    }

                    entry.SpentOutpoints.Add(input.PrevOut);
                    entry.SpentIndices.Add(i);
                }

                i++;
            }

            i = 0;
            var hasOpReturn = false;
            foreach (TxOut output in transaction.Outputs)
            {
                byte[] bytes = output.ScriptPubKey.ToBytes(true);
                if (bytes.Length != 0 && bytes[0] == (byte)OpcodeType.OP_RETURN)
                {
                    hasOpReturn = true;
                    i++;
                    continue;
                }

                // Can be used to Extract Multisig Address
                var isScript = output.ScriptPubKey.IsPayToScriptHash(network);
                var ms = PayToScriptHashTemplate.Instance.CheckScriptPubKey(output.ScriptPubKey);
                if (isScript || ms)
                {
                    var addr = output.ScriptPubKey.GetDestinationAddress(network);
                }

                OrderedBalanceChange entry = null;

                if (!changeByScriptPubKey.TryGetValue(output.ScriptPubKey, out entry))
                {
                    entry = new OrderedBalanceChange(txId, output.ScriptPubKey, blockId, blockHeader, height);
                    changeByScriptPubKey.Add(output.ScriptPubKey, entry);
                }

                entry.ReceivedCoins.Add(new Coin()
                {
                    Outpoint = new OutPoint(txId, i),
                    TxOut = output
                });
                i++;
            }

            foreach (KeyValuePair<Script, OrderedBalanceChange> entity in changeByScriptPubKey)
            {
                entity.Value.HasOpReturn = hasOpReturn;
                entity.Value.IsCoinbase = transaction.IsCoinBase;
            }

            return changeByScriptPubKey.Values;
        }

        public static TxDestination GetSigner(WitScript witScript, Network network)
        {
            if (witScript == WitScript.Empty)
            {
                return null;
            }

            PayToWitPubkeyHashScriptSigParameters parameters = PayToWitPubKeyHashTemplate.Instance.ExtractWitScriptParameters(network, witScript);
            if (parameters != null)
            {
                return parameters.PublicKey.WitHash;
            }

            return Script.FromBytesUnsafe(witScript.GetUnsafePush(witScript.PushCount - 1)).WitHash;
        }

        public static IEnumerable<OrderedBalanceChange> ExtractWalletBalances(
                                                                            uint256 txId,
                                                                            Transaction tx,
                                                                            uint256 blockId,
                                                                            BlockHeader blockHeader,
                                                                            int height,
                                                                            WalletRuleEntryCollection walletCollection,
                                                                            Network network)
        {
            Dictionary<string, OrderedBalanceChange> entitiesByWallet = new Dictionary<string, OrderedBalanceChange>();
            IEnumerable<OrderedBalanceChange> scriptBalances = ExtractScriptBalances(txId, tx, blockId, blockHeader, height, network);
            foreach (OrderedBalanceChange scriptBalance in scriptBalances)
            {
                foreach (WalletRuleEntry walletRuleEntry in walletCollection.GetRulesFor(scriptBalance.ScriptPubKey))
                {
                    OrderedBalanceChange walletEntity = null;
                    if (!entitiesByWallet.TryGetValue(walletRuleEntry.WalletId, out walletEntity))
                    {
                        walletEntity = new OrderedBalanceChange(walletRuleEntry.WalletId, scriptBalance);
                        entitiesByWallet.Add(walletRuleEntry.WalletId, walletEntity);
                    }

                    walletEntity.Merge(scriptBalance, walletRuleEntry.Rule);
                }
            }

            foreach (OrderedBalanceChange b in entitiesByWallet.Values)
            {
                b.UpdateToScriptCoins();
            }

            return entitiesByWallet.Values;
        }

        private List<MatchedRule> _MatchedRules = new List<MatchedRule>();

        public List<MatchedRule> MatchedRules
        {
            get => this._MatchedRules;
            internal set => this._MatchedRules = value;
        }

        internal Task<bool> EnsureSpentCoinsLoadedAsync(uint256[] parentIds, Transaction[] transactions, IndexerConfiguration configuration)
        {
            NoSqlTransactionRepository repo = new NoSqlTransactionRepository(configuration.Network);
            for (int i = 0; i < parentIds.Length; i++)
            {
                if (transactions[i] == null)
                {
                    return Task.FromResult(false);
                }

                repo.Put(parentIds[i], transactions[i]);
            }
            return this.EnsureSpentCoinsLoadedAsync(repo);
        }

        public async Task<bool> EnsureSpentCoinsLoadedAsync(ITransactionRepository transactions)
        {
            if (this.SpentCoins != null)
            {
                return true;
            }

            var cleanSpent = false;
            CoinCollection result = new CoinCollection();
            for (var i = 0; i < this.SpentOutpoints.Count; i++)
            {
                OutPoint outpoint = this.SpentOutpoints[i];
                if (outpoint.IsNull)
                {
                    continue;
                }

                Transaction prev = await transactions.GetAsync(outpoint.Hash).ConfigureAwait(false);
                if (prev == null)
                {
                    return false;
                }

                Coin coin = new Coin(outpoint, prev.Outputs[this.SpentOutpoints[i].N]);
                if (coin.ScriptPubKey != this.GetScriptPubkey(i))
                {
                    cleanSpent = true;
                    this.SpentOutpoints[i] = null;
                }
                else
                {
                    result.Add(coin);
                }
            }

            if (cleanSpent)
            {
                var spentIndices = new List<uint>();
                var spentOutpoints = new List<OutPoint>();
                var matchedRules = new List<MatchedRule>();
                for (var i = 0; i < this.SpentOutpoints.Count; i++)
                {
                    if (this.SpentOutpoints[i] != null)
                    {
                        spentIndices.Add(this.SpentIndices[i]);
                        spentOutpoints.Add(this.SpentOutpoints[i]);
                        if (this.MatchedRules != null && this.MatchedRules.Count != 0)
                        {
                            matchedRules.Add(this.MatchedRules[i]);
                        }
                    }
                }

                this.SpentIndices = spentIndices;
                this.SpentOutpoints = spentOutpoints;
                this.MatchedRules = matchedRules;
            }

            this.SpentCoins = result;
            this.UpdateToScriptCoins();
            return true;
        }

        private Script GetScriptPubkey(int i)
        {
            if (this.MatchedRules.Count == 0)
            {
                return this.ScriptPubKey;
            }

            return ((ScriptRule)(this.MatchedRules.First(r => r.MatchType == MatchLocation.Input && r.Index == this.SpentIndices[i]).Rule)).ScriptPubKey;
        }

        internal void Merge(OrderedBalanceChange other, WalletRule walletRule)
        {
            if (other.ReceivedCoins.Count != 0)
            {
                this.ReceivedCoins.AddRange(other.ReceivedCoins);
                this.ReceivedCoins = new CoinCollection(this.ReceivedCoins.Distinct<ICoin, OutPoint>(c => c.Outpoint));
                if (walletRule != null)
                {
                    foreach (ICoin c in other.ReceivedCoins)
                    {
                        this.MatchedRules.Add(new MatchedRule()
                        {
                            Index = c.Outpoint.N,
                            Rule = walletRule,
                            MatchType = MatchLocation.Output
                        });
                    }
                }
            }

            if (other.SpentIndices.Count != 0)
            {
                this.SpentIndices.AddRange(other.SpentIndices);
                this.SpentIndices = this.SpentIndices.Distinct().ToList();

                this.SpentOutpoints.AddRange(other.SpentOutpoints);
                this.SpentOutpoints = this.SpentOutpoints.Distinct().ToList();

                // Remove cached value, no longer correct.
                this.UpdateToUncoloredCoins();
                this.SpentCoins = null;

                if (walletRule != null)
                {
                    foreach (var c in other.SpentIndices)
                    {
                        this.MatchedRules.Add(new MatchedRule()
                        {
                            Index = c,
                            Rule = walletRule,
                            MatchType = MatchLocation.Input
                        });
                    }
                }
            }
        }

        public void UpdateToScriptCoins()
        {
            foreach (MatchedRule match in this.MatchedRules)
            {
                ScriptRule scriptRule = match.Rule as ScriptRule;
                if (scriptRule != null && scriptRule.RedeemScript != null)
                {
                    if (match.MatchType == MatchLocation.Output)
                    {
                        OutPoint outpoint = new OutPoint(this.TransactionId, match.Index);
                        Coin coin = this.ReceivedCoins[outpoint] as Coin;
                        if (coin != null)
                        {
                            this.ReceivedCoins[outpoint] = coin.ToScriptCoin(scriptRule.RedeemScript);
                        }
                    }
                    else
                    {
                        if (this.SpentCoins == null)
                        {
                            continue;
                        }

                        var n = this.SpentIndices.IndexOf(match.Index);
                        Coin coin = this.SpentCoins[n] as Coin;
                        if (coin != null)
                        {
                            this.SpentCoins[n] = coin.ToScriptCoin(scriptRule.RedeemScript);
                        }
                    }
                }
            }
        }

        BalanceId _BalanceId;

        public BalanceId BalanceId
        {
            get => this._BalanceId;
            internal set => this._BalanceId = value;
        }

        public string PartitionKey => this.BalanceId.PartitionKey;

        public int Height { get; set; }

        public uint256 BlockId { get; set; }

        public uint256 TransactionId { get; set; }

        public bool HasOpReturn { get; set; }

        public bool IsCoinbase { get; set; }

        public DateTime SeenUtc { get; set; }

        public OrderedBalanceChange()
        {
            this._SpentIndices = new List<uint>();
            this._SpentOutpoints = new List<OutPoint>();
            this._ReceivedCoins = new CoinCollection();
        }

        private List<uint> _SpentIndices;

        public List<uint> SpentIndices
        {
            get => this._SpentIndices;
            private set => this._SpentIndices = value;
        }

        private List<OutPoint> _SpentOutpoints;

        public List<OutPoint> SpentOutpoints
        {
            get => this._SpentOutpoints;
            private set => this._SpentOutpoints = value;
        }

        private CoinCollection _ReceivedCoins;

        public CoinCollection ReceivedCoins
        {
            get => this._ReceivedCoins;
            private set => this._ReceivedCoins = value;
        }

        private CoinCollection _SpentCoins;

        /// <summary>
        /// Might be null if parent transactions have not yet been indexed
        /// </summary>
        public CoinCollection SpentCoins
        {
            get => this._SpentCoins;
            internal set => this._SpentCoins = value;
        }

        Money _Amount;

        public Money Amount
        {
            get
            {
                if (this._Amount == null && this._SpentCoins != null)
                {
                    this._Amount = this._ReceivedCoins.WhereUncolored().Select(c => c.Amount).Sum() - this._SpentCoins.WhereUncolored().Select(c => c.Amount).Sum();
                }
                return this._Amount;
            }
        }

        internal OrderedBalanceChange(DynamicTableEntity entity)
        {
            string[] splitted = entity.RowKey.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
            this.Height = Helper.StringToHeight(splitted[1]);
            this.BalanceId = BalanceId.Parse(splitted[0]);

            BalanceLocator locator = BalanceLocator.Parse(string.Join("-", splitted.Skip(1).ToArray()), true);
            ConfirmedBalanceLocator confLocator = locator as ConfirmedBalanceLocator;
            if (confLocator != null)
            {
                this.Height = confLocator.Height;
                this.TransactionId = confLocator.TransactionId;
                this.BlockId = confLocator.BlockHash;
            }

            UnconfirmedBalanceLocator unconfLocator = locator as UnconfirmedBalanceLocator;
            if (unconfLocator != null)
            {
                this.TransactionId = unconfLocator.TransactionId;
            }

            this.SeenUtc = entity.Properties["s"].DateTime.Value;

            this._SpentOutpoints = Helper.DeserializeList<OutPoint>(Helper.GetEntityProperty(entity, "a"));

            if (entity.Properties.ContainsKey("b0"))
            {
                this._SpentCoins = new CoinCollection(Helper.DeserializeList<Spendable>(Helper.GetEntityProperty(entity, "b")).Select(s => new Coin()
                {
                    Outpoint = s.OutPoint,
                    TxOut = s.TxOut
                }).ToList());
            }
            else if (this._SpentOutpoints.Count == 0)
            {
                this._SpentCoins = new CoinCollection();
            }

            this._SpentIndices = Helper.DeserializeList<IntCompactVarInt>(Helper.GetEntityProperty(entity, "ss")).Select(i => (uint)i.ToLong()).ToList();

            List<uint> receivedIndices = Helper.DeserializeList<IntCompactVarInt>(Helper.GetEntityProperty(entity, "c")).Select(i => (uint)i.ToLong()).ToList();
            List<TxOut> receivedTxOuts = Helper.DeserializeList<TxOut>(Helper.GetEntityProperty(entity, "d"));

            this._ReceivedCoins = new CoinCollection();
            for (int i = 0; i < receivedIndices.Count; i++)
            {
                this._ReceivedCoins.Add(new Coin()
                {
                    Outpoint = new OutPoint(this.TransactionId, receivedIndices[i]),
                    TxOut = receivedTxOuts[i]
                });
            }

            var flags = entity.Properties["e"].StringValue;
            this.HasOpReturn = flags[0] == 'o';
            this.IsCoinbase = flags[1] == 'o';

            this._MatchedRules = Helper.DeserializeObject<List<MatchedRule>>(entity.Properties["f"].StringValue).ToList();

            if (entity.Properties.ContainsKey("g"))
            {
                ColoredTransaction ctx = new ColoredTransaction();
                ctx.FromBytes(entity.Properties["g"].BinaryValue);
                this.ColoredTransaction = ctx;
            }

            if (entity.Properties.ContainsKey("h"))
            {
                this._ScriptPubKey = new Script(entity.Properties["h"].BinaryValue);
            }

            byte[] data = Helper.GetEntityProperty(entity, "cu");
            if (data != null)
            {
                this.CustomData = Encoding.UTF8.GetString(data);
            }
        }

        public ColoredTransaction ColoredTransaction { get; set; }

        public void UpdateToColoredCoins()
        {
            if (this.ColoredTransaction == null)
            {
                throw new InvalidOperationException("Impossible to get colored coin if ColoredTransaction is unknown");
            }

            this.UpdateToColoredCoins(this.SpentCoins, true);
            this.UpdateToColoredCoins(this.ReceivedCoins, false);
        }

        private void UpdateToColoredCoins(CoinCollection collection, bool input)
        {
            if (collection == null)
            {
                return;
            }

            for (int i = 0; i < collection.Count; i++)
            {
                Coin coin = collection[i] as Coin;
                if (coin != null)
                {
                    if (input)
                    {
                        var txinIndex = this.SpentIndices[i];
                        AssetMoney asset = this.ColoredTransaction
                                        .Inputs
                                        .Where(_ => _.Index == (uint)txinIndex)
                                        .Select(_ => _.Asset)
                                        .FirstOrDefault();
                        if (asset != null)
                        {
                            collection[i] = coin.ToColoredCoin(asset);
                        }
                    }
                    else
                    {
                        ColoredEntry asset = this.ColoredTransaction.GetColoredEntry(coin.Outpoint.N);
                        if (asset != null)
                        {
                            collection[i] = coin.ToColoredCoin(asset.Asset);
                        }
                    }
                }
            }
        }

        public void UpdateToUncoloredCoins()
        {
            if (this.SpentCoins != null)
            {
                this.UpdateToUncoloredCoins(this.SpentCoins);
                this.UpdateToUncoloredCoins(this.ReceivedCoins);
            }
        }

        private void UpdateToUncoloredCoins(CoinCollection collection)
        {
            if (collection == null)
            {
                return;
            }

            for (int i = 0; i < collection.Count; i++)
            {
                ColoredCoin coin = collection[i] as ColoredCoin;
                if (coin != null)
                {
                    collection[i] = coin.Bearer;
                }
            }
        }

        internal OrderedBalanceChange(uint256 txId, Script scriptPubKey, uint256 blockId, BlockHeader blockHeader, int height)
            : this()
        {
            BalanceId balanceId = new BalanceId(scriptPubKey);
            this.Init(txId, balanceId, blockId, blockHeader, height);
            if (!balanceId.ContainsScript)
            {
                this._ScriptPubKey = scriptPubKey;
            }
        }

        private void Init(uint256 txId, BalanceId balanceId, uint256 blockId, BlockHeader blockHeader, int height)
        {
            this.BlockId = blockId;
            this.SeenUtc = blockHeader == null ? DateTime.UtcNow : blockHeader.BlockTime.UtcDateTime;
            this.Height = blockId == null ? UnconfirmedBalanceLocator.UnconfHeight : height;
            this.TransactionId = txId;
            this.BalanceId = balanceId;
        }

        internal OrderedBalanceChange(uint256 txId, string walletId, Script scriptPubKey, uint256 blockId, BlockHeader blockHeader, int height)
            : this()
        {
            this.Init(txId, new BalanceId(walletId), blockId, blockHeader, height);
            this._ScriptPubKey = scriptPubKey;
        }

        internal OrderedBalanceChange(string walletId, OrderedBalanceChange source)
            : this(source.TransactionId, walletId, source.ScriptPubKey, source.BlockId, null, source.Height)
        {
            this.SeenUtc = source.SeenUtc;
            this.IsCoinbase = source.IsCoinbase;
            this.HasOpReturn = source.HasOpReturn;
        }

        internal class IntCompactVarInt : CompactVarInt
        {
            public IntCompactVarInt(uint value)
                : base(value, 4)
            {
            }

            public IntCompactVarInt()
                : base(4)
            {
            }
        }

        public BalanceLocator CreateBalanceLocator()
        {
            if (this.Height == UnconfirmedBalanceLocator.UnconfHeight)
            {
                return new UnconfirmedBalanceLocator(this.SeenUtc, this.TransactionId);
            }
            else
            {
                return new ConfirmedBalanceLocator(this);
            }
        }

        internal DynamicTableEntity ToEntity()
        {
            DynamicTableEntity entity = new DynamicTableEntity();
            entity.ETag = "*";
            entity.PartitionKey = this.PartitionKey;

            BalanceLocator locator = this.CreateBalanceLocator();
            entity.RowKey = this.BalanceId + "-" + locator.ToString(true);

            entity.Properties.Add("s", new EntityProperty(this.SeenUtc));
            Helper.SetEntityProperty(entity, "ss", Helper.SerializeList(this.SpentIndices.Select(e => new IntCompactVarInt(e))));

            Helper.SetEntityProperty(entity, "a", Helper.SerializeList(this.SpentOutpoints));
            if (this.SpentCoins != null)
            {
                Helper.SetEntityProperty(entity, "b", Helper.SerializeList(this.SpentCoins.Select(c => new Spendable(c.Outpoint, c.TxOut))));
            }

            Helper.SetEntityProperty(entity, "c", Helper.SerializeList(this.ReceivedCoins.Select(e => new IntCompactVarInt(e.Outpoint.N))));
            Helper.SetEntityProperty(entity, "d", Helper.SerializeList(this.ReceivedCoins.Select(e => e.TxOut)));
            var flags = (this.HasOpReturn ? "o" : "n") + (this.IsCoinbase ? "o" : "n");
            entity.Properties.AddOrReplace("e", new EntityProperty(flags));
            entity.Properties.AddOrReplace("f", new EntityProperty(Helper.Serialize(this.MatchedRules)));
            if (this.ColoredTransaction != null)
            {
                entity.Properties.AddOrReplace("g", new EntityProperty(this.ColoredTransaction.ToBytes()));
            }

            if (this.ScriptPubKey != null && !this.BalanceId.ContainsScript)
            {
                byte[] bytes = this.ScriptPubKey.ToBytes(true);
                if (bytes.Length < 63000)
                {
                    entity.Properties.Add("h", new EntityProperty(bytes));
                }
            }

            if (this.CustomData != null)
            {
                Helper.SetEntityProperty(entity, "cu", Encoding.UTF8.GetBytes(this.CustomData));
            }

            return entity;
        }

        public string CustomData { get; set; }

        const string DateFormat = "yyyyMMddhhmmssff";

        public static IEnumerable<OrderedBalanceChange> ExtractScriptBalances(Transaction tx, Network network)
        {
            return ExtractScriptBalances(null, tx, null, null, 0, network);
        }

        Script _ScriptPubKey;

        internal Script ScriptPubKey
        {
            get
            {
                if (this._ScriptPubKey == null)
                {
                    this._ScriptPubKey = this.BalanceId.ExtractScript();
                }

                return this._ScriptPubKey;
            }
        }

        public IEnumerable<WalletRule> GetMatchedRules(int index, MatchLocation matchType)
        {
            return this.MatchedRules.Where(r => r.Index == index && r.MatchType == matchType).Select(c => c.Rule);
        }


        public IEnumerable<WalletRule> GetMatchedRules(ICoin coin)
        {
            return this.GetMatchedRules(coin.Outpoint);
        }

        public bool MempoolEntry => this.BlockId == null;

        internal bool IsEmpty => this.SpentCoins.Count == 0 && this.ReceivedCoins.Count == 0;

        public IEnumerable<WalletRule> GetMatchedRules(OutPoint outPoint)
        {
            if (outPoint.Hash == this.TransactionId)
            {
                return this.GetMatchedRules((int)outPoint.N, MatchLocation.Output);
            }
            else
            {
                var index = this.SpentOutpoints.IndexOf(outPoint);
                if (index == -1)
                {
                    return new WalletRule[0];
                }

                return this.GetMatchedRules((int)this.SpentIndices[index], MatchLocation.Input);
            }
        }

        public async Task<bool> EnsureColoredTransactionLoadedAsync(IColoredTransactionRepository repository)
        {
            if (this.ColoredTransaction != null)
            {
                this.UpdateToColoredCoins();
                return true;
            }

            if (!(repository is CachedColoredTransactionRepository))
            {
                repository = new CachedColoredTransactionRepository(repository);
            }

            Transaction tx = await repository.Transactions.GetAsync(this.TransactionId).ConfigureAwait(false);
            if (tx == null)
            {
                return false;
            }

            try
            {
                ColoredTransaction color = await tx.GetColoredTransactionAsync(repository).ConfigureAwait(false);
                if (color == null)
                {
                    return false;
                }

                this.ColoredTransaction = color;
                this.UpdateToColoredCoins();
                return true;
            }
            catch (TransactionNotFoundException)
            {
                return false;
            }
        }

        /// <summary>
        /// Get the quantity of asset in this balance change
        /// </summary>
        /// <param name="assetId">The asset id, if null, returns uncolored satoshi</param>
        /// <returns>Money</returns>
        public IMoney GetAssetAmount(BitcoinAssetId assetId)
        {
            if (assetId == null)
            {
                return this.Amount;
            }

            return this.GetAssetAmount(assetId.AssetId);
        }



        /// <summary>
        /// Get the quantity of asset in this balance change
        /// </summary>
        /// <param name="assetId">The asset id, if null, returns uncolored satoshi</param>
        /// <returns>Money</returns>
        public IMoney GetAssetAmount(AssetId assetId)
        {
            if (assetId == null)
            {
                return this.Amount;
            }

            AssetMoney amount = this._ReceivedCoins.WhereColored(assetId)
                .Select(c => c.Amount).Sum(assetId) - this._SpentCoins.WhereColored(assetId).Select(c => c.Amount).Sum(assetId);
            return amount;
        }
    }
}
