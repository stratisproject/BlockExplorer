namespace Stratis.Bitcoin.Features.AzureIndexer.IndexTasks
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;

    public abstract class IndexTableEntitiesTaskBase<TIndexed> : IndexTask<TIndexed>
    {
        private int indexedEntities = 0;

        public IndexTableEntitiesTaskBase(IndexerConfiguration configuration, ILoggerFactory loggerFactory)
            : base(configuration, loggerFactory)
        {
        }

        public int IndexedEntities => this.indexedEntities;

        protected override int PartitionSize => 100;

        protected override Task EnsureSetup()
        {
            return this.GetCloudTable().CreateIfNotExistsAsync();
        }

        protected abstract CloudTable GetCloudTable();

        protected abstract ITableEntity ToTableEntity(TIndexed item);

         protected override void IndexCore(string partitionName, IEnumerable<TIndexed> items)
        {
            TableBatchOperation transactionsBatch = new TableBatchOperation();
            foreach (TIndexed item in items)
            {
                transactionsBatch.Add(TableOperation.InsertOrReplace(this.ToTableEntity(item)));
            }

            CloudTable table = this.GetCloudTable();

            TableRequestOptions options = new TableRequestOptions()
            {
                PayloadFormat = TablePayloadFormat.Json,
                MaximumExecutionTime = this.Timeout,
                ServerTimeout = this.Timeout,
            };

            OperationContext context = new OperationContext();
            Queue<TableBatchOperation> batches = new Queue<TableBatchOperation>();
            batches.Enqueue(transactionsBatch);

            while (batches.Count > 0)
            {
                transactionsBatch = batches.Dequeue();

                try
                {
                    //Stopwatch watch = new Stopwatch();
                    //watch.Start();

                    if (transactionsBatch.Count > 1)
                    {
                        table.ExecuteBatchAsync(transactionsBatch, options, context).GetAwaiter().GetResult();
                    }
                    else
                    {
                        if (transactionsBatch.Count == 1)
                        {
                            table.ExecuteAsync(transactionsBatch[0], options, context).GetAwaiter().GetResult();
                        }
                    }

                    Interlocked.Add(ref this.indexedEntities, transactionsBatch.Count);
                }
                catch (Exception ex)
                {
                    if (this.IsError413(ex) /* Request too large */ || Helper.IsError(ex, "OperationTimedOut"))
                    {
                        // Reduce the size of all batches to half the size of the offending batch.
                        var maxSize = Math.Max(1, transactionsBatch.Count / 2);
                        var workDone = false;
                        var newBatches = new Queue<TableBatchOperation>();

                        for (/* starting with the current batch */; ; transactionsBatch = batches.Dequeue())
                        {
                            for (; transactionsBatch.Count > maxSize;)
                            {
                                newBatches.Enqueue(this.ToBatch(transactionsBatch.Take(maxSize).ToList()));
                                transactionsBatch = this.ToBatch(transactionsBatch.Skip(maxSize).ToList());
                                workDone = true;
                            }

                            if (transactionsBatch.Count > 0)
                            {
                                newBatches.Enqueue(transactionsBatch);
                            }

                            if (batches.Count == 0)
                            {
                                break;
                            }
                        }

                        batches = newBatches;

                        // Nothing could be done?
                        if (!workDone)
                        {
                           // throw;
                        }
                    }
                    else if (Helper.IsError(ex, "EntityTooLarge"))
                    {
                        TableOperation op = this.GetFaultyOperation(ex, transactionsBatch);
                        DynamicTableEntity entity = (DynamicTableEntity)this.GetEntity(op);
                        byte[] serialized = entity.Serialize();

                        this.Configuration
                            .GetBlocksContainer()
                            .GetBlockBlobReference(entity.GetFatBlobName())
                            .UploadFromByteArrayAsync(serialized, 0, serialized.Length).GetAwaiter().GetResult();

                        entity.MakeFat(serialized.Length);
                        batches.Enqueue(transactionsBatch);
                    }
                    else
                    {
                        IndexerTrace.ErrorWhileImportingEntitiesToAzure(transactionsBatch.Select(b => this.GetEntity(b)).ToArray(), ex);
                        batches.Enqueue(transactionsBatch);
                        throw;
                    }
                }
            }
        }

        //protected override void IndexCore(string partitionName, IEnumerable<TIndexed> items, string partitionName2, IEnumerable<IIndexed> scItems)
        //{
        //    var transactionsBatch = new TableBatchOperation();
        //    var smartContractsBatch = new TableBatchOperation();
        //    foreach (var item in items)
        //    {
        //        transactionsBatch.Add(TableOperation.InsertOrReplace(this.ToTableEntity(item)));
        //    }

        //    foreach (var scItem in scItems)
        //    {
        //        smartContractsBatch.Add(TableOperation.InsertOrReplace(scItem.CreateTableEntity()));
        //    }

        //    CloudTable table = this.GetCloudTable();
        //    CloudTable scTable = this.Configuration.GetSmartContactTable();

        //    var options = new TableRequestOptions()
        //    {
        //        PayloadFormat = TablePayloadFormat.Json,
        //        MaximumExecutionTime = this._Timeout,
        //        ServerTimeout = this._Timeout,
        //    };

        //    var context = new OperationContext();
        //    Queue<TableBatchOperation> batches = new Queue<TableBatchOperation>();
        //    batches.Enqueue(transactionsBatch);



        //    while (batches.Count > 0)
        //    {
        //        transactionsBatch = batches.Dequeue();

        //        try
        //        {
        //            Stopwatch watch = new Stopwatch();
        //            watch.Start();

        //            if (transactionsBatch.Count > 1)
        //            {
        //                table.ExecuteBatchAsync(transactionsBatch, options, context).GetAwaiter().GetResult();
        //            }
        //            else
        //            {
        //                if (transactionsBatch.Count == 1)
        //                {
        //                    table.ExecuteAsync(transactionsBatch[0], options, context).GetAwaiter().GetResult();
        //                }
        //            }

        //            Interlocked.Add(ref _IndexedEntities, transactionsBatch.Count);
        //        }
        //        catch (Exception ex)
        //        {
        //            if (IsError413(ex) /* Request too large */ || Helper.IsError(ex, "OperationTimedOut"))
        //            {
        //                // Reduce the size of all batches to half the size of the offending batch.
        //                int maxSize = Math.Max(1, transactionsBatch.Count / 2);
        //                bool workDone = false;
        //                Queue<TableBatchOperation> newBatches = new Queue<TableBatchOperation>();

        //                for (/* starting with the current batch */; ; transactionsBatch = batches.Dequeue())
        //                {
        //                    for (; transactionsBatch.Count > maxSize; )
        //                    {
        //                        newBatches.Enqueue(ToBatch(transactionsBatch.Take(maxSize).ToList()));
        //                        transactionsBatch = ToBatch(transactionsBatch.Skip(maxSize).ToList());
        //                        workDone = true;
        //                    }

        //                    if (transactionsBatch.Count > 0)
        //                        newBatches.Enqueue(transactionsBatch);

        //                    if (batches.Count == 0)
        //                        break;
        //                }

        //                batches = newBatches;

        //                // Nothing could be done?
        //                if (!workDone) throw;
        //            }
        //            else if (Helper.IsError(ex, "EntityTooLarge"))
        //            {
        //                var op = GetFaultyOperation(ex, transactionsBatch);
        //                var entity = (DynamicTableEntity)GetEntity(op);
        //                var serialized = entity.Serialize();

        //                Configuration
        //                    .GetBlocksContainer()
        //                    .GetBlockBlobReference(entity.GetFatBlobName())
        //                    .UploadFromByteArrayAsync(serialized, 0, serialized.Length).GetAwaiter().GetResult();

        //                entity.MakeFat(serialized.Length);
        //                batches.Enqueue(transactionsBatch);
        //            }
        //            else
        //            {
        //                IndexerTrace.ErrorWhileImportingEntitiesToAzure(transactionsBatch.Select(b => GetEntity(b)).ToArray(), ex);
        //                batches.Enqueue(transactionsBatch);
        //                throw;
        //            }
        //        }
        //    }
        //}

        protected ITableEntity GetEntity(TableOperation op)
        {
            return (ITableEntity)typeof(TableOperation).GetProperty("Entity", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .GetValue(op);
        }

        protected bool IsError413(Exception ex)
        {
            StorageException storage = (StorageException)ex;
            if (storage == null)
            {
                return false;
            }

            return storage.RequestInformation != null && storage.RequestInformation.HttpStatusCode == 413;
        }

        protected TableOperation GetFaultyOperation(Exception ex, TableBatchOperation batch)
        {
            if (batch.Count == 1)
            {
                return batch[0];
            }

            StorageException storage = ex as StorageException;
            if (storage == null)
            {
                return null;
            }

            if (storage.RequestInformation != null
                && storage.RequestInformation.ExtendedErrorInformation != null)
            {
                Match match = Regex.Match(storage.RequestInformation.ExtendedErrorInformation.ErrorMessage, "[0-9]*");
                if (match.Success)
                {
                    return batch[int.Parse(match.Value)];
                }
            }

            return null;
        }

        protected TableBatchOperation ToBatch(List<TableOperation> ops)
        {
            TableBatchOperation op = new TableBatchOperation();
            foreach (TableOperation operation in ops)
            {
                op.Add(operation);
            }

            return op;
        }
    }
}