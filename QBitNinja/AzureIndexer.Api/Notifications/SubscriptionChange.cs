using AzureIndexer.Api.Models;

namespace AzureIndexer.Api.Notifications
{
    public class SubscriptionChange
    {
        public SubscriptionChange()
        {

        }
        public SubscriptionChange(Subscription subscription, bool added)
        {
            Subscription = subscription;
            Added = added;
        }
        public Subscription Subscription
        {
            get;
            set;
        }
        public bool Added
        {
            get;
            set;
        }
    }
}
