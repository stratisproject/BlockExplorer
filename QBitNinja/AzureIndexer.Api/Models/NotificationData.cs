using AzureIndexer.Api.JsonConverters;
using Newtonsoft.Json;
#if !CLIENT
#else
using QBitNinja.Client.JsonConverters;
#endif

#if !CLIENT
namespace AzureIndexer.Api.Models
#else
namespace QBitNinja.Client.Models
#endif
{

    [JsonConverter(typeof(EnumTypeJsonConverter))]
    [EnumType(SubscriptionType.NewBlock, typeof(NewBlockNotificationData))]
    [EnumType(SubscriptionType.NewTransaction, typeof(NewTransactionNotificationData))]
    public class NotificationData
    {
        public SubscriptionType Type
        {
            get;
            set;
        }
    }
}
