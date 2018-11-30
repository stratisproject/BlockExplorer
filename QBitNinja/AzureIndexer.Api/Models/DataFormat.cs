#if !CLIENT
namespace AzureIndexer.Api.Models
#else
namespace QBitNinja.Client.Models
#endif
{
    public enum DataFormat
    {
        Json,
        Raw
    }
}
