using NBitcoin;

namespace Stratis.Features.AzureIndexer.Entities
{
    public class BroadcastedTransaction
    {
        public BroadcastedTransaction()
        {

        }
        public BroadcastedTransaction(Transaction tx)
        {
            Transaction = tx;
        }
        public int Tried
        {
            get;
            set;
        }
        public Transaction Transaction
        {
            get;
            set;
        }
    }
}
