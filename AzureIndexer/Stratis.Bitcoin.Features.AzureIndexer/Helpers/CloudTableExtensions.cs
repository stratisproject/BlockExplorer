namespace Stratis.Bitcoin.Features.AzureIndexer.Helpers
{
    using System.Collections.Generic;
    using System.Threading;
    using Microsoft.WindowsAzure.Storage.Table;

    public static class CloudTableExtensions
    {
        // From: https://stackoverflow.com/questions/24234350/how-to-execute-an-azure-table-storage-query-async-client-version-4-0-1
        public static IEnumerable<DynamicTableEntity> ExecuteQuery(this CloudTable table, TableQuery query, CancellationToken ct = default(CancellationToken))
        {
            TableContinuationToken token = null;

            do
            {
                TableQuerySegment seg = table.ExecuteQuerySegmentedAsync(query, token).GetAwaiter().GetResult();
                token = seg.ContinuationToken;
                foreach (DynamicTableEntity tableEntity in seg)
                    yield return tableEntity;
            } while (token != null && !ct.IsCancellationRequested);
        }
    }
}