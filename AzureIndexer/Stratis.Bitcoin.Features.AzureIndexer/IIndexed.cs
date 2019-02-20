namespace Stratis.Bitcoin.Features.AzureIndexer
{
    using Microsoft.WindowsAzure.Storage.Table;
    using NBitcoin;

    public interface IIndexed
    {
        ITableEntity CreateTableEntity();

        DynamicTableEntity CreateTableEntity(Network network);

        ITableEntity GetChildTableEntity();

        IIndexed GetChild();
    }
}
