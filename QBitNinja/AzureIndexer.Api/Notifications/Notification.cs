using AzureIndexer.Api.Models;
using Stratis.Bitcoin.Utilities.JsonConverters;

namespace AzureIndexer.Api.Notifications
{
    public class Notification
    {
        public Subscription Subscription
        {
            get;
            set;
        }

        public NotificationData Data
        {
            get;
            set;
        }

        public override string ToString()
        {
            return Serializer.ToString(this);
        }

        public int Tried
        {
            get;
            set;
        }
    }
}
