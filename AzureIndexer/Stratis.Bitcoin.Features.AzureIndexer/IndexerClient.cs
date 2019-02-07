// ReSharper disable ArrangeThisQualifier
namespace Stratis.Bitcoin.Features.AzureIndexer
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using Microsoft.WindowsAzure.Storage.Table;
    using NBitcoin;
    using NBitcoin.OpenAsset;
    using Stratis.Bitcoin.Utilities;

    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "<Pending>")]

    // ReSharper disable once PartialTypeWithSinglePart
    public partial class IndexerClient
    {
        private Dictionary<string, Func<WalletRule>> rules = new Dictionary<string, Func<WalletRule>>();
        private readonly Network network;

        public IndexerClient(IndexerConfiguration configuration)
        {
            this.Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.BalancePartitionSize = 50;
            this.network = this.Configuration.Network;
        }

        public IndexerConfiguration Configuration { get; }

        public bool ColoredBalance { get; set; }

        public int BalancePartitionSize { get; set; }

        public Block GetBlock(uint256 blockId)
        {
            MemoryStream ms = new MemoryStream();
            CloudBlobContainer container = this.Configuration.GetBlocksContainer();
            try
            {
                container.GetPageBlobReference(blockId.ToString()).DownloadToStreamAsync(ms).GetAwaiter().GetResult();
                ms.Position = 0;
                Block b = this.network.Consensus.ConsensusFactory.CreateBlock();
                BitcoinStream stream = new BitcoinStream(ms.ToArray());
                stream.ConsensusFactory = this.network.Consensus.ConsensusFactory;
                b.ReadWrite(stream);
                return b;
            }
            catch (StorageException ex)
            {
                if (ex.RequestInformation != null && ex.RequestInformation.HttpStatusCode == 404)
                {
                    return null;
                }

                throw;
            }
        }

        // ReSharper disable once UnusedMember.Global
        public TransactionEntry GetTransaction(bool loadPreviousOutput, uint256 txId)
        {
            return this.GetTransactionAsync(loadPreviousOutput, txId).Result;
        }

        public Task<TransactionEntry> GetTransactionAsync(bool loadPreviousOutput, uint256 txId)
        {
            return this.GetTransactionAsync(loadPreviousOutput, false, txId);
        }

        public TransactionEntry GetTransaction(uint256 txId)
        {
            return this.GetTransactionAsync(txId).Result;
        }

        public Task<TransactionEntry> GetTransactionAsync(uint256 txId)
        {
            return this.GetTransactionAsync(true, false, txId);
        }

        // ReSharper disable once UnusedMember.Global
        public TransactionEntry[] GetTransactions(bool loadPreviousOutput, uint256[] txIds)
        {
            return this.GetTransactionsAsync(loadPreviousOutput, txIds).Result;
        }

        public Task<TransactionEntry[]> GetTransactionsAsync(bool loadPreviousOutput, uint256[] txIds)
        {
            return this.GetTransactionsAsync(loadPreviousOutput, false, txIds);
        }

        public async Task<TransactionEntry> GetTransactionAsync(bool loadPreviousOutput, bool fetchColor, uint256 txId)
        {
            if (txId == null)
            {
                return null;
            }

            TransactionEntry result = null;

            CloudTable table = this.Configuration.GetTransactionTable();
            TransactionEntry.Entity searchedEntity = new TransactionEntry.Entity(txId);
            TableQuery query = new TableQuery()
                            .Where(
                                    TableQuery.CombineFilters(
                                        TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, searchedEntity.PartitionKey),
                                        TableOperators.And,
                                        TableQuery.CombineFilters(
                                            TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.GreaterThan, txId + "-"),
                                            TableOperators.And,
                                            TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.LessThan, txId + "|"))));

            query.TakeCount = 10; // Should not have more
            List<TransactionEntry.Entity> entities = new List<TransactionEntry.Entity>();
            foreach (DynamicTableEntity e in await table.ExecuteQuerySegmentedAsync(query, null).ConfigureAwait(false))
            {
                entities.Add(
                    e.IsFat()
                        ? new TransactionEntry.Entity(
                            await this.FetchFatEntity(e).ConfigureAwait(false),
                            this.network)
                        : new TransactionEntry.Entity(e, this.network));
            }

            if (entities.Count > 0)
            {
                result = new TransactionEntry(entities.ToArray());
                if (result.Transaction == null)
                {
                    foreach (Block block in result.BlockIds.Select(this.GetBlock).Where(b => b != null))
                    {
                        result.Transaction = block.Transactions.FirstOrDefault(t => t.GetHash() == txId);
                        entities[0].Transaction = result.Transaction;
                        if (entities[0].Transaction != null)
                        {
                            await this.UpdateEntity(table, entities[0].CreateTableEntity(this.network)).ConfigureAwait(false);
                        }

                        break;
                    }
                }

                if (fetchColor && result.ColoredTransaction == null)
                {
                    result.ColoredTransaction = await ColoredTransaction.FetchColorsAsync(txId, result.Transaction, new CachedColoredTransactionRepository(new IndexerColoredTransactionRepository(this.Configuration))).ConfigureAwait(false);
                    entities[0].ColoredTransaction = result.ColoredTransaction;
                    if (entities[0].ColoredTransaction != null)
                    {
                        await this.UpdateEntity(table, entities[0].CreateTableEntity(this.network)).ConfigureAwait(false);
                    }
                }

                var needTxOut = result.SpentCoins == null && loadPreviousOutput && result.Transaction != null;
                if (needTxOut)
                {
                    OutPoint[] inputs = result.Transaction.Inputs.Select(o => o.PrevOut).ToArray();
                    TransactionEntry[] parents = await
                        this.GetTransactionsAsync(false, false, inputs
                             .Select(i => i.Hash)
                             .ToArray()).ConfigureAwait(false);

                    for (int i = 0; i < parents.Length; i++)
                    {
                        if (parents[i] == null)
                        {
                            IndexerTrace.MissingTransactionFromDatabase(result.Transaction.Inputs[i].PrevOut.Hash);
                            return null;
                        }
                    }

                    TxOut[] outputs = parents.Select((p, i) => p.Transaction.Outputs[inputs[i].N]).ToArray();

                    result.SpentCoins = Enumerable
                                            .Range(0, inputs.Length)
                                            .Select(i => new Spendable(inputs[i], outputs[i]))
                                            .ToList();
                    entities[0].PreviousTxOuts.Clear();
                    entities[0].PreviousTxOuts.AddRange(outputs);
                    if (entities[0].IsLoaded)
                    {
                        await this.UpdateEntity(table, entities[0].CreateTableEntity(this.network)).ConfigureAwait(false);
                    }
                }
            }

            return result?.Transaction != null ? result : null;
        }

        /// <summary>Get transactions in Azure Table</summary>
        /// <param name="lazyLoadPreviousOutput">Whether to lazy load previous output.</param>
        /// <param name="fetchColor">Fetch color.</param>
        /// <param name="txIds">Transaction ids.</param>
        /// <returns>All transactions (with null entries for unfounded transactions)</returns>
        public async Task<TransactionEntry[]> GetTransactionsAsync(bool lazyLoadPreviousOutput, bool fetchColor, uint256[] txIds)
        {
            var result = new TransactionEntry[txIds.Length];
            Task[] tasks = Enumerable.Range(0, txIds.Length)
                        .Select(i => new
                        {
                            TxId = txIds[i],
                            Index = i
                        })
                        .GroupBy(o => o.TxId, o => o.Index)
                        .Select(async (o) =>
                        {
                            TransactionEntry transaction = await this.GetTransactionAsync(lazyLoadPreviousOutput, fetchColor, o.Key).ConfigureAwait(false);
                            foreach (var index in o)
                            {
                                result[index] = transaction;
                            }
                        }).ToArray();

            await Task.WhenAll(tasks).ConfigureAwait(false);
            return result;
        }

        public ChainBlockHeader GetBestBlock()
        {
            CloudTable table = this.Configuration.GetChainTable();
            ChainPartEntry part = table.ExecuteQuery(new TableQuery()
            {
                TakeCount = 1
            }).Select(e => new ChainPartEntry(e)).FirstOrDefault();
            if (part == null)
            {
                return null;
            }

            BlockHeader block = part.BlockHeaders[part.BlockHeaders.Count - 1];
            return new ChainBlockHeader()
            {
                BlockId = block.GetHash(),
                Header = block,
                Height = part.ChainOffset + part.BlockHeaders.Count - 1
            };
        }

        public async Task<SmartContactEntry> GetSmartContractAsync(uint256 txId)
        {
            Guard.NotNull(txId, nameof(txId));

            CloudTable table = this.Configuration.GetSmartContactTable();
            TableQuery query = new TableQuery().Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, txId.ToString()));
            query.TakeCount = 1;

            TableQuerySegment result = await table.ExecuteQuerySegmentedAsync(query, null).ConfigureAwait(false);
            SmartContactEntry entity = result.Results.Select(r => new SmartContactEntry(r)).FirstOrDefault();
            return entity;
        }

        public async Task<SmartContactDetailsEntry> GetSmartContractDetailsAsync(string smartContractId)
        {
            Guard.NotNull(smartContractId, nameof(smartContractId));

            CloudTable table = this.Configuration.GetSmartContactDetailTable();
            TableQuery query = new TableQuery().Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, smartContractId));
            query.TakeCount = 1;

            TableQuerySegment result = await table.ExecuteQuerySegmentedAsync(query, null).ConfigureAwait(false);
            SmartContactDetailsEntry entity = result.Results.Select(r => new SmartContactDetailsEntry(r)).FirstOrDefault();
            return entity;
        }

        public IEnumerable<ChainBlockHeader> GetChainChangesUntilFork(ChainedHeader currentTip, bool forkIncluded, CancellationToken cancellation = default(CancellationToken))
        {
            ChainedHeader oldTip = currentTip;
            CloudTable table = this.Configuration.GetChainTable();
            foreach (ChainPartEntry chainPart in this.ExecuteBalanceQuery(table, new TableQuery(), new[] { 1, 2, 10 })
                                          .Concat(table.ExecuteQuery(new TableQuery()).Skip(2))
                                          .Select(e => new ChainPartEntry(e)))
            {
                cancellation.ThrowIfCancellationRequested();

                int height = chainPart.ChainOffset + chainPart.BlockHeaders.Count - 1;
                foreach (BlockHeader block in chainPart.BlockHeaders.Reverse<BlockHeader>())
                {
                    if (currentTip == null && oldTip != null)
                    {
                        throw new InvalidOperationException(
                            "No fork found, the chain stored in azure is probably different from the one of the provided input");
                    }

                    if (oldTip == null || height > currentTip.Height)
                    {
                        yield return this.CreateChainChange(height, block);
                    }
                    else
                    {
                        if (height < currentTip.Height)
                        {
                            currentTip = currentTip.GetAncestor(height);
                        }

                        if (currentTip == null || height > currentTip.Height)
                        {
                            throw new InvalidOperationException("Ancestor block not found in chain.");
                        }

                        ChainBlockHeader chainChange = this.CreateChainChange(height, block);
                        if (chainChange.BlockId == currentTip.HashBlock)
                        {
                            if (forkIncluded)
                            {
                                yield return chainChange;
                            }

                            yield break;
                        }

                        yield return chainChange;
                        currentTip = currentTip.Previous;
                    }

                    height--;
                }
            }
        }

        public WalletRuleEntry[] GetWalletRules(string walletId)
        {
            CloudTable table = this.Configuration.GetWalletRulesTable();
            DynamicTableEntity searchedEntity = new WalletRuleEntry(walletId, null).CreateTableEntity();
            TableQuery query = new TableQuery()
                                    .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, searchedEntity.PartitionKey));
            return
                table.ExecuteQuery(query)
                 .Select(e => new WalletRuleEntry(e, this))
                 .ToArray();
        }

        public WalletRuleEntry AddWalletRule(string walletId, WalletRule walletRule)
        {
            CloudTable table = this.Configuration.GetWalletRulesTable();
            WalletRuleEntry entry = new WalletRuleEntry(walletId, walletRule);
            DynamicTableEntity entity = entry.CreateTableEntity();
            table.ExecuteAsync(TableOperation.InsertOrReplace(entity)).GetAwaiter().GetResult();
            return entry;
        }

        public WalletRuleEntryCollection GetAllWalletRules()
        {
            return
                new WalletRuleEntryCollection(
                this.Configuration.GetWalletRulesTable()
                .ExecuteQuery(new TableQuery())
                .Select(e => new WalletRuleEntry(e, this)));
        }

        public IEnumerable<OrderedBalanceChange> GetOrderedBalance(
            string walletId,
            BalanceQuery query = null,
            CancellationToken cancel = default(CancellationToken))
        {
            return this.GetOrderedBalanceCore(new BalanceId(walletId), query, cancel);
        }

        public IEnumerable<OrderedBalanceChange> GetOrderedBalance(
            BalanceId balanceId,
            BalanceQuery query = null,
            CancellationToken cancel = default(CancellationToken))
        {
            return this.GetOrderedBalanceCore(balanceId, query, cancel);
        }

        public IEnumerable<Task<List<OrderedBalanceChange>>> GetOrderedBalanceAsync(
            BalanceId balanceId,
            BalanceQuery query = null,
            CancellationToken cancel = default(CancellationToken))
        {
            return this.GetOrderedBalanceCoreAsync(balanceId, query, cancel);
        }

        public IEnumerable<Task<List<OrderedBalanceChange>>> GetOrderedBalanceAsync(
            string walletId,
            BalanceQuery query = null,
            CancellationToken cancel = default(CancellationToken))
        {
            return this.GetOrderedBalanceCoreAsync(new BalanceId(walletId), query, cancel);
        }

        public IEnumerable<OrderedBalanceChange> GetOrderedBalance(IDestination destination, BalanceQuery query = null, CancellationToken cancel = default(CancellationToken))
        {
            return this.GetOrderedBalance(destination.ScriptPubKey, query, cancel);
        }

        public IEnumerable<Task<List<OrderedBalanceChange>>> GetOrderedBalanceAsync(IDestination destination, BalanceQuery query = null, CancellationToken cancel = default(CancellationToken))
        {
            return this.GetOrderedBalanceAsync(destination.ScriptPubKey, query, cancel);
        }

        public IEnumerable<OrderedBalanceChange> GetOrderedBalance(Script scriptPubKey, BalanceQuery query = null, CancellationToken cancel = default(CancellationToken))
        {
            return this.GetOrderedBalanceCore(new BalanceId(scriptPubKey), query, cancel);
        }

        public IEnumerable<Task<List<OrderedBalanceChange>>> GetOrderedBalanceAsync(Script scriptPubKey, BalanceQuery query = null, CancellationToken cancel = default(CancellationToken))
        {
            return this.GetOrderedBalanceCoreAsync(new BalanceId(scriptPubKey), query, cancel);
        }

        public void CleanUnconfirmedChanges(IDestination destination, TimeSpan olderThan)
        {
            this.CleanUnconfirmedChanges(destination.ScriptPubKey, olderThan);
        }

        public void CleanUnconfirmedChanges(Script scriptPubKey, TimeSpan olderThan)
        {
            CloudTable table = this.Configuration.GetBalanceTable();
            List<DynamicTableEntity> unconfirmed = new List<DynamicTableEntity>();

            foreach (DynamicTableEntity c in table.ExecuteQuery(new BalanceQuery().CreateTableQuery(new BalanceId(scriptPubKey))))
            {
                OrderedBalanceChange change = new OrderedBalanceChange(c);
                if (change.BlockId != null)
                {
                    break;
                }

                if (DateTime.UtcNow - change.SeenUtc < olderThan)
                {
                    continue;
                }

                unconfirmed.Add(c);
            }

            Parallel.ForEach(unconfirmed, c =>
            {
                CloudTable t = this.Configuration.GetBalanceTable();
                c.ETag = "*";
                t.ExecuteAsync(TableOperation.Delete(c)).GetAwaiter().GetResult();
            });
        }

        public bool NeedLoading(OrderedBalanceChange change)
        {
            if (change.SpentCoins != null)
            {
                if (change.ColoredTransaction != null || !this.ColoredBalance)
                {
                    return false;
                }
            }

            return true;
        }

        public async Task<bool> EnsurePreviousLoadedAsync(OrderedBalanceChange change)
        {
            if (!this.NeedLoading(change))
            {
                return true;
            }

            uint256[] parentIds = change.SpentOutpoints.Select(s => s.Hash).ToArray();
            TransactionEntry[] parents =
                await this.GetTransactionsAsync(false, this.ColoredBalance, parentIds).ConfigureAwait(false);

            NoSqlTransactionRepository cache = new NoSqlTransactionRepository(this.network);
            foreach (TransactionEntry parent in parents.Where(p => p != null))
            {
                cache.Put(parent.TransactionId, parent.Transaction);
            }

            if (change.SpentCoins == null)
            {
                var success = await change.EnsureSpentCoinsLoadedAsync(cache).ConfigureAwait(false);
                if (!success)
                {
                    return false;
                }
            }

            if (this.ColoredBalance && change.ColoredTransaction == null)
            {
                IndexerColoredTransactionRepository indexerRepo = new IndexerColoredTransactionRepository(this.Configuration);
                indexerRepo.Transactions = new CompositeTransactionRepository(new[] { new ReadOnlyTransactionRepository(cache), indexerRepo.Transactions });
                var success = await change.EnsureColoredTransactionLoadedAsync(indexerRepo).ConfigureAwait(false);
                if (!success)
                {
                    return false;
                }
            }

            DynamicTableEntity entity = change.ToEntity();
            if (!change.IsEmpty)
            {
                await this.Configuration.GetBalanceTable().ExecuteAsync(TableOperation.Merge(entity)).ConfigureAwait(false);
            }
            else
            {
                try
                {
                    await this.Configuration.GetTransactionTable().ExecuteAsync(TableOperation.Delete(entity)).ConfigureAwait(false);
                }
                catch (StorageException ex)
                {
                    if (ex.RequestInformation == null || ex.RequestInformation.HttpStatusCode != 404)
                    {
                        throw;
                    }
                }
            }

            return true;
        }

        public void PruneBalances(IEnumerable<OrderedBalanceChange> balances)
        {
            Parallel.ForEach(balances, b =>
            {
                CloudTable table = this.Configuration.GetBalanceTable();
                table.ExecuteAsync(TableOperation.Delete(b.ToEntity())).GetAwaiter().GetResult();
            });
        }

        public ConcurrentChain GetMainChain()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            ConcurrentChain chain = new ConcurrentChain();
#pragma warning restore CS0618 // Type or member is obsolete
            this.SynchronizeChain(chain);
            return chain;
        }

        public void SynchronizeChain(ChainBase chain)
        {
            if (chain.Tip != null && chain.Genesis.HashBlock != this.network.GetGenesis().GetHash())
            {
                throw new ArgumentException("Incompatible Network between the indexer and the chain", nameof(chain));
            }

            if (chain.Tip == null)
            {
                Block genesis = this.network.GetGenesis();
                chain.SetTip(new ChainedHeader(genesis.Header, genesis.GetHash(), 0));
            }

            this.GetChainChangesUntilFork(chain.Tip, false)
                .UpdateChain(chain);
        }

        public bool MergeIntoWallet(
            string walletId,
            IDestination destination,
            WalletRule rule = null,
            CancellationToken cancel = default(CancellationToken))
        {
            return this.MergeIntoWallet(walletId, destination.ScriptPubKey, rule, cancel);
        }

        public bool MergeIntoWallet(string walletId, Script scriptPubKey, WalletRule rule = null, CancellationToken cancel = default(CancellationToken))
        {
            return this.MergeIntoWalletCore(walletId, new BalanceId(scriptPubKey), rule, cancel);
        }

        public bool MergeIntoWallet(
            string walletId,
            string walletSource,
            WalletRule rule = null,
            CancellationToken cancel = default(CancellationToken))
        {
            return this.MergeIntoWalletCore(walletId, new BalanceId(walletSource), rule, cancel);
        }

        private IEnumerable<OrderedBalanceChange> GetOrderedBalanceCore(BalanceId balanceId, BalanceQuery query, CancellationToken cancel)
        {
            foreach (Task<List<OrderedBalanceChange>> partition in this.GetOrderedBalanceCoreAsync(balanceId, query, cancel))
            {
                foreach (OrderedBalanceChange change in partition.Result)
                {
                    yield return change;
                }
            }
        }

        private IEnumerable<Task<List<OrderedBalanceChange>>> GetOrderedBalanceCoreAsync(BalanceId balanceId, BalanceQuery query, CancellationToken cancel)
        {
            if (query == null)
            {
                query = new BalanceQuery();
            }

            CloudTable table = this.Configuration.GetBalanceTable();
            IEnumerable<DynamicTableEntity> tableQuery = this.ExecuteBalanceQuery(table, query.CreateTableQuery(balanceId), query.PageSizes);

            IEnumerable<List<LoadingTransactionTask>> partitions =
                  tableQuery
                 .Select(c => new OrderedBalanceChange(c))
                 .Select(c => new LoadingTransactionTask
                 {
                     Loaded = this.NeedLoading(c) ? this.EnsurePreviousLoadedAsync(c) : Task.FromResult(true),
                     Change = c
                 })
                 .Partition(this.BalancePartitionSize);

            if (!query.RawOrdering)
            {
                return this.GetOrderedBalanceCoreAsyncOrdered(partitions, cancel);
            }

            return this.GetOrderedBalanceCoreAsyncRaw(partitions, cancel);
        }

        private IEnumerable<Task<List<OrderedBalanceChange>>> GetOrderedBalanceCoreAsyncRaw(IEnumerable<List<LoadingTransactionTask>> partitions, CancellationToken cancel)
        {
            List<OrderedBalanceChange> result = new List<OrderedBalanceChange>();
            foreach (List<LoadingTransactionTask> partition in partitions)
            {
                cancel.ThrowIfCancellationRequested();
                Task<bool[]> partitionLoading = Task.WhenAll(partition.Select(_ => _.Loaded));
                foreach (OrderedBalanceChange change in partition.Select(p => p.Change))
                {
                    result.Add(change);
                }

                yield return this.WaitAndReturn(partitionLoading, result);
                result = new List<OrderedBalanceChange>();
            }
        }

        private bool Prepare(OrderedBalanceChange change)
        {
            change.UpdateToScriptCoins();
            if (change.SpentCoins == null || change.ReceivedCoins == null)
            {
                return false;
            }

            if (change.IsEmpty)
            {
                return false;
            }

            if (this.ColoredBalance)
            {
                if (change.ColoredTransaction == null)
                {
                    return false;
                }

                change.UpdateToColoredCoins();
            }

            return true;
        }

        private IEnumerable<Task<List<OrderedBalanceChange>>> GetOrderedBalanceCoreAsyncOrdered(IEnumerable<List<LoadingTransactionTask>> partitions, CancellationToken cancel)
        {
            Queue<OrderedBalanceChange> unconfirmed = new Queue<OrderedBalanceChange>();
            List<OrderedBalanceChange> unconfirmedList = new List<OrderedBalanceChange>();

            List<OrderedBalanceChange> result = new List<OrderedBalanceChange>();
            foreach (List<LoadingTransactionTask> partition in partitions)
            {
                cancel.ThrowIfCancellationRequested();
                Task<bool[]> partitionLoading = Task.WhenAll(partition.Select(_ => _.Loaded));
                foreach (OrderedBalanceChange change in partition.Select(p => p.Change))
                {
                    if (change.BlockId == null)
                    {
                        unconfirmedList?.Add(change);
                    }
                    else
                    {
                        if (unconfirmedList != null)
                        {
                            unconfirmed = new Queue<OrderedBalanceChange>(unconfirmedList.OrderByDescending(o => o.SeenUtc));
                            unconfirmedList = null;
                        }

                        while (unconfirmed.Count != 0 && change.SeenUtc < unconfirmed.Peek().SeenUtc)
                        {
                            OrderedBalanceChange unconfirmedChange = unconfirmed.Dequeue();
                            result.Add(unconfirmedChange);
                        }

                        result.Add(change);
                    }
                }

                yield return this.WaitAndReturn(partitionLoading, result);
                result = new List<OrderedBalanceChange>();
            }

            if (unconfirmedList != null)
            {
                unconfirmed = new Queue<OrderedBalanceChange>(unconfirmedList.OrderByDescending(o => o.SeenUtc));
            }

            while (unconfirmed.Count != 0)
            {
                OrderedBalanceChange change = unconfirmed.Dequeue();
                result.Add(change);
            }

            if (result.Count > 0)
            {
                yield return this.WaitAndReturn(null, result);
            }
        }

        private IEnumerable<DynamicTableEntity> ExecuteBalanceQuery(CloudTable table, TableQuery tableQuery, IEnumerable<int> pages)
        {
            pages = pages ?? Array.Empty<int>();
            TableContinuationToken continuation = null;
            using (IEnumerator<int> pagesEnumerator = pages.GetEnumerator())
            {
                do
                {
                    tableQuery.TakeCount = pagesEnumerator.MoveNext() ? (int?)pagesEnumerator.Current : null;

                    TableQuerySegment segment = table.ExecuteQuerySegmentedAsync(tableQuery, continuation).GetAwaiter().GetResult();
                    continuation = segment.ContinuationToken;
                    foreach (DynamicTableEntity entity in segment)
                    {
                        yield return entity;
                    }
                }
                while (continuation != null);
            }
        }

        private async Task<List<OrderedBalanceChange>> WaitAndReturn(Task<bool[]> partitionLoading, List<OrderedBalanceChange> result)
        {
            if (partitionLoading != null)
            {
                await Task.WhenAll(partitionLoading).ConfigureAwait(false);
            }

            List<OrderedBalanceChange> toDelete = new List<OrderedBalanceChange>();
            foreach (OrderedBalanceChange entity in result)
            {
                if (!this.Prepare(entity))
                {
                    toDelete.Add(entity);
                }
            }

            foreach (OrderedBalanceChange deletion in toDelete)
            {
                result.Remove(deletion);
            }

            return result;
        }

        private async Task UpdateEntity(CloudTable table, DynamicTableEntity entity)
        {
            try
            {
                await table.ExecuteAsync(TableOperation.Merge(entity)).ConfigureAwait(false);
                return;
            }
            catch (StorageException ex)
            {
                if (!Helper.IsError(ex, "EntityTooLarge"))
                {
                    throw;
                }
            }

            byte[] serialized = entity.Serialize();
            this.Configuration
                .GetBlocksContainer()
                .GetBlockBlobReference(entity.GetFatBlobName())
                .UploadFromByteArrayAsync(serialized, 0, serialized.Length).GetAwaiter().GetResult();
            entity.MakeFat(serialized.Length);
            await table.ExecuteAsync(TableOperation.InsertOrReplace(entity)).ConfigureAwait(false);
        }

        private ChainBlockHeader CreateChainChange(int height, BlockHeader block)
        {
            return new ChainBlockHeader()
            {
                Height = height,
                Header = block,
                BlockId = block.GetHash()
            };
        }

        private async Task<DynamicTableEntity> FetchFatEntity(DynamicTableEntity e)
        {
            int? fatValue = e.Properties["fat"].Int32Value;
            if (fatValue == null)
            {
                return e;
            }

            var size = fatValue.Value;
            byte[] bytes = new byte[size];
            await this.Configuration
                .GetBlocksContainer()
                .GetBlockBlobReference(e.GetFatBlobName())
                .DownloadRangeToByteArrayAsync(bytes, 0, 0, bytes.Length).ConfigureAwait(false);
            e = new DynamicTableEntity();
            e.Deserialize(bytes);

            return e;
        }

        private bool MergeIntoWalletCore(string walletId, BalanceId balanceId, WalletRule rule, CancellationToken cancel)
        {
            AzureIndexer indexer = this.Configuration.CreateIndexer();

            BalanceQuery query = new BalanceQuery()
            {
                From = new UnconfirmedBalanceLocator().Floor(),
                RawOrdering = true
            };
            Dictionary<string, OrderedBalanceChange> sourcesByKey = this.GetOrderedBalanceCore(balanceId, query, cancel)
                .ToDictionary(this.GetKey);
            if (sourcesByKey.Count == 0)
            {
                return false;
            }

            Dictionary<string, OrderedBalanceChange> destByKey =
                this.GetOrderedBalance(walletId, query, cancel)
                .ToDictionary(this.GetKey);

            List<OrderedBalanceChange> entities = new List<OrderedBalanceChange>();
            foreach (KeyValuePair<string, OrderedBalanceChange> kv in sourcesByKey)
            {
                OrderedBalanceChange source = kv.Value;
                OrderedBalanceChange existing = destByKey.TryGet(kv.Key) ?? new OrderedBalanceChange(walletId, source);

                existing.Merge(kv.Value, rule);
                entities.Add(existing);
                if (entities.Count == 100)
                {
                    indexer.Index(entities);
                }
            }

            if (entities.Count != 0)
            {
                indexer.Index(entities);
            }

            return true;
        }

        private string GetKey(OrderedBalanceChange change)
        {
            return change.Height + "-" + (change.BlockId == null ? new uint256(0) : change.BlockId) + "-" + change.TransactionId + "-" + change.SeenUtc.Ticks;
        }
    }
}
