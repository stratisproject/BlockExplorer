using System;
using NBitcoin;

namespace AzureIndexer.Api.Infrastructure
{
    public static class SenderRetriever
    {
        public static string GetAddressFromScript(this Script script, Network network)
        {
            PubKey payToPubKey = PayToPubkeyTemplate.Instance.ExtractScriptPubKeyParameters(script);

            if (payToPubKey != null)
            {
                var address = new uint160(payToPubKey.Hash.ToBytes());
                return address.ToBase58Address(network);
            }

            if (PayToPubkeyHashTemplate.Instance.CheckScriptPubKey(script))
            {
                var address = new uint160(PayToPubkeyHashTemplate.Instance.ExtractScriptPubKeyParameters(script).ToBytes());
                return address.ToBase58Address(network);
            }

            return null;
        }

        public static string ToBase58Address(this uint160 address, Network network)
        {
            return new BitcoinPubKeyAddress(new KeyId(address), network).ToString();
        }

        public static byte[] HexToByteArray(this string hex)
        {
            string toHex = hex;

            if (hex.StartsWith("0x"))
            {
                toHex = hex.Substring(2);
            }

            int numberChars = toHex.Length;
            byte[] bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(toHex.Substring(i, 2), 16);
            }

            return bytes;
        }
    }
}
