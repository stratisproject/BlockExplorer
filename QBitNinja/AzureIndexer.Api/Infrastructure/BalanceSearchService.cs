namespace AzureIndexer.Api.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using Models;
    using NBitcoin;
    using Stratis.Bitcoin.Features.AzureIndexer;

    public class BalanceSearchService : IBalanceSearchService
    {
        private readonly ChainIndexer chain;
        private readonly QBitNinjaConfiguration configuration;
        private readonly TimeSpan expiration = TimeSpan.FromHours(24.0);
        private readonly Network network;

        public BalanceSearchService(ChainIndexer chain, QBitNinjaConfiguration configuration)
        {
            this.chain = chain;
            this.configuration = configuration;
            this.network = this.configuration.Indexer.Network;
        }

        public BalanceSummary GetBalanceSummary(
            BalanceId balanceId,
            BlockFeature at,
            bool debug,
            bool colored)
        {
            var repo = this.configuration.CreateWalletRepository();
            CancellationTokenSource cancel = new CancellationTokenSource();
            cancel.CancelAfter(30000);
            var checkpoint = this.configuration.Indexer.CreateIndexer()
                .GetCheckpoint(balanceId.Type == BalanceType.Address ? IndexerCheckpoints.Balances : IndexerCheckpoints.Wallets);

            var atBlock = this.AtBlock(at);

            var query = new BalanceQuery();
            query.RawOrdering = true;
            query.From = null;

            if (at != null)
            {
                query.From = this.ToBalanceLocator(atBlock);
            }

            if (query.From == null)
            {
                query.From = new UnconfirmedBalanceLocator(DateTimeOffset.UtcNow - this.expiration);
            }

            query.PageSizes = new[] { 1, 10, 100 };

            var cacheTable = repo.GetBalanceSummaryCacheTable(balanceId, colored);
            var cachedSummary = cacheTable.Query(this.chain, query).FirstOrDefault(c => (((ConfirmedBalanceLocator)c.Locator).BlockHash == atBlock.HashBlock && at != null) ||
                                                                                   c.Immature.TransactionCount == 0 ||
                                                                                   ((c.Immature.TransactionCount != 0) && !this.IsMature(c.OlderImmature, atBlock)));

            var cachedLocator = (ConfirmedBalanceLocator)cachedSummary?.Locator;
            if (cachedSummary != null && at != null && cachedLocator.Height == atBlock.Height)
            {
                cachedSummary.CacheHit = CacheHit.FullCache;
                cachedSummary.PrepareForSend(at, debug);
                return cachedSummary;
            }

            cachedSummary = cachedSummary ?? new BalanceSummary
            {
                Confirmed = new BalanceSummaryDetails(),
                UnConfirmed = new BalanceSummaryDetails(),
                OlderImmature = int.MaxValue
            };

            int stopAtHeight = cachedSummary.Locator == null ? -1 : cachedLocator?.Height ?? -1;
            int lookback = (int)(this.expiration.Ticks / this.network.Consensus.PowTargetSpacing.Ticks);
            if (at == null)
            {
                stopAtHeight = stopAtHeight - lookback;
            }

            var client = this.configuration.Indexer.CreateIndexerClient();
            client.ColoredBalance = colored;

            var diff =
                client
                .GetOrderedBalance(balanceId, query)
                .WhereNotExpired(this.expiration)
                .TakeWhile(_ => !cancel.IsCancellationRequested)
                .TakeWhile(_ => _.BlockId == null || _.Height > stopAtHeight - lookback) // Some confirmation of the fetched unconfirmed may hide behind stopAtHeigh
                .AsBalanceSheet(this.chain);

            if (cancel.Token.IsCancellationRequested)
            {
                throw new HttpResponseException(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    ReasonPhrase = "The server can't fetch the balance summary because the balance is too big. Please, load it in several step with ?at={blockFeature} parameter. Once fully loaded after all the step, the summary will return in constant time."
                });
            }

            this.RemoveBehind(diff, stopAtHeight);
            this.RemoveConflicts(diff);

            var unconfs = diff.Unconfirmed;
            var confs = cachedLocator == null ?
                                            diff.Confirmed :
                                            diff.Confirmed.Where(c => c.Height > cachedLocator.Height).ToList();

            var immature = confs.Where(c => !this.IsMature(c, atBlock)).ToList();

            var summary = new BalanceSummary()
            {
                Confirmed = BalanceSummaryDetails.CreateFrom(confs, this.network, colored),
                Immature = BalanceSummaryDetails.CreateFrom(immature, this.network, colored),
                UnConfirmed = BalanceSummaryDetails.CreateFrom(unconfs, this.network, colored),
            };
            summary.Confirmed += cachedSummary.Confirmed;
            summary.Immature += cachedSummary.Immature;
            summary.Locator = new ConfirmedBalanceLocator(atBlock.Height, atBlock.HashBlock);
            summary.CacheHit = cachedSummary.Locator == null ? CacheHit.NoCache : CacheHit.PartialCache;

            var newCachedLocator = (ConfirmedBalanceLocator)summary.Locator;

            if (cachedSummary.Locator == null ||
                newCachedLocator.BlockHash != cachedLocator?.BlockHash)
            {
                var olderImmature = immature.Select(_ => _.Height).Concat(new[] { int.MaxValue }).Min();
                var newCachedSummary = new BalanceSummary
                {
                    Confirmed = summary.Confirmed,
                    Immature = summary.Immature,
                    Locator = summary.Locator,
                    OlderImmature = Math.Min(cachedSummary.OlderImmature, olderImmature)
                };
                var checkpointBlock = this.chain.GetBlock(checkpoint.BlockLocator.Blocks[0]);
                if (checkpointBlock != null && checkpointBlock.Height >= atBlock.Height)
                {
                    cacheTable.Create(newCachedLocator, newCachedSummary);
                }
            }

            summary.PrepareForSend(at, debug);
            return summary;
        }

        public BalanceModel GetBalanceDetails(
            BalanceId balanceId,
            BalanceLocator continuation,
            BlockFeature until,
            BlockFeature from,
            bool includeImmature,
            bool unspentOnly,
            bool colored)
        {
            CancellationTokenSource cancel = new CancellationTokenSource();
            cancel.CancelAfter(30000);

            BalanceQuery query = new BalanceQuery();
            query.RawOrdering = true;
            query.From = null;

            if (from != null)
            {
                query.From = this.ToBalanceLocator(from);
                query.FromIncluded = true;
            }

            if (continuation != null)
            {
                query = new BalanceQuery
                {
                    From = continuation,
                    FromIncluded = false
                };
                query.RawOrdering = true;
            }

            if (query.From == null)
            {
                query.From = new UnconfirmedBalanceLocator(DateTimeOffset.UtcNow - this.expiration);
            }

            if (until != null)
            {
                query.To = this.ToBalanceLocator(until);
                query.FromIncluded = true;
            }

            if (query.To.IsGreaterThan(query.From))
            {
                throw this.InvalidParameters("Invalid argument : from < until");
            }

            var client = this.configuration.Indexer.CreateIndexerClient();
            client.ColoredBalance = colored;
            var balance =
                client
                .GetOrderedBalance(balanceId, query)
                .TakeWhile(_ => !cancel.IsCancellationRequested)
                .WhereNotExpired(this.expiration)
                .Where(o => includeImmature || this.IsMature(o, this.chain.Tip))
                .AsBalanceSheet(this.chain);

            var balanceChanges = balance.All;

            // Strip unconfirmed that can appear after the last until
            if (until != null && balance.Confirmed.Count != 0)
            {
                List<OrderedBalanceChange> oldUnconfirmed = new List<OrderedBalanceChange>();
                var older = balanceChanges.Last();

                foreach (var last in balanceChanges)
                {
                    if (last.BlockId == null)
                    {
                        if (last.SeenUtc < older.SeenUtc)
                        {
                            oldUnconfirmed.Add(last);
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                foreach (var unconf in oldUnconfirmed)
                {
                    balanceChanges.Remove(unconf);
                }
            }

            var conflicts = this.RemoveConflicts(balance);

            if (unspentOnly)
            {
                HashSet<OutPoint> spents = new HashSet<OutPoint>();
                foreach (var change in balanceChanges.SelectMany(b => b.SpentCoins))
                {
                    spents.Add(change.Outpoint);
                }

                foreach (var change in balanceChanges)
                {
                    change.SpentCoins.Clear();
                    change.ReceivedCoins.RemoveAll(c => spents.Contains(c.Outpoint));
                }
            }

            var result = new BalanceModel(balanceChanges, this.chain);
            result.ConflictedOperations = result.GetBalanceOperations(conflicts, this.chain);
            if (cancel.IsCancellationRequested)
            {
                if (balanceChanges.Count > 0)
                {
                    var lastop = balanceChanges[balanceChanges.Count - 1];
                    result.Continuation = lastop.CreateBalanceLocator();
                }
            }

            return result;
        }

        private List<OrderedBalanceChange> RemoveConflicts(BalanceSheet balance)
        {
            var spentOutputs = new Dictionary<OutPoint, OrderedBalanceChange>();
            var conflicts = new List<OrderedBalanceChange>();
            var unconfirmedConflicts = new List<OrderedBalanceChange>();
            foreach (var balanceChange in balance.All)
            {
                foreach (var spent in balanceChange.SpentCoins)
                {
                    if (!spentOutputs.TryAdd(spent.Outpoint, balanceChange))
                    {
                        var balanceChange2 = spentOutputs[spent.Outpoint];
                        var score = this.GetScore(balanceChange);
                        var score2 = this.GetScore(balanceChange2);
                        var conflicted =
                            score == score2 ?
                                ((balanceChange.SeenUtc < balanceChange2.SeenUtc) ? balanceChange : balanceChange2) :
                                score < score2 ? balanceChange : balanceChange2;
                        conflicts.Add(conflicted);

                        var nonConflicted = conflicted == balanceChange ? balanceChange2 : balanceChange;
                        if (nonConflicted.BlockId == null || !this.chain.Contains(nonConflicted.BlockId))
                        {
                            unconfirmedConflicts.Add(conflicted);
                        }
                    }
                }
            }

            foreach (var conflict in conflicts)
            {
                balance.All.Remove(conflict);
                balance.Unconfirmed.Remove(conflict);
            }

            return unconfirmedConflicts;
        }

        private long GetScore(OrderedBalanceChange balance)
        {
            long score = 0;
            if (balance.BlockId != null)
            {
                score += 10;
                if (this.chain.Contains(balance.BlockId))
                {
                    score += 100;
                }
            }

            return score;
        }

        private void RemoveBehind(BalanceSheet diff, int stopAtHeight)
        {
            this.RemoveBehind(diff.All, stopAtHeight);
            this.RemoveBehind(diff.Confirmed, stopAtHeight);
            this.RemoveBehind(diff.Unconfirmed, stopAtHeight);
            this.RemoveBehind(diff.Prunable, stopAtHeight);
        }

        private void RemoveBehind(List<OrderedBalanceChange> changes, int stopAtHeight)
        {
            foreach (var change in changes.ToList())
            {
                if (change.BlockId != null)
                {
                    if (change.Height <= stopAtHeight)
                    {
                        changes.Remove(change);
                    }
                }
            }
        }

        private ConfirmedBalanceLocator ToBalanceLocator(BlockFeature feature)
        {
            return this.ToBalanceLocator(this.AtBlock(feature));
        }

        private ConfirmedBalanceLocator ToBalanceLocator(ChainedHeader atBlock)
        {
            return new ConfirmedBalanceLocator(atBlock.Height, atBlock.HashBlock);
        }

        private ChainedHeader AtBlock(BlockFeature at)
        {
            var atBlock = this.chain.Tip;
            if (at != null)
            {
                var chainedBlock = at.GetChainedBlock(this.chain);
                atBlock = chainedBlock ?? throw new FormatException("'at' not found in the blockchain");
            }

            return atBlock;
        }

        private bool IsMature(int height, ChainedHeader tip)
        {
            return tip.Height - height >= this.configuration.CoinbaseMaturity;
        }

        private bool IsMature(OrderedBalanceChange c, ChainedHeader tip)
        {
            return !c.IsCoinbase || (c.BlockId != null && this.IsMature(c.Height, tip));
        }

        private Exception InvalidParameters(string message)
        {
            return new HttpResponseException(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.BadRequest,
                ReasonPhrase = message
            });
        }
    }
}
