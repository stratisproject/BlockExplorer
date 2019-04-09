using System.Threading.Tasks;
using AzureIndexer.Api.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AzureIndexer.Api.ModelBinders
{
    public class BlockFeatureModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (!typeof(BlockFeature).IsAssignableFrom(bindingContext.ModelType))
            {
                return Task.CompletedTask;
            }

            ValueProviderResult val = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            string key = val.FirstValue;
            if (key == null)
            {
                bindingContext.Model = null;
                return Task.CompletedTask;
            }

            BlockFeature feature = BlockFeature.Parse(key);
            bindingContext.Model = feature;
            return Task.CompletedTask;
        }
    }
}
