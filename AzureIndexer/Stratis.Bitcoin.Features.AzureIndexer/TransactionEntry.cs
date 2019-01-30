namespace Stratis.Bitcoin.Features.AzureIndexer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CSharpFunctionalExtensions;
    using Microsoft.WindowsAzure.Storage.Table;
    using NBitcoin;
    using NBitcoin.OpenAsset;
    using Stratis.Bitcoin.Networks;
    using Stratis.SmartContracts.CLR;
    using Stratis.SmartContracts.CLR.Compilation;
    using Stratis.SmartContracts.CLR.Decompilation;
    using Stratis.SmartContracts.CLR.Serialization;
    using Stratis.SmartContracts.Core;

    public partial class TransactionEntry
    {
        public ColoredTransaction ColoredTransaction { get; set; }

        public DateTimeOffset? MempoolDate { get; set; }

        public bool HasSmartContract { get; set; }

        public Money Fees
        {
            get
            {
                if (this.SpentCoins == null || this.Transaction == null)
                {
                    return null;
                }

                if (this.Transaction.IsCoinBase)
                {
                    return Money.Zero;
                }

                return this.SpentCoins.Select(o => o.TxOut.Value).Sum() - this.Transaction.TotalOut;
            }
        }

        public uint256[] BlockIds { get; internal set; }

        public uint256 TransactionId { get; internal set; }

        public Transaction Transaction { get; internal set; }

        /// <summary>
        /// Coins spent during the transaction, can be null if the indexer miss parent transactions
        /// </summary>
        public List<Spendable> SpentCoins { get; set; }

        public DateTimeOffset FirstSeen { get; set; }

        public Network Network { get; set; }

        internal TransactionEntry(Entity[] entities)
        {
            this.TransactionId = entities[0].TxId;
            this.BlockIds = entities.Select(e => e.BlockId).Where(b => b != null).ToArray();
            this.MempoolDate = entities.Where(e => e.Type == Entity.TransactionEntryType.Mempool)
                                  .Select(e => new DateTimeOffset?(e.Timestamp))
                                  .Min();
            this.FirstSeen = this.MempoolDate != null ? this.MempoolDate.Value :
                                    entities.Where(e => e.Type == Entity.TransactionEntryType.ConfirmedTransaction)
                                  .Select(e => new DateTimeOffset?(e.Timestamp))
                                  .Min().Value;

            Entity loadedEntity = entities.FirstOrDefault(e => e.Transaction != null && e.IsLoaded);
            if (loadedEntity == null)
            {
                loadedEntity = entities.FirstOrDefault(e => e.Transaction != null);
            }

            if (loadedEntity != null)
            {
                this.Transaction = loadedEntity.Transaction;
                if (loadedEntity.Transaction.IsCoinBase)
                {
                    this.SpentCoins = new List<Spendable>();
                }
                else if (loadedEntity.IsLoaded)
                {
                    this.SpentCoins = new List<Spendable>();
                    for (var i = 0; i < this.Transaction.Inputs.Count; i++)
                    {
                        this.SpentCoins.Add(new Spendable(this.Transaction.Inputs[i].PrevOut, loadedEntity.PreviousTxOuts[i]));
                    }
                }

                this.HasSmartContract = loadedEntity.HasSmartContract;
            }

            Entity coloredLoadedEntity = entities.FirstOrDefault(e => e.ColoredTransaction != null);
            if (coloredLoadedEntity != null)
            {
                this.ColoredTransaction = coloredLoadedEntity.ColoredTransaction;
            }
        }

        public class Entity : IIndexed
        {
            public Entity(uint256 txId, Transaction tx, uint256 blockId, Network network)
            {
                if (txId == null)
                {
                    txId = tx.GetHash();
                }

                this.Network = network;
                this.Timestamp = DateTimeOffset.UtcNow;
                this.TxId = txId;
                this.Transaction = tx;
                this.BlockId = blockId;
                this.Type = blockId == null ? TransactionEntryType.Mempool : TransactionEntryType.ConfirmedTransaction;

                this.CheckForSmartContract(tx);

                this.Child = new SmartContactEntry.Entity(this);
            }

            public Entity(uint256 txId)
            {
                if (txId == null)
                {
                    throw new ArgumentNullException("txId");
                }

                this.Timestamp = DateTimeOffset.UtcNow;
                this.TxId = txId;
            }

            public Entity(DynamicTableEntity entity, Network network)
            {
                this.Network = network;
                string[] splitted = entity.RowKey.Split(new string[] { "-" }, StringSplitOptions.None);
                this._PartitionKey = entity.PartitionKey;
                this.Timestamp = entity.Timestamp;
                this.TxId = uint256.Parse(splitted[0]);
                this.Type = this.GetType(splitted[1]);
                if (splitted.Length >= 3 && splitted[2] != string.Empty)
                {
                    this.BlockId = uint256.Parse(splitted[2]);
                }

                byte[] bytes = Helper.GetEntityProperty(entity, "a");
                if (bytes != null && bytes.Length != 0)
                {
                    this.Transaction = network.Consensus.ConsensusFactory.CreateTransaction(bytes);
                }

                bytes = Helper.GetEntityProperty(entity, "b");
                if (bytes != null && bytes.Length != 0)
                {
                    this.ColoredTransaction = new ColoredTransaction();
                    this.ColoredTransaction.ReadWrite(bytes);
                }

                this._PreviousTxOuts = Helper.DeserializeList<TxOut>(Helper.GetEntityProperty(entity, "c"));

                byte[] timestamp = Helper.GetEntityProperty(entity, "d");
                if (timestamp != null && timestamp.Length == 8)
                {
                    this.Timestamp = new DateTimeOffset((long)ToUInt64(timestamp, 0), TimeSpan.Zero);
                }

                try
                {
                    this.HasSmartContract = Convert.ToBoolean(Helper.GetEntityProperty(entity, "HasSmartContract"));
                }
                catch
                {
                    this.HasSmartContract = false;
                }
            }

            public Entity(uint256 txId, ColoredTransaction colored)
            {
                if (txId == null)
                {
                    throw new ArgumentNullException("txId");
                }

                this.TxId = txId;
                this.ColoredTransaction = colored;
                this.Type = TransactionEntryType.Colored;
            }

            public SmartContactEntry.Entity Child { get; set; }

            public static ulong ToUInt64(byte[] value, int index)
            {
                return value[index]
                       + ((ulong)value[index + 1] << 8)
                       + ((ulong)value[index + 2] << 16)
                       + ((ulong)value[index + 3] << 24)
                       + ((ulong)value[index + 4] << 32)
                       + ((ulong)value[index + 5] << 40)
                       + ((ulong)value[index + 6] << 48)
                       + ((ulong)value[index + 7] << 56);
            }

            public Transaction Transaction { get; set; }

            readonly List<TxOut> _PreviousTxOuts = new List<TxOut>();

            public List<TxOut> PreviousTxOuts => this._PreviousTxOuts;

            public TransactionEntryType Type { get; set; }

            public bool HasSmartContract { get; set; }

            public Network Network { get; set; }

            public ContractTxData ContractTxData { get; set; }

            public byte[] ContractByteCode { get; set; }

            public string ContractCode { get; set; }

            public enum TransactionEntryType
            {
                Mempool,
                ConfirmedTransaction,
                Colored
            }

            string _PartitionKey;

            public string PartitionKey
            {
                get
                {
                    if (this._PartitionKey == null && this.TxId != null)
                    {
                        byte[] b = this.TxId.ToBytes();
                        this._PartitionKey = Helper.GetPartitionKey(10, new[] { b[0], b[1] }, 0, 2);
                    }

                    return this._PartitionKey;
                }
            }

            public DateTimeOffset Timestamp { get; set; }

            public uint256 BlockId { get; set; }

            public uint256 TxId { get; set; }

            public ColoredTransaction ColoredTransaction { get; set; }

            public string TypeLetter => this.Type == TransactionEntryType.Colored ? "c" :
                this.Type == TransactionEntryType.ConfirmedTransaction ? "b" :
                this.Type == TransactionEntryType.Mempool ? "m" : "?";

            public DynamicTableEntity CreateTableEntity(Network network)
            {
                var entity = new DynamicTableEntity
                {
                    ETag = "*", PartitionKey = this.PartitionKey, RowKey = this.TxId + "-" + this.TypeLetter+ "-" + this.BlockId
                };
                if (this.Transaction != null)
                {
                    Helper.SetEntityProperty(entity, "a", this.Transaction.ToBytes(network.Consensus.ConsensusFactory));
                }

                if (this.ColoredTransaction != null)
                {
                    Helper.SetEntityProperty(entity, "b", this.ColoredTransaction.ToBytes());
                }

                Helper.SetEntityProperty(entity, "c", Helper.SerializeList(this.PreviousTxOuts));
                Helper.SetEntityProperty(entity, "d", Utils.ToBytes((ulong)this.Timestamp.UtcTicks, true));

                this.CheckForSmartContract(this.Transaction);

                entity.Properties.AddOrReplace("HasSmartContract", new EntityProperty(this.HasSmartContract));

                return entity;
            }

            public ITableEntity GetChildTableEntity()
            {
                return this.Child.CreateTableEntity(this.Network);
            }

            public IIndexed GetChild()
            {
                return this.Child;
            }

            /// <summary>
            /// Check Tx for containing SmartContract in it.
            /// </summary>
            /// <param name="transaction">Transaction</param>
            /// <returns>True or False</returns>
            private bool CheckForSmartContract(Transaction transaction)
            {
                var smartContractSerializer = new CallDataSerializer(new ContractPrimitiveSerializer(this.Network));
                var csharpDecompiler = new CSharpContractDecompiler();

                foreach (TxOut transactionOutput in transaction.Outputs)
                {
                    Result<ContractTxData> contractTxDataResult = smartContractSerializer.Deserialize(transactionOutput.ScriptPubKey.ToBytes());
                    if (contractTxDataResult.IsSuccess)
                    {
                        this.HasSmartContract = true;
                        this.ContractTxData = contractTxDataResult.Value;

                        if (!this.ContractTxData.IsCreateContract)
                        {
                            continue;
                        }

                        Result<IContractModuleDefinition> contractDecompileResult = ContractDecompiler.GetModuleDefinition(this.ContractTxData.ContractExecutionCode);
                        if (contractDecompileResult.IsSuccess)
                        {
                            this.ContractByteCode = contractDecompileResult.Value.ToByteCode().Value;
                            if (this.ContractByteCode.Length > 0)
                            {
                                Result<string> csharpDecompileResult =
                                    csharpDecompiler.GetSource(this.ContractByteCode);
                                if (csharpDecompileResult.IsSuccess)
                                {
                                    this.ContractCode = csharpDecompileResult.Value;
                                }
                            }
                        }
                    }
                }

                return this.HasSmartContract;
            }

            public TransactionEntryType GetType(string letter)
            {
                switch (letter[0])
                {
                    case 'c':
                        return TransactionEntryType.Colored;
                    case 'b':
                        return TransactionEntryType.ConfirmedTransaction;
                    case 'm':
                        return TransactionEntryType.Mempool;
                    default:
                        return TransactionEntryType.Mempool;
                }
            }

            public bool IsLoaded => this.Transaction != null &&
                                    (this.Transaction.IsCoinBase || (this.PreviousTxOuts.Count == this.Transaction.Inputs.Count));

            public ITableEntity CreateTableEntity()
            {
                throw new NotImplementedException();
            }
        }
    }
}
