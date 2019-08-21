namespace Stratis.Bitcoin.Features.AzureIndexer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NBitcoin;
    using Stratis.Bitcoin.Features.AzureIndexer.Entities;
    using Stratis.Bitcoin.Features.AzureIndexer.Internal;

    public class WalletRuleEntryCollection : IEnumerable<WalletRuleEntry>
    {
        private readonly List<WalletRuleEntry> walletRules;
        private readonly HashSet<Tuple<string,string>> walletsIds = new HashSet<Tuple<string,string>>();

        private readonly MultiValueDictionary<string, WalletRuleEntry> entriesByWallet;
        private readonly ILookup<string, WalletRuleEntry> entriesByWalletLookup;

        private readonly MultiValueDictionary<Script, WalletRuleEntry> entriesByAddress;
        private readonly ILookup<Script, WalletRuleEntry> entriesByAddressLookup;

        internal WalletRuleEntryCollection(IEnumerable<WalletRuleEntry> walletRules)
        {
            if (walletRules == null)
            {
                throw new ArgumentNullException("walletRules");
            }

            this.walletRules = new List<WalletRuleEntry>();
            this.entriesByWallet = new MultiValueDictionary<string, WalletRuleEntry>();
            this.entriesByWalletLookup = this.entriesByWallet.AsLookup();

            this.entriesByAddress = new MultiValueDictionary<Script, WalletRuleEntry>();
            this.entriesByAddressLookup = this.entriesByAddress.AsLookup();

            foreach (WalletRuleEntry rule in walletRules)
            {
                this.Add(rule);
            }
        }

        public int Count => this.walletRules.Count;

        public bool Add(WalletRuleEntry entry)
        {
            if (!this.walletsIds.Add(this.GetId(entry)))
            {
                return false;
            }

            this.walletRules.Add(entry);
            this.entriesByWallet.Add(entry.WalletId, entry);

            if (entry.Rule is ScriptRule rule)
            {
                this.entriesByAddress.Add(rule.ScriptPubKey, entry);
            }

            return true;
        }

        private Tuple<string,string> GetId(WalletRuleEntry entry)
        {
            return Tuple.Create(entry.WalletId, entry.Rule.Id);
        }

        public void AddRange(IEnumerable<WalletRuleEntry> entries)
        {
            foreach (WalletRuleEntry entry in entries)
            {
                this.Add(entry);
            }
        }

        public IEnumerable<WalletRuleEntry> GetRulesForWallet(string walletName)
        {
            return this.entriesByWalletLookup[walletName];
        }

        public IEnumerable<WalletRuleEntry> GetRulesFor(IDestination destination)
        {
            return this.GetRulesFor(destination.ScriptPubKey);
        }

        public IEnumerable<WalletRuleEntry> GetRulesFor(Script script)
        {
            return this.entriesByAddressLookup[script];
        }

        public IEnumerator<WalletRuleEntry> GetEnumerator()
        {
            return this.walletRules.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
