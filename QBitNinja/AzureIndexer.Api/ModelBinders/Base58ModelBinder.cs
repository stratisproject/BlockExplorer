using System;
using System.Threading.Tasks;
using AzureIndexer.Api.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NBitcoin;

namespace AzureIndexer.Api.ModelBinders
{
    public class Base58ModelBinder : IModelBinder
    {
        #region IModelBinder Members

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (!
                (typeof(Base58Data).IsAssignableFrom(bindingContext.ModelType) ||
                typeof(IDestination).IsAssignableFrom(bindingContext.ModelType)))
            {
                return Task.CompletedTask;
            }

            ValueProviderResult val = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            string key = val.FirstValue;

            var data = Network.Parse(key, bindingContext.ActionContext.HttpContext.Request.GetConfiguration().Indexer.Network);
            if (!bindingContext.ModelType.IsInstanceOfType(data))
            {
                throw new FormatException("Invalid base58 type");
            }

            bindingContext.Model = data;
            return Task.CompletedTask;
        }

        #endregion
    }
}
