using AzureIndexer.Api.Models;

namespace AzureIndexer.Api.Infrastructure
{
    using System;
    using NBitcoin;
    using NBitcoin.DataEncoders;
    using Stratis.Bitcoin.Features.AzureIndexer;

    public static class StringExtensions
    {
        public static BalanceId ToBalanceId(this string value, Network network)
        {
            if (value.Length > 3 && value.Length < 5000 && value.StartsWith("0x"))
            {
                return new BalanceId(new Script(Encoders.Hex.DecodeData(value.Substring(2))));
            }

            if (value.Length > 3 && value.Length < 5000 && value.StartsWith("W-"))
            {
                return new BalanceId(value.Substring(2));
            }

            var data = Network.Parse(value, network);
            if (!(data is IDestination))
            {
                throw new FormatException("Invalid base58 type");
            }

            return new BalanceId((IDestination)data);
        }

        public static BlockFeature ToBlockFeature(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            var feature = BlockFeature.Parse(value);
            return feature;
        }

        public static BalanceLocator ToBalanceLocator(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            var feature = BalanceLocator.Parse(value);
            return feature;
        }
    }
}