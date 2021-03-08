using System;
using System.Threading.Tasks;
using AzureIndexer.Api.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NBitcoin;
using NBitcoin.DataEncoders;
using Stratis.Features.AzureIndexer;

namespace AzureIndexer.Api.ModelBinders
{
    public class BalanceIdModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (!typeof(BalanceId).IsAssignableFrom(bindingContext.ModelType))
            {
                return Task.CompletedTask;
            }

            ValueProviderResult val = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            string key = val.FirstValue;
            if (key.Length > 3 && key.Length < 5000 && key.StartsWith("0x"))
            {
                bindingContext.Model = new BalanceId(new Script(Encoders.Hex.DecodeData(key.Substring(2))));
                return Task.CompletedTask;
            }

            if (key.Length > 3 && key.Length < 5000 && key.StartsWith("W-"))
            {
                bindingContext.Model = new BalanceId(key.Substring(2));
                return Task.CompletedTask;
            }

            var data = Network.Parse(key, bindingContext.HttpContext.Request.GetConfiguration().Indexer.Network);
            if (!(data is IDestination))
            {
                throw new FormatException("Invalid base58 type");
            }

            if (data is BitcoinColoredAddress)
            {
                bindingContext.HttpContext.Items["BitcoinColoredAddress"] = true;
            }

            bindingContext.Model = new BalanceId((IDestination)data);
            return Task.CompletedTask;
        }
    }
}
