#if !CLIENT
namespace AzureIndexer.Api.Models
#else
namespace QBitNinja.Client.Models
#endif
{
    public class NewBlockSubscription : Subscription
    {
        public NewBlockSubscription()
        {
            Type = SubscriptionType.NewBlock;
        }
    }
}
