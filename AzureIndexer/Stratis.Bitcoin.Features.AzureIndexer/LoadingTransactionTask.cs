namespace Stratis.Bitcoin.Features.AzureIndexer
{
    using System.Threading.Tasks;

    public class LoadingTransactionTask
    {
        public Task<bool> Loaded { get; set; }

        public OrderedBalanceChange Change { get; set; }
    }
}
