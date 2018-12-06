namespace Stratis.Bitcoin.Features.AzureIndexer
{
    using System;
    using System.Text;
    using NBitcoin;
    using Stratis.Bitcoin.Features.AzureIndexer.DamienG.Security.Cryptography;

    public enum BalanceType
    {
        Wallet,
        Address
    }

    public class BalanceId
    {
        /// <summary>Balance id prefix to use when using a wallet id as the id.</summary>
        public const string WalletPrefix = "w$";

        /// <summary>Balance id prefix to use when using a script hash as the id.</summary>
        public const string HashPrefix = "h$";

        /// <summary>Maximum script size that can be used as-is in the id - otherwise the script hash is used.</summary>
        public const int MaxScriptSize = 79;

        /// <summary>The cached partition key.</summary>
        string _partitionKey;

        /// <summary>The balance id.</summary>
        string _balanceId;

        /// <summary>
        /// Initializes a new instance of the <see cref="BalanceId"/> class.
        /// Constructor for constructing a balance id from a wallet id.
        /// </summary>
        /// <param name="walletId">The wallet id to build the balance id from.</param>
        public BalanceId(string walletId)
        {
            this._balanceId = WalletPrefix + FastEncoder.Instance.EncodeData(Encoding.UTF8.GetBytes(walletId));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BalanceId"/> class.
        /// Constructor for constructing a balance id from a script.
        /// </summary>
        /// <param name="scriptPubKey">The script to build the balance id from.</param>
        /// <remarks>The MaxScriptSize determines the maximum script size that can be used as-is in the id - otherwise the script hash is used.</remarks>
        public BalanceId(Script scriptPubKey)
        {
            var pubKey = scriptPubKey.ToBytes(true);

            if (pubKey.Length > MaxScriptSize)
            {
                this._balanceId = HashPrefix + FastEncoder.Instance.EncodeData(scriptPubKey.Hash.ToBytes(true));
            }
            else
            {
                this._balanceId = FastEncoder.Instance.EncodeData(scriptPubKey.ToBytes(true));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BalanceId"/> class.
        /// Constructor for constructing a balance id from a destination.
        /// </summary>
        /// <param name="destination">The destination to build the balance id from.</param>
        /// <remarks>The MaxScriptSize determines the maximum script size that can be used as-is in the id - otherwise the script hash is used.</remarks>
        public BalanceId(IDestination destination)
            : this(destination.ScriptPubKey)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BalanceId"/> class.
        /// Private parameter-less constructor.
        /// </summary>
        private BalanceId()
        {
        }

        /// <summary>
        /// Recovers the wallet id from the internal id if it was constucted from a wallet id.
        /// </summary>
        /// <returns>The wallet id.</returns>
        /// <exception cref="System.InvalidOperationException">Thrown if the internal id was not constructed from a wallet id.</exception>
        public string GetWalletId()
        {
            if (this.Type != BalanceType.Wallet)
            {
                throw new InvalidOperationException("This balance id does not represent a wallet");
            }

            return Encoding.UTF8.GetString(FastEncoder.Instance.DecodeData(this._balanceId.Substring(WalletPrefix.Length)));
        }

        /// <summary>
        /// Determines if this object was constructed from a wallet id or a script.
        /// </summary>
        public BalanceType Type
        {
            get
            {
                return this._balanceId.StartsWith(WalletPrefix) ? BalanceType.Wallet : BalanceType.Address;
            }
        }

        /// <summary>
        /// Returns the partition key.
        /// </summary>
        public string PartitionKey
        {
            get
            {
                // Calculate the partition key if not calculated yet.
                return this._partitionKey ?? (this._partitionKey = Helper.GetPartitionKey(10, Crc32.Compute(this._balanceId)));
            }
        }

        /// <summary>
        /// Extracts the script that was used to construct this object, if any, and only if the script size does not exceed MaxScriptSize.
        /// </summary>
        /// <returns>The script that was used to construct this object - otherwise returns null.</returns>
        public Script ExtractScript()
        {
            return this.ContainsScript ? Script.FromBytesUnsafe(FastEncoder.Instance.DecodeData(this._balanceId)) : null;
        }

        /// <summary>
        /// Gets a value indicating whether determines if a script, with a size not exceeding MaxScriptSize, was used to construct this object.
        /// </summary>
        public bool ContainsScript => this._balanceId.Length >= 2 && this._balanceId[1] != '$';

        /// <summary>
        /// Returns the balance id.
        /// </summary>
        public override string ToString()
        {
            return this._balanceId;
        }

        /// <summary>
        /// Sets the balance id directly.
        /// </summary>
        /// <param name="balanceId">The balance id to set.</param>
        public static BalanceId Parse(string balanceId)
        {
            return new BalanceId()
            {
                _balanceId = balanceId
            };
        }
    }
}
