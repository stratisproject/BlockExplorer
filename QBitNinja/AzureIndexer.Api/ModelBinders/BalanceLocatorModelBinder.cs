using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Stratis.Features.AzureIndexer;

namespace AzureIndexer.Api.ModelBinders
{
    public class BalanceLocatorModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (!typeof(BalanceLocator).IsAssignableFrom(bindingContext.ModelType))
            {
                return Task.CompletedTask;
            }

            ValueProviderResult val = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            string key = val.FirstValue;
            bindingContext.Model = BalanceLocator.Parse(key);
            return Task.CompletedTask;
        }
    }
}
