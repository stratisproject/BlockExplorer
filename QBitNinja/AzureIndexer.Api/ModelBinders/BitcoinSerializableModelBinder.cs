using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NBitcoin;

namespace AzureIndexer.Api.ModelBinders
{
    public class BitcoinSerializableModelBinder : IModelBinder
    {
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if(typeof(uint160).IsAssignableFrom(bindingContext.ModelType))
            {
                await new UInt160ModelBinding().BindModelAsync(bindingContext);
            }

            if(typeof(uint256).IsAssignableFrom(bindingContext.ModelType))
            {
                await new UInt256ModelBinding().BindModelAsync(bindingContext);
            }

            if (!typeof(IBitcoinSerializable).IsAssignableFrom(bindingContext.ModelType))
            {
                return;
            }

            ValueProviderResult val = bindingContext.ValueProvider.GetValue(
                bindingContext.ModelName);

            string key = val.FirstValue;
            if (key == null)
            {
                bindingContext.Model = null;
                return;
            }

            try
            {
                bindingContext.Model = Activator.CreateInstance(bindingContext.ModelType, key);
            }
            catch(TargetInvocationException ex)
            {
                throw ex.InnerException;
            }

            if (bindingContext.Model is uint256 || bindingContext.Model is uint160)
            {
                if (bindingContext.Model.ToString().StartsWith(uint160.Zero.ToString()))
                    throw new FormatException("Invalid hash format");
            }
        }
    }
}
