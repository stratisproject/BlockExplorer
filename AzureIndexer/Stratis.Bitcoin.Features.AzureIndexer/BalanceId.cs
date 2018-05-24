using NBitcoin;
using System;
using System.Text;
using Stratis.Bitcoin.Features.AzureIndexer.DamienG.Security.Cryptography;

namespace Stratis.Bitcoin.Features.AzureIndexer
{
    public enum BalanceType
    {
        Wallet,
        Address
    }

    public class BalanceId
    {
        /// <summary>Balance id prefix to use when using a wallet id as the id.</summary>
        const string WalletPrefix = "w$";

        /// <summary>Balance id prefix to use when using a script hash as the id.</summary>
        const string HashPrefix = "h$";

        /// <summary>Maximum script size that can be used as-is in the id - otherwise the script hash is used.</summary>
        internal const int MaxScriptSize = 79;

        /// <summary>The cached partition key.</summary>
        string partitionKey;

        /// <summary>The balance id.</summary>
        string balanceId;

        /// <summary>
        /// Constructor for constructing a balance id from a wallet id.
        /// </summary>
        /// <param name="walletId">The wallet id to build the balance id from.</param>
        public BalanceId(string walletId)
        {
            balanceId = WalletPrefix + FastEncoder.Instance.EncodeData(Encoding.UTF8.GetBytes(walletId));
        }

        /// <summary>
        /// Constructor for constructing a balance id from a script.
        /// </summary>
        /// <param name="scriptPubKey">The script to build the balance id from.</param>
        /// <remarks>The MaxScriptSize determines the maximum script size that can be used as-is in the id - otherwise the script hash is used.</remarks>
        public BalanceId(Script scriptPubKey)
        {
            var pubKey = scriptPubKey.ToBytes(true);

            if (pubKey.Length > MaxScriptSize)
                balanceId = HashPrefix + FastEncoder.Instance.EncodeData(scriptPubKey.Hash.ToBytes(true));
            else
                balanceId = FastEncoder.Instance.EncodeData(scriptPubKey.ToBytes(true));
        }

        /// <summary>
        /// Constructor for constructing a balance id from a destination.
        /// </summary>
        /// <param name="destination">The destination to build the balance id from.</param>
        /// <remarks>The MaxScriptSize determines the maximum script size that can be used as-is in the id - otherwise the script hash is used.</remarks>
        public BalanceId(IDestination destination)
            : this(destination.ScriptPubKey)
        {
        }

        /// <summary>
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
            if (Type != BalanceType.Wallet)
                throw new InvalidOperationException("This balance id does not represent a wallet");

            return Encoding.UTF8.GetString(FastEncoder.Instance.DecodeData(balanceId.Substring(WalletPrefix.Length)));
        }

        /// <summary>
        /// Determines if this object was constructed from a wallet id or a script.
        /// </summary>
        public BalanceType Type
        {
            get
            {
                return balanceId.StartsWith(WalletPrefix) ? BalanceType.Wallet : BalanceType.Address;
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
                if (partitionKey == null)
                    partitionKey = Helper.GetPartitionKey(10, Crc32.Compute(balanceId));

                return partitionKey;
            }
        }

        /// <summary>
        /// Extracts the script that was used to construct this object, if any, and only if the script size does not exceed MaxScriptSize.
        /// </summary>
        /// <returns>The script that was used to construct this object - otherwise returns null.</returns>
        public Script ExtractScript()
        {
            return ContainsScript?Script.FromBytesUnsafe(FastEncoder.Instance.DecodeData(balanceId)):null;
        }

        /// <summary>
        /// Determines if a script, with a size not exceeding MaxScriptSize, was used to construct this object.
        /// </summary>
        public bool ContainsScript => balanceId.Length >= 2 && balanceId[1] != '$';

        /// <summary>
        /// Returns the balance id.
        /// </summary>
        public override string ToString()
        {
            return balanceId;
        }

        /// <summary>
        /// Sets the balance id directly.
        /// </summary>
        /// <param name="balanceId">The balance id to set.</param>
        public static BalanceId Parse(string balanceId)
        {
            return new BalanceId()
            {
                balanceId = balanceId
            };
        }
    }
}
