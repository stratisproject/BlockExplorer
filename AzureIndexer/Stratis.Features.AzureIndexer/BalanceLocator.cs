namespace Stratis.Features.AzureIndexer
{
    using System;
    using NBitcoin;
    using Stratis.Features.AzureIndexer.Helpers;

    public class BalanceLocator
    {
        static BalanceLocator()
        {
            _MinUInt256 = new uint256(new byte[32]);
            var b = new byte[32];
            for (int i = 0; i < b.Length; i++)
            {
                b[i] = 0xFF;
            }

            _MaxUInt256 = new uint256(b);
        }

        internal static uint256 _MinUInt256;
        internal static uint256 _MaxUInt256;

        public static BalanceLocator Parse(string str)
        {
            return Parse(str, false);
        }

        public static BalanceLocator Parse(string str, bool queryFormat)
        {
            string[] splitted = str.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
            if (splitted.Length == 0)
            {
                throw new FormatException("Invalid BalanceLocator string");
            }

            var height = queryFormat ? Helper.StringToHeight(splitted[0]) : int.Parse(splitted[0]);

            if (height == UnconfirmedBalanceLocator.UnconfHeight)
            {
                return UnconfirmedBalanceLocator.ParseCore(splitted, queryFormat);
            }
            else
            {
                return ConfirmedBalanceLocator.ParseCore(height, splitted);
            }
        }


        public override string ToString()
        {
            return this.ToString(false);
        }

        public virtual string ToString(bool queryFormat)
        {
            return this.ToString();
        }

        public bool IsGreaterThan(BalanceLocator to)
        {
            var result = string.Compare(this.ToString(true), to.ToString(true));
            return result < 1;
        }

        public virtual BalanceLocator Floor()
        {
            return this;
        }

        public virtual BalanceLocator Ceil()
        {
            return this;
        }
    }
}