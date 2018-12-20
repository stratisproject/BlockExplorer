using Microsoft.WindowsAzure.Storage.Table;
using NBitcoin;

namespace Stratis.Bitcoin.Features.AzureIndexer
{
    public interface IIndexed
    {
        ITableEntity CreateTableEntity();

        DynamicTableEntity CreateTableEntity(Network network);
    }
}
