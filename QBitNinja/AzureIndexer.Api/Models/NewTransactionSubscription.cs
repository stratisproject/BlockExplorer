#if !CLIENT
namespace AzureIndexer.Api.Models
#else
namespace QBitNinja.Client.Models
#endif
{
    public class NewTransactionSubscription : Subscription
    {
        public NewTransactionSubscription()
        {
            Type = SubscriptionType.NewTransaction;
        }
    }
}
