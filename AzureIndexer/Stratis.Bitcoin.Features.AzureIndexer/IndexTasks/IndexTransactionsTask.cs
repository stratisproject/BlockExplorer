namespace Stratis.Bitcoin.Features.AzureIndexer.IndexTasks
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using Microsoft.Extensions.Logging;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;
    using NBitcoin;
    using Stratis.Bitcoin.Features.AzureIndexer.Entities;
    using Stratis.Bitcoin.Features.AzureIndexer.Helpers;

    public class IndexTransactionsTask : IndexTableEntitiesTaskBase<TransactionEntry.Entity>
    {
        private readonly ILogger logger;
        private readonly IndexerConfiguration config;
        private IndexTableEntitiesTaskBase<TransactionEntry.Entity> _indexTableEntitiesTaskBaseImplementation;

        public IndexTransactionsTask(IndexerConfiguration configuration, ILoggerFactory loggerFactory)
            : base(configuration, loggerFactory)
        {
            this.config = configuration;
            this.logger = loggerFactory.CreateLogger(this.GetType().FullName);
        }

        protected override void ProcessBlock(BlockInfo block, BulkImport<TransactionEntry.Entity> transactionsBulk, Network network, BulkImport<SmartContactEntry.Entity> smartContractBulk = null)
        {
            foreach (Transaction transaction in block.Block.Transactions)
            {
                TransactionEntry.Entity indexed = new TransactionEntry.Entity(null, transaction, block.BlockId, network, this.IsSC);

                if (this.IsSC)
                {
                    if (indexed.HasSmartContract && smartContractBulk != null)
                    {
                        SmartContactEntry.Entity scEntity = new SmartContactEntry.Entity(indexed);
                        smartContractBulk.Add(scEntity.PartitionKey, scEntity);
                    }
                }

                transactionsBulk.Add(indexed.PartitionKey, indexed);
            }
        }

        protected override void IndexCore(string txPartitionName, IEnumerable<TransactionEntry.Entity> txItems)
        {
            TableBatchOperation transactionsBatch = new TableBatchOperation();
            TableBatchOperation smartContractsBatch = new TableBatchOperation();
            TableBatchOperation smartContractDetailsBatch = new TableBatchOperation();
            foreach (TransactionEntry.Entity item in txItems)
            {
                transactionsBatch.Add(TableOperation.InsertOrReplace(this.ToTableEntity(item)));
                if (this.IsSC && item.HasSmartContract)
                {
                    this.logger.LogInformation($"SmartContract detected in Tx: {item.TxId}");
                    smartContractsBatch.Add(TableOperation.InsertOrReplace(item.GetChildTableEntity()));

                    if (item.GetChild().GetChild() != null)
                    {
                        smartContractDetailsBatch.Add(TableOperation.InsertOrReplace(item.GetChild().GetChildTableEntity()));
                    }
                }
            }

            CloudTable txTable = this.GetCloudTable();
            CloudTable scTable = this.GetSmartContractCloudTable();
            CloudTable scdTable = this.GetSmartContractCloudDetailTable();

            TableRequestOptions options = new TableRequestOptions()
            {
                PayloadFormat = TablePayloadFormat.Json,
                MaximumExecutionTime = this.Timeout,
                ServerTimeout = this.Timeout,
            };

            OperationContext context = new OperationContext();
            var txBatches = new Queue<TableBatchOperation>();
            txBatches.Enqueue(transactionsBatch);

            Queue<TableBatchOperation> scBatches = new Queue<TableBatchOperation>();
            if (this.IsSC)
            {
                scBatches.Enqueue(smartContractsBatch);
            }

            this.SendEntities(ref transactionsBatch, txTable, options, context, ref txBatches);

            if (this.IsSC && smartContractDetailsBatch.Count > 0)
            {
                this.SendEntities(ref smartContractsBatch, scTable, options, context, ref scBatches);
                if (smartContractDetailsBatch.Count > 0)
                {
                    Queue<TableBatchOperation> scdBatches = new Queue<TableBatchOperation>();
                    scdBatches.Enqueue(smartContractDetailsBatch);
                    this.SendEntities(ref smartContractDetailsBatch, scdTable, options, context, ref scdBatches);
                }
            }
        }

        private void SendEntities(ref TableBatchOperation transactionsBatch, CloudTable txTable, TableRequestOptions options, OperationContext context, ref Queue<TableBatchOperation> txBatches)
        {
            while (txBatches.Count > 0)
            {
                transactionsBatch = txBatches.Dequeue();

                try
                {
                    Stopwatch watch = new Stopwatch();
                    watch.Start();

                    if (transactionsBatch.Count > 1)
                    {
                        txTable.ExecuteBatchAsync(transactionsBatch, options, context).GetAwaiter().GetResult();
                    }
                    else
                    {
                        if (transactionsBatch.Count == 1)
                        {
                            txTable.ExecuteAsync(transactionsBatch[0], options, context).GetAwaiter().GetResult();
                        }
                    }

                    var indexedEntities = this.IndexedEntities;
                    Interlocked.Add(ref indexedEntities, transactionsBatch.Count);
                }
                catch (Exception ex)
                {
                    this.ExceptionHandler(ex, ref transactionsBatch, ref txBatches);
                }
            }
        }

        private void ExceptionHandler(Exception ex, ref TableBatchOperation transactionsBatch, ref Queue<TableBatchOperation> txBatches)
        {
            if (this.IsError413(ex) /* Request too large */ || Helper.IsError(ex, "OperationTimedOut"))
            {
                // Reduce the size of all batches to half the size of the offending batch.
                var maxSize = Math.Max(1, transactionsBatch.Count / 2);
                var workDone = false;
                var newBatches = new Queue<TableBatchOperation>();

                for ( /* starting with the current batch */; ; transactionsBatch = txBatches.Dequeue())
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

                    if (txBatches.Count == 0)
                    {
                        break;
                    }
                }

                txBatches = newBatches;

                // Nothing could be done?
                if (!workDone)
                {
                    throw new NotSupportedException();
                }
            }
            else if (Helper.IsError(ex, "EntityTooLarge"))
            {
                TableOperation op = this.GetFaultyOperation(ex, transactionsBatch);
                DynamicTableEntity entity = (DynamicTableEntity) this.GetEntity(op);
                byte[] serialized = entity.Serialize();

                this.Configuration.GetBlocksContainer()
                    .GetBlockBlobReference(entity.GetFatBlobName())
                    .UploadFromByteArrayAsync(serialized, 0, serialized.Length)
                    .GetAwaiter()
                    .GetResult();

                entity.MakeFat(serialized.Length);
                txBatches.Enqueue(transactionsBatch);
            }
            else
            {
                IndexerTrace.ErrorWhileImportingEntitiesToAzure(transactionsBatch.Select(b => this.GetEntity(b)).ToArray(), ex);
                txBatches.Enqueue(transactionsBatch);
                throw new NotSupportedException();
            }
        }

        protected CloudTable GetSmartContractCloudTable()
        {
            return this.Configuration.GetSmartContactTable();
        }

        protected CloudTable GetSmartContractCloudDetailTable()
        {
            return this.Configuration.GetSmartContactDetailTable();
        }

        protected override CloudTable GetCloudTable()
        {
            return this.Configuration.GetTransactionTable();
        }

        protected override ITableEntity ToTableEntity(TransactionEntry.Entity indexed)
        {
            return indexed.CreateTableEntity(this.config.Network);
        }

        protected ITableEntity ToTableEntity(SmartContactEntry.Entity indexed)
        {
            return indexed.CreateTableEntity(this.config.Network);
        }
    }
}
