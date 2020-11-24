namespace Stratis.Features.AzureIndexer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NBitcoin;

    public class BalanceSheet
    {
        public BalanceSheet(IEnumerable<OrderedBalanceChange> changes, ChainIndexer chain)
        {
            this.ChainIndexer = chain ?? throw new ArgumentNullException("chain");

            List<OrderedBalanceChange> all = changes
                        .Where(c => c.SpentCoins != null) // Remove line whose previous coins have not been loaded could not be deduced
                        .Where(c => c.MempoolEntry || this.ChainIndexer.GetHeader(c.BlockId) != null) // Take only mempool entry, or confirmed one
                        .Where(c => !(c.IsCoinbase && c.MempoolEntry)) // There is no such thing as a Coinbase unconfirmed, by definition a coinbase appear in a block
                        .ToList();
            Dictionary<uint256, OrderedBalanceChange> confirmed = all.Where(o => o.BlockId != null).ToDictionary(o => o.TransactionId);
            var unconfirmed = new Dictionary<uint256, OrderedBalanceChange>();

            foreach (OrderedBalanceChange item in all.Where(o => o.MempoolEntry && !confirmed.ContainsKey(o.TransactionId)))
            {
                unconfirmed.AddOrReplace(item.TransactionId, item);
            }

            this.Prunable = all.Where(o => o.BlockId == null && confirmed.ContainsKey(o.TransactionId)).ToList();
            this.All = all.Where(o =>
                (unconfirmed.ContainsKey(o.TransactionId) || confirmed.ContainsKey(o.TransactionId))
                    &&
                    !(o.BlockId == null && confirmed.ContainsKey(o.TransactionId))
                ).ToList();
            this.Confirmed = this.All.Where(o => o.BlockId != null && confirmed.ContainsKey(o.TransactionId)).ToList();
            this.Unconfirmed = this.All.Where(o => o.BlockId == null && unconfirmed.ContainsKey(o.TransactionId)).ToList();
        }

        public ChainIndexer ChainIndexer { get; }

        public List<OrderedBalanceChange> Unconfirmed { get; }

        public List<OrderedBalanceChange> Confirmed { get; }

        public List<OrderedBalanceChange> All { get; }

        public List<OrderedBalanceChange> Prunable { get; }
    }
}
