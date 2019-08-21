using System;
using System.Collections.Generic;
using AzureIndexer.Api.Models;
using CommonServiceLocator;
using Microsoft.AspNetCore.Http;
using Stratis.Bitcoin.Features.AzureIndexer;
using Stratis.Bitcoin.Features.AzureIndexer.Entities;
using Stratis.Bitcoin.Utilities.JsonConverters;

namespace AzureIndexer.Api.Infrastructure
{
    public static class Extensions
    {
        public static WalletRuleEntry CreateWalletRuleEntry(this WalletAddress address)
        {
            return new WalletRuleEntry(address.WalletName, new ScriptRule
            {
                CustomData = Serializer.ToString(address),
                ScriptPubKey = address.ScriptPubKey,
                RedeemScript = address.RedeemScript
            });

        }

        public static QBitNinjaConfiguration GetConfiguration(this HttpRequest ctx)
        {
            return ServiceLocator.Current.GetInstance<QBitNinjaConfiguration>();
        }

        public static T MinElement<T>(this IEnumerable<T> input, Func<T, int> predicate)
        {
            int min = int.MaxValue;
            T element = default(T);

            foreach (var el in input)
            {
                var val = predicate(el);
                if (val < min)
                {
                    min = predicate(el);
                    element = el;
                }
            }
            return element;
        }
    }
}
