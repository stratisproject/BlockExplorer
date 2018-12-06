namespace AzureIndexer.Api.Models.Response
{
    using System;
    using System.Collections.Generic;
    using System.Net.Sockets;
    using NBitcoin;
    using Newtonsoft.Json;
    using Stratis.Bitcoin.P2P.Protocol.Payloads;

    public class BroadcastResponseModel
    {
        [JsonProperty("success", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public bool? Success { get; set; }

        [JsonProperty("error", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public BroadcastErrorModel Error { get; set; }
    }

    public class BroadcastErrorModel
    {
        [JsonProperty("errorCode", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public RejectCode? ErrorCode { get; set; }

        [JsonProperty("reason", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string Reason { get; set; }
    }

    public class TransactionResponseModel
    {
        // [JsonProperty("transaction", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        // public Transaction Transaction { get; set; }

        [JsonProperty("transactionId", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public uint256 TransactionId { get; set; }

        [JsonProperty("isCoinbase", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsCoinbase { get; set; }

        [JsonProperty("block", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public BlockInformationModel Block { get; set; }

        [JsonProperty("spentCoins", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<CoinModel> SpentCoins { get; set; }

        [JsonProperty("receivedCoins", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<CoinModel> ReceivedCoins { get; set; }

        [JsonProperty("firstSeen", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? FirstSeen { get; set; }

        [JsonProperty("fees", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public Money Fees { get; set; }
    }

    public class TransactionModel
    {
        [JsonProperty("rBF", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public bool? RBF { get; set; }

        [JsonProperty("version", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public int? Version { get; set; }

        [JsonProperty("time", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public int? Time { get; set; }

        [JsonProperty("totalOut", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public MoneyModel TotalOut { get; set; }

        //[JsonProperty("lockTime", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        //public LockTimeModel LockTimeModel { get; set; }

        [JsonProperty("inputs", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<TxInModel> Inputs { get; set; }

        [JsonProperty("outputs", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<TxOutModel> Outputs { get; set; }

        [JsonProperty("isCoinBase", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsCoinBase { get; set; }

        [JsonProperty("isCoinStake", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsCoinStake { get; set; }

        [JsonProperty("hasWitness", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public bool? HasWitness { get; set; }
    }

    public class BlockInformationModel
    {
        [JsonProperty("blockId", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public uint256 BlockId { get; set; }

        [JsonProperty("blockHeader", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public BlockHeaderModel BlockHeader { get; set; }

        [JsonProperty("height", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public int? Height { get; set; }

        [JsonProperty("confirmations", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public int? Confirmations { get; set; }

        [JsonProperty("medianTimePast", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? MedianTimePast { get; set; }

        [JsonProperty("blockTime", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? BlockTime { get; set; }
    }

    public class CoinModel
    {
        [JsonProperty("amount", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public object Amount { get; set; }

        [JsonProperty("outpoint", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public OutPointModel Outpoint { get; set; }

        //[JsonProperty("txOut", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        //public TxOut TxOut { get; set; }
    }

    public class MoneyModel
    {
        [JsonProperty("satoshi", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public long? Satoshi { get; set; }
    }

    public class LockTimeModel
    {
        [JsonProperty("date", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? Date { get; set; }

        [JsonProperty("height", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public int? Height { get; set; }

        [JsonProperty("value", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public int? Value { get; set; }

        [JsonProperty("isHeightLock", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsHeightLock { get; set; }

        [JsonProperty("isTimeLock", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsTimeLock { get; set; }
    }

    public class TxInModel
    {
        [JsonProperty("sequence", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public SequenceModel Sequence { get; set; }

        [JsonProperty("prevOut", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public OutPointModel PrevOut { get; set; }

        [JsonProperty("scriptSig", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public ScriptModel ScriptSig { get; set; }

        [JsonProperty("witScript", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public WitScriptModel WitScript { get; set; }

        [JsonProperty("isFinal", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsFinal { get; set; }
    }

    public class TxOutModel
    {
        [JsonProperty("scriptPubKey", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public ScriptModel ScriptPubKey { get; set; }

        [JsonProperty("isEmpty", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsEmpty { get; set; }

        [JsonProperty("value", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public MoneyModel Value { get; set; }
    }

    public class BlockHeaderModel
    {
        [JsonProperty("currentVersion", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public int? CurrentVersion { get; set; }

        [JsonProperty("hashPrevBlock", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public uint256 HashPrevBlock { get; set; }

        [JsonProperty("time", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public int? Time { get; set; }

        [JsonProperty("bits", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public TargetModel Bits { get; set; }

        [JsonProperty("version", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public int? Version { get; set; }

        [JsonProperty("nonce", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public int? Nonce { get; set; }

        [JsonProperty("hashMerkleRoot", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public uint256 HashMerkleRoot { get; set; }

        [JsonProperty("isNull", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsNull { get; set; }

        [JsonProperty("blockTime", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? BlockTime { get; set; }
    }

    public class OutPointModel
    {
        [JsonProperty("isNull", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsNull { get; set; }

        [JsonProperty("hash", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public uint256 Hash { get; set; }

        [JsonProperty("n", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public int? N { get; set; }
    }

    public class SequenceModel
    {
        [JsonProperty("value", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public uint? Value { get; set; }

        [JsonProperty("isRelativeLock", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsRelativeLock { get; set; }

        [JsonProperty("isRBF", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsRBF { get; set; }

        // [JsonProperty("lockType", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        // public SequenceLockType? LockType { get; set; }
        // [JsonProperty("lockHeight", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        // public int? LockHeight { get; set; }
        // [JsonProperty("lockPeriod", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        // public string LockPeriod { get; set; }
    }

    public class ScriptModel
    {
        [JsonProperty("length", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public int? Length { get; set; }

        [JsonProperty("paymentScript", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public ScriptModel PaymentScript { get; set; }

        [JsonProperty("isPushOnly", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsPushOnly { get; set; }

        [JsonProperty("hasCanonicalPushes", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public bool? HasCanonicalPushes { get; set; }

        [JsonProperty("hash", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public ScriptIdModel Hash { get; set; }

        [JsonProperty("WitHash", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public WitScriptIdModel WitHash { get; set; }

        [JsonProperty("isUnspendable", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsUnspendable { get; set; }

        [JsonProperty("isValid", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsValid { get; set; }
    }

    public class WitScriptModel
    {
        [JsonProperty("pushes", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<byte[]> Pushes { get; set; }

        [JsonProperty("pushCount", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public int? PushCount { get; set; }
    }

    public class TargetModel
    {
        [JsonProperty("difficulty", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public double? Difficulty { get; set; }
    }

    public class ScriptIdModel
    {
        [JsonProperty("scriptPubKey", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public ScriptModel ScriptPubKey { get; set; }
    }

    public class WitScriptIdModel
    {
        [JsonProperty("scriptPubKey", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public ScriptModel ScriptPubKey { get; set; }
    }

    public class WalletModel
    {
        [JsonProperty("name", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
    }

    public class SubscriptionModel
    {
        [JsonProperty("id", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }

        [JsonProperty("url", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string Url { get; set; }

        [JsonProperty("type", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public SubscriptionType? Type { get; set; }
    }

    public class BalanceResponseModel
    {
        [JsonProperty("continuation", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string Continuation { get; set; }

        [JsonProperty("operations", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<BalanceOperationModel> Operations { get; set; }

        [JsonProperty(
            "ConflictedOperations",
            Required = Required.Default,
            NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<BalanceOperationModel> ConflictedOperations { get; set; }
    }

    public class BalanceOperationModel
    {
        [JsonProperty("amount", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public MoneyModel Amount { get; set; }

        [JsonProperty("confirmations", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public int? Confirmations { get; set; }

        [JsonProperty("height", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public int? Height { get; set; }

        [JsonProperty("blockId", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public uint256 BlockId { get; set; }

        [JsonProperty("transactionId", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public uint256 TransactionId { get; set; }

        [JsonProperty("receivedCoins", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<CoinModel> ReceivedCoins { get; set; }

        [JsonProperty("spentCoins", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<CoinModel> SpentCoins { get; set; }

        [JsonProperty("firstSeen", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? FirstSeen { get; set; }
    }

    public class InsertWalletAddress
    {
        [JsonProperty("mergePast", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public bool? MergePast { get; set; }

        [JsonProperty("address", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public DestinationModel Address { get; set; }

        [JsonProperty("redeemScript", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public ScriptModel RedeemScript { get; set; }

        [JsonProperty("userData", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public object UserData { get; set; }
    }

    public class DestinationModel
    {
        [JsonProperty("scriptPubKey", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public ScriptModel ScriptPubKey { get; set; }
    }

    public class WalletAddressModel
    {
        [JsonProperty("WalletName", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string WalletName { get; set; }

        [JsonProperty("address", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public DestinationModel Address { get; set; }

        [JsonProperty("redeemScript", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public ScriptModel RedeemScript { get; set; }

        [JsonProperty("hDKeySet", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public HDKeySetModel HDKeySet { get; set; }

        [JsonProperty("hDKey", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public HDKeyDataModel HDKey { get; set; }

        [JsonProperty("userData", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public object UserData { get; set; }
    }

    public class HDKeySetModel
    {
        [JsonProperty("name", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("extPubKeys", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<BitcoinExtPubKeyModel> ExtPubKeys { get; set; }

        [JsonProperty("signatureCount", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public int? SignatureCount { get; set; }

        [JsonProperty("path", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public KeyPathModel Path { get; set; }

        [JsonProperty("lexicographicOrder", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public bool? LexicographicOrder { get; set; }

        [JsonProperty("p2SH", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public bool? P2SH { get; set; }
    }

    public class HDKeyDataModel
    {
        [JsonProperty("path", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public KeyPathModel Path { get; set; }

        [JsonProperty("address", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public BitcoinAddressModel Address { get; set; }

        [JsonProperty("extPubKeys", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<BitcoinExtPubKeyModel> ExtPubKeys { get; set; }

        [JsonProperty("redeemScript", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public ScriptModel RedeemScript { get; set; }

        [JsonProperty("scriptPubKey", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public ScriptModel ScriptPubKey { get; set; }
    }

    public class BitcoinExtPubKeyModel
    {
        [JsonProperty("extPubKey", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public ExtPubKeyModel ExtPubKey { get; set; }

        [JsonProperty("type", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public Base58Type? Type { get; set; }

        [JsonProperty("scriptPubKey", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public ScriptModel ScriptPubKey { get; set; }

        //[JsonProperty("network", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        //public NetworkModel Network { get; set; }
    }

    public class KeyPathModel
    {
        [JsonProperty("indexes", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<int> Indexes { get; set; }

        [JsonProperty("parent", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public KeyPathModel Parent { get; set; }

        [JsonProperty("isHardened", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsHardened { get; set; }
    }

    public class BitcoinAddressModel
    {
        [JsonProperty("scriptPubKey", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public ScriptModel ScriptPubKey { get; set; }

        //[JsonProperty("network", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        //public NetworkModel Network { get; set; }
    }

    public class ExtPubKeyModel
    {
        [JsonProperty("depth", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public int? Depth { get; set; }

        [JsonProperty("child", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public int? Child { get; set; }

        [JsonProperty("isHardened", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsHardened { get; set; }

        [JsonProperty("pubKey", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public PubKeyModel PubKey { get; set; }

        [JsonProperty("chainCode", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public byte[] ChainCode { get; set; }

        [JsonProperty("fingerprint", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public byte[] Fingerprint { get; set; }

        [JsonProperty("scriptPubKey", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public ScriptModel ScriptPubKey { get; set; }
    }

    public class NetworkModel
    {
        [JsonProperty("magicBytesArray", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public byte[] MagicBytesArray { get; set; }

        [JsonProperty(
            "MaxTimeOffsetSeconds",
            Required = Required.Default,
            NullValueHandling = NullValueHandling.Ignore)]
        public int? MaxTimeOffsetSeconds { get; set; }

        [JsonProperty("maxTipAge", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public int? MaxTipAge { get; set; }

        [JsonProperty("minTxFee", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public long? MinTxFee { get; set; }

        [JsonProperty("fallbackFee", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public long? FallbackFee { get; set; }

        [JsonProperty("minRelayTxFee", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public long? MinRelayTxFee { get; set; }

        [JsonProperty("rPCPort", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public int? RPCPort { get; set; }

        [JsonProperty("defaultPort", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public int? DefaultPort { get; set; }

        [JsonProperty("consensus", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public ConsensusModel Consensus { get; set; }

        [JsonProperty("name", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("additionalNames", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<string> AdditionalNames { get; set; }

        [JsonProperty("coinTicker", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string CoinTicker { get; set; }

        [JsonProperty("rootFolderName", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string RootFolderName { get; set; }

        [JsonProperty(
            "defaultConfigFilename",
            Required = Required.Default,
            NullValueHandling = NullValueHandling.Ignore)]
        public string DefaultConfigFilename { get; set; }

        [JsonProperty("seedNodes", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<NetworkAddressModel> SeedNodes { get; set; }

        [JsonProperty("dNSSeeds", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<DNSSeedDataModel> DNSSeeds { get; set; }

        [JsonProperty("checkpoints", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public IDictionary<string, CheckpointInfoModel> Checkpoints { get; set; }

        [JsonProperty("base58Prefixes", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<byte[]> Base58Prefixes { get; set; }

        [JsonProperty("bech32Encoders", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<Bech32EncoderModel> Bech32Encoders { get; set; }

        [JsonProperty("magic", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public int? Magic { get; set; }

        [JsonProperty("magicBytes", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public byte[] MagicBytes { get; set; }

        [JsonProperty("genesisTime", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public int? GenesisTime { get; set; }

        [JsonProperty("genesisNonce", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public int? GenesisNonce { get; set; }

        [JsonProperty("genesisBits", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public int? GenesisBits { get; set; }

        [JsonProperty("genesisVersion", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public int? GenesisVersion { get; set; }

        [JsonProperty("genesisReward", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public MoneyModel GenesisReward { get; set; }

        [JsonProperty("genesisHash", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public uint256 GenesisHash { get; set; }
    }

    public class PubKeyModel
    {
        [JsonProperty("hash", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public KeyIdModel Hash { get; set; }

        [JsonProperty("WitHash", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public WitKeyIdModel WitHash { get; set; }

        [JsonProperty("isCompressed", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsCompressed { get; set; }

        [JsonProperty("scriptPubKey", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public ScriptModel ScriptPubKey { get; set; }
    }

    public class ConsensusModel
    {
        [JsonProperty("coinbaseMaturity", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public long? CoinbaseMaturity { get; set; }

        [JsonProperty("premineReward", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public MoneyModel PremineReward { get; set; }

        [JsonProperty("premineHeight", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public long? PremineHeight { get; set; }

        [JsonProperty("proofOfWorkReward", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public MoneyModel ProofOfWorkReward { get; set; }

        [JsonProperty("proofOfStakeReward", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public MoneyModel ProofOfStakeReward { get; set; }

        [JsonProperty("maxReorgLength", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public int? MaxReorgLength { get; set; }

        [JsonProperty("maxMoney", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public long? MaxMoney { get; set; }

        [JsonProperty("options", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public ConsensusOptionsModel Options { get; set; }

        [JsonProperty("buriedDeployments", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public object BuriedDeployments { get; set; }

        [JsonProperty("bIP9Deployments", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public IBIP9DeploymentsArray BIP9Deployments { get; set; }

        [JsonProperty(
            "SubsidyHalvingInterval",
            Required = Required.Default,
            NullValueHandling = NullValueHandling.Ignore)]
        public int? SubsidyHalvingInterval { get; set; }

        [JsonProperty(
            "MajorityEnforceBlockUpgrade",
            Required = Required.Default,
            NullValueHandling = NullValueHandling.Ignore)]
        public int? MajorityEnforceBlockUpgrade { get; set; }

        [JsonProperty(
            "MajorityRejectBlockOutdated",
            Required = Required.Default,
            NullValueHandling = NullValueHandling.Ignore)]
        public int? MajorityRejectBlockOutdated { get; set; }

        [JsonProperty("majorityWindow", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public int? MajorityWindow { get; set; }

        [JsonProperty("bIP34Hash", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public uint256 BIP34Hash { get; set; }

        [JsonProperty("powLimit", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public TargetModel PowLimit { get; set; }

        [JsonProperty("powTargetTimespan", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string PowTargetTimespan { get; set; }

        [JsonProperty("powTargetSpacing", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string PowTargetSpacing { get; set; }

        [JsonProperty(
            "PowAllowMinDifficultyBlocks",
            Required = Required.Default,
            NullValueHandling = NullValueHandling.Ignore)]
        public bool? PowAllowMinDifficultyBlocks { get; set; }

        [JsonProperty("powNoRetargeting", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public bool? PowNoRetargeting { get; set; }

        [JsonProperty("hashGenesisBlock", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public uint256 HashGenesisBlock { get; set; }

        [JsonProperty("minimumChainWork", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public uint256 MinimumChainWork { get; set; }

        [JsonProperty(
            "MinerConfirmationWindow",
            Required = Required.Default,
            NullValueHandling = NullValueHandling.Ignore)]
        public int? MinerConfirmationWindow { get; set; }

        [JsonProperty(
            "RuleChangeActivationThreshold",
            Required = Required.Default,
            NullValueHandling = NullValueHandling.Ignore)]
        public int? RuleChangeActivationThreshold { get; set; }

        [JsonProperty("coinType", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public int? CoinType { get; set; }

        [JsonProperty("proofOfStakeLimit", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public BigIntegerModel ProofOfStakeLimit { get; set; }

        [JsonProperty("proofOfStakeLimitV2", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public BigIntegerModel ProofOfStakeLimitV2 { get; set; }

        [JsonProperty("lastPOWBlock", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public int? LastPOWBlock { get; set; }

        [JsonProperty("isProofOfStake", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsProofOfStake { get; set; }

        [JsonProperty("defaultAssumeValid", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public uint256 DefaultAssumeValid { get; set; }

        [JsonProperty("consensusFactory", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public object ConsensusFactory { get; set; }

        [JsonProperty(
            "HeaderValidationRules",
            Required = Required.Default,
            NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<object> HeaderValidationRules { get; set; }

        [JsonProperty(
            "IntegrityValidationRules",
            Required = Required.Default,
            NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<object> IntegrityValidationRules { get; set; }

        [JsonProperty(
            "PartialValidationRules",
            Required = Required.Default,
            NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<object> PartialValidationRules { get; set; }

        [JsonProperty("fullValidationRules", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<object> FullValidationRules { get; set; }
    }

    public class NetworkAddressModel
    {
        [JsonProperty("service", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public long? Service { get; set; }

        [JsonProperty("ago", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string Ago { get; set; }

        [JsonProperty("endpoint", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public IPEndPointModel Endpoint { get; set; }

        [JsonProperty("time", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? Time { get; set; }
    }

    public class DNSSeedDataModel
    {
        [JsonProperty("name", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("host", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string Host { get; set; }
    }

    public class CheckpointInfoModel
    {
        [JsonProperty("hash", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public uint256 Hash { get; set; }

        [JsonProperty("stakeModifierV2", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public uint256 StakeModifierV2 { get; set; }
    }

    public class Bech32EncoderModel
    {
        [JsonProperty("humanReadablePart", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public byte[] HumanReadablePart { get; set; }
    }

    public class KeyIdModel
    {
        [JsonProperty("scriptPubKey", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public ScriptModel ScriptPubKey { get; set; }
    }

    public class WitKeyIdModel
    {
        [JsonProperty("scriptPubKey", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public ScriptModel ScriptPubKey { get; set; }

        [JsonProperty("WitScriptPubKey", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public ScriptModel WitScriptPubKey { get; set; }
    }

    public class ConsensusOptionsModel
    {
        [JsonProperty("maxBlockBaseSize", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public int? MaxBlockBaseSize { get; set; }

        [JsonProperty("maxBlockWeight", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public int? MaxBlockWeight { get; set; }

        [JsonProperty(
            "MaxBlockSerializedSize",
            Required = Required.Default,
            NullValueHandling = NullValueHandling.Ignore)]
        public int? MaxBlockSerializedSize { get; set; }

        [JsonProperty("WitnessScaleFactor", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public int? WitnessScaleFactor { get; set; }

        [JsonProperty("maxStandardVersion", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public int? MaxStandardVersion { get; set; }

        [JsonProperty("maxStandardTxWeight", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public int? MaxStandardTxWeight { get; set; }

        [JsonProperty("maxBlockSigopsCost", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public int? MaxBlockSigopsCost { get; set; }

        [JsonProperty(
            "MaxStandardTxSigopsCost",
            Required = Required.Default,
            NullValueHandling = NullValueHandling.Ignore)]
        public int? MaxStandardTxSigopsCost { get; set; }
    }

    public class IBIP9DeploymentsArray
    {
        [JsonProperty("length", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public int? Length { get; set; }
    }

    public class BigIntegerModel
    {
        [JsonProperty("bitCount", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public int? BitCount { get; set; }

        [JsonProperty("bitLength", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public int? BitLength { get; set; }

        [JsonProperty("intValue", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public int? IntValue { get; set; }

        [JsonProperty("longValue", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public long? LongValue { get; set; }

        [JsonProperty("signValue", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public int? SignValue { get; set; }
    }

    public class IPEndPointModel
    {
        [JsonProperty("addressFamily", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public AddressFamily? AddressFamily { get; set; }

        [JsonProperty("address", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public IPAddressModel Address { get; set; }

        [JsonProperty("port", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public int? Port { get; set; }
    }

    public class IPAddressModel
    {
        [JsonProperty("addressFamily", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public AddressFamily? AddressFamily { get; set; }

        [JsonProperty("scopeId", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public long? ScopeId { get; set; }

        [JsonProperty("isIPv6Multicast", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsIPv6Multicast { get; set; }

        [JsonProperty("isIPv6LinkLocal", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsIPv6LinkLocal { get; set; }

        [JsonProperty("isIPv6SiteLocal", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsIPv6SiteLocal { get; set; }

        [JsonProperty("isIPv6Teredo", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsIPv6Teredo { get; set; }

        [JsonProperty("isIPv4MappedToIPv6", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsIPv4MappedToIPv6 { get; set; }

        [JsonProperty("address", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public long? Address { get; set; }
    }

    public class KeySetDataModel
    {
        [JsonProperty("keySet", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public HDKeySetModel KeySet { get; set; }

        [JsonProperty("state", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public HDKeyStateModel State { get; set; }
    }

    public class HDKeyStateModel
    {
        [JsonProperty("nextUnused", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public int? NextUnused { get; set; }
    }

    public class BalanceSummaryModel
    {
        [JsonProperty("unConfirmed", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public BalanceSummaryDetailsModel UnConfirmed { get; set; }

        [JsonProperty("confirmed", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public BalanceSummaryDetailsModel Confirmed { get; set; }

        [JsonProperty("spendable", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public BalanceSummaryDetailsModel Spendable { get; set; }

        [JsonProperty("immature", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public BalanceSummaryDetailsModel Immature { get; set; }

        [JsonProperty("locator", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public object Locator { get; set; }

        [JsonProperty("olderImmature", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public int? OlderImmature { get; set; }

        [JsonProperty("cacheHit", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public CacheHit? CacheHit { get; set; }
    }

    public class BalanceSummaryDetailsModel
    {
        [JsonProperty("transactionCount", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public int? TransactionCount { get; set; }

        [JsonProperty("amount", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public MoneyModel Amount { get; set; }

        [JsonProperty("received", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public MoneyModel Received { get; set; }

        [JsonProperty("assets", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<AssetBalanceSummaryDetailsModel> Assets { get; set; }
    }

    public class AssetBalanceSummaryDetailsModel
    {
        [JsonProperty("asset", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public BitcoinAssetIdModel Asset { get; set; }

        [JsonProperty("quantity", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public long? Quantity { get; set; }

        [JsonProperty("received", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public long? Received { get; set; }
    }

    public class BitcoinAssetIdModel
    {
        [JsonProperty("assetId", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public object AssetId { get; set; }

        [JsonProperty("type", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public Base58Type? Type { get; set; }

        //[JsonProperty("network", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        //public NetworkModel Network { get; set; }
    }

    public class WhatIsBlockHeaderModel
    {
        [JsonProperty("version", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string Version { get; set; }

        [JsonProperty("hash", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public uint256 Hash { get; set; }

        [JsonProperty("previous", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public uint256 Previous { get; set; }

        [JsonProperty("time", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? Time { get; set; }

        [JsonProperty("nonce", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public int? Nonce { get; set; }

        [JsonProperty("hashMerkelRoot", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public uint256 HashMerkelRoot { get; set; }

        [JsonProperty("bits", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string Bits { get; set; }

        [JsonProperty("difficulty", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public double? Difficulty { get; set; }
    }

    public class VersionStatsResponseModel
    {
        [JsonProperty("last144", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public VersionStatsModel Last144 { get; set; }

        [JsonProperty("last2016", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public VersionStatsModel Last2016 { get; set; }

        [JsonProperty("sincePeriodStart", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public VersionStatsModel SincePeriodStart { get; set; }
    }

    public class VersionStatsModel
    {
        [JsonProperty("total", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public int? Total { get; set; }

        [JsonProperty("fromHeight", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public int? FromHeight { get; set; }

        [JsonProperty("toHeight", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public int? ToHeight { get; set; }

        [JsonProperty("stats", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public ICollection<VersionStatsItemModel> Stats { get; set; }
    }

    public class VersionStatsItemModel
    {
        [JsonProperty("version", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public int? Version { get; set; }

        [JsonProperty("proposal", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public string Proposal { get; set; }

        [JsonProperty("count", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public int? Count { get; set; }

        [JsonProperty("percentage", Required = Required.Default, NullValueHandling = NullValueHandling.Ignore)]
        public double? Percentage { get; set; }
    }
}