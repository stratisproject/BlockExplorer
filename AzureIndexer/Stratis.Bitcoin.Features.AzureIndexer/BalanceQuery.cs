namespace Stratis.Bitcoin.Features.AzureIndexer
{
    using System;
    using System.Collections.Generic;
    using Microsoft.WindowsAzure.Storage.Table;
    using NBitcoin;
    using NBitcoin.DataEncoders;
    using Stratis.Bitcoin.Features.AzureIndexer.Helpers;

    public class UnconfirmedBalanceLocator : BalanceLocator
    {
        internal const int UnconfHeight = int.MaxValue - 1;

        public UnconfirmedBalanceLocator()
        {
            this.SeenDate = Utils.UnixTimeToDateTime(0);
        }

        public UnconfirmedBalanceLocator(DateTimeOffset seenDate, uint256 transactionId = null)
        {
            this.SeenDate = seenDate;
            this.TransactionId = transactionId;
        }

        public new static UnconfirmedBalanceLocator Parse(string str)
        {
            BalanceLocator result = BalanceLocator.Parse(str);
            if (!(result is UnconfirmedBalanceLocator))
            {
                throw InvalidUnconfirmedBalance();
            }

            return (UnconfirmedBalanceLocator)result;
        }

        private static FormatException InvalidUnconfirmedBalance()
        {
            return new FormatException("Invalid Unconfirmed Balance Locator");
        }

        public DateTimeOffset SeenDate { get; set; }

        public uint256 TransactionId { get; set; }

        internal static BalanceLocator ParseCore(string[] splitted, bool queryFormat)
        {
            if (splitted.Length < 2)
            {
                throw InvalidUnconfirmedBalance();
            }

            uint date = ParseUint(splitted[1]);
            uint256 transactionId = null;
            if (splitted.Length >= 3)
            {
                transactionId = new uint256(Encoders.Hex.DecodeData(splitted[2]), false);
            }

            return new UnconfirmedBalanceLocator(Utils.UnixTimeToDateTime(date), transactionId);
        }

        private static uint ParseUint(string str)
        {
            uint result;
            if (!uint.TryParse(str, out result))
            {
                throw InvalidUnconfirmedBalance();
            }

            return result;
        }

        public override string ToString(bool queryFormat)
        {
            var height = queryFormat ? Helper.HeightToString(UnconfHeight) : (UnconfHeight).ToString();
            var date = this.ToString(this.SeenDate);
            return height + "-" + date+ "-" + this.TransactionId;
        }

        private string ToString(DateTimeOffset SeenDate)
        {
            return Utils.DateTimeToUnixTime(SeenDate).ToString(Helper.format);
        }

        public override BalanceLocator Ceil()
        {
            UnconfirmedBalanceLocator result = this;
            if (this.TransactionId == null)
            {
                result = new UnconfirmedBalanceLocator(result.SeenDate, transactionId: _MinUInt256);
            }

            return result;
        }

        public override BalanceLocator Floor()
        {
            UnconfirmedBalanceLocator result = this;
            if (this.TransactionId == null)
            {
                result = new UnconfirmedBalanceLocator(result.SeenDate, transactionId: _MaxUInt256);
            }

            return result;
        }
    }

    public class ConfirmedBalanceLocator : BalanceLocator
    {
        public new static ConfirmedBalanceLocator Parse(string str)
        {
            BalanceLocator result = BalanceLocator.Parse(str);
            if (!(result is ConfirmedBalanceLocator))
            {
                throw new FormatException("Invalid Confirmed Balance Locator");
            }

            return (ConfirmedBalanceLocator)result;
        }

        internal static BalanceLocator ParseCore(int height, string[] splitted)
        {
            uint256 blockId = null;
            uint256 transactionId = null;

            if (splitted.Length >= 2)
            {
                blockId = new uint256(Encoders.Hex.DecodeData(splitted[1]), false);
            }

            if (splitted.Length >= 3)
            {
                transactionId = new uint256(Encoders.Hex.DecodeData(splitted[2]), false);
            }

            return new ConfirmedBalanceLocator(height, blockId, transactionId);
        }

        public ConfirmedBalanceLocator()
        {

        }

        public ConfirmedBalanceLocator(OrderedBalanceChange change)
            : this(change.Height, change.BlockId, change.TransactionId)
        {
        }

        public ConfirmedBalanceLocator(int height, uint256 blockId = null, uint256 transactionId = null)
        {
            if (height >= UnconfirmedBalanceLocator.UnconfHeight)
            {
                throw new ArgumentOutOfRangeException("height", "A confirmed block can't have such height");
            }

            this.Height = height;
            this.BlockHash = blockId;
            this.TransactionId = transactionId;
        }

        public int Height { get; set; }

        public uint256 BlockHash { get; set; }

        public uint256 TransactionId { get; set; }

        public override string ToString(bool queryFormat)
        {
            var height = queryFormat ? Helper.HeightToString(this.Height) : this.Height.ToString();
            return height + "-" + this.BlockHash + "-" + this.TransactionId;
        }

        public override BalanceLocator Floor()
        {
            ConfirmedBalanceLocator result = this;
            if (this.TransactionId == null)
            {
                result = new ConfirmedBalanceLocator(result.Height, result.BlockHash, transactionId: _MinUInt256);
            }

            if (this.BlockHash == null)
            {
                result = new ConfirmedBalanceLocator(result.Height, _MinUInt256, result.TransactionId);
            }

            return result;
        }

        public override BalanceLocator Ceil()
        {
            ConfirmedBalanceLocator result = this;
            if (this.TransactionId == null)
            {
                result = new ConfirmedBalanceLocator(result.Height, result.BlockHash, _MaxUInt256);
            }

            if (this.BlockHash == null)
            {
                result = new ConfirmedBalanceLocator(result.Height, _MaxUInt256, result.TransactionId);
            }

            return result;
        }
    }

    public class BalanceQuery
    {
        public BalanceQuery()
        {
            this.From = new UnconfirmedBalanceLocator();
            this.To = new ConfirmedBalanceLocator(0);
            this.ToIncluded = true;
            this.FromIncluded = true;
        }

        public BalanceLocator To
        {
            get;
            set;
        }

        public bool ToIncluded
        {
            get;
            set;
        }

        public BalanceLocator From
        {
            get;
            set;
        }

        public bool FromIncluded
        {
            get;
            set;
        }

        public bool RawOrdering
        {
            get;
            set;
        }

        public IEnumerable<int> PageSizes
        {
            get;
            set;
        }

        public TableQuery CreateTableQuery(BalanceId balanceId)
        {
            return this.CreateTableQuery(balanceId.PartitionKey, balanceId.ToString());
        }

        public TableQuery CreateTableQuery(string partitionId, string scope)
        {
            BalanceLocator from = this.From ?? new UnconfirmedBalanceLocator();
            BalanceLocator to = this.To ?? new ConfirmedBalanceLocator(0);
            var toIncluded = this.ToIncluded;
            var fromIncluded = this.FromIncluded;

            // Fix automatically if wrong order.
            if (!from.IsGreaterThan(to))
            {
                BalanceLocator temp = to;
                var temp2 = toIncluded;
                to = from;
                toIncluded = this.FromIncluded;
                from = temp;
                fromIncluded = temp2;
            }

            ////

            // Complete the balance locator is partial.
            from = fromIncluded ? from.Floor() : from.Ceil();
            to = toIncluded ? to.Ceil() : to.Floor();
            /////

            return new TableQuery()
            {
                FilterString =
                TableQuery.CombineFilters(
                                            TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionId),
                                            TableOperators.And,
                                            TableQuery.CombineFilters(
                                            TableQuery.GenerateFilterCondition("RowKey",
                                                    fromIncluded ? QueryComparisons.GreaterThanOrEqual : QueryComparisons.GreaterThan,
                                                    scope + "-" + from.ToString(true)),
                                                TableOperators.And,
                                                TableQuery.GenerateFilterCondition("RowKey",
                                                        toIncluded ? QueryComparisons.LessThanOrEqual : QueryComparisons.LessThan,
                                                        scope + "-" + to.ToString(true))
                                            ))
            };
        }
    }
}
