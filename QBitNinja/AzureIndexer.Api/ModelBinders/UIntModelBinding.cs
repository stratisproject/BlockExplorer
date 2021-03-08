using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NBitcoin;

namespace AzureIndexer.Api.ModelBinders
{
    public class UInt256ModelBinding : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (!typeof(uint256).IsAssignableFrom(bindingContext.ModelType))
            {
                return Task.CompletedTask;
            }

            ValueProviderResult val = bindingContext.ValueProvider.GetValue(
                bindingContext.ModelName);

            string key = val.FirstValue;
            if(key == null)
            {
                bindingContext.Model = null;
                return Task.CompletedTask;
            }
            
            bindingContext.Model = uint256.Parse(key);
            if (bindingContext.Model.ToString().StartsWith(uint160.Zero.ToString()))
                throw new FormatException("Invalid hash format");

            return Task.CompletedTask;
        }
    }

    public class UInt160ModelBinding : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if(!typeof(uint160).IsAssignableFrom(bindingContext.ModelType))
            {
                return Task.CompletedTask;
            }

            ValueProviderResult val = bindingContext.ValueProvider.GetValue(
                bindingContext.ModelName);

            string key = val.FirstValue;
            if(key == null)
            {
                bindingContext.Model = null;
                return Task.CompletedTask;
            }

            bindingContext.Model = uint160.Parse(key);
            if (bindingContext.Model.ToString().StartsWith(uint160.Zero.ToString()))
                throw new FormatException("Invalid hash format");

            return Task.CompletedTask;
        }
    }
}
