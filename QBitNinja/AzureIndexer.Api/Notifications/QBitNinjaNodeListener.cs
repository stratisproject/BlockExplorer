using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AzureIndexer.Api.Infrastructure;
using AzureIndexer.Api.Models;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceBus.Messaging;
using NBitcoin;
using Stratis.Bitcoin;
using Stratis.Bitcoin.Connection;
using Stratis.Bitcoin.Consensus;
using Stratis.Bitcoin.Features.AzureIndexer;
using Stratis.Bitcoin.Features.AzureIndexer.IndexTasks;
using Stratis.Bitcoin.Interfaces;
using Stratis.Bitcoin.P2P.Peer;
using Stratis.Bitcoin.P2P.Protocol;
using Stratis.Bitcoin.P2P.Protocol.Behaviors;
using Stratis.Bitcoin.P2P.Protocol.Payloads;
using ReaderWriterLock = NBitcoin.ReaderWriterLock;

namespace AzureIndexer.Api.Notifications
{
    public class QBitNinjaNodeListener : IDisposable
    {
        class Behavior : NetworkPeerBehavior
        {
            QBitNinjaNodeListener _Listener;
            private readonly ILoggerFactory loggerFactory;

            public Behavior(QBitNinjaNodeListener listener, ILoggerFactory loggerFactory)
            {
                _Listener = listener;
                this.loggerFactory = loggerFactory;
            }

            protected override void AttachCore()
            {
                AttachedPeer.StateChanged.Register(AttachedNode_StateChanged);
            }

            Task AttachedNode_StateChanged(INetworkPeer node, NetworkPeerState oldState)
            {
                ListenerTrace.Info("State change " + node.State);
                if(node.State == NetworkPeerState.HandShaked)
                {
                    ListenerTrace.Info("Node handshaked");
                    AttachedPeer.MessageReceived.Register(_Listener.node_MessageReceived);
                    // AttachedPeer.Disconnected += AttachedNode_Disconnected; TODO: check if still needed
                    // ListenerTrace.Info("Connection count : " + nNodesGroup.GetNodeGroup(node).ConnectedNodes.Count); TODO: fix count
                }

                return Task.CompletedTask;
            }

            void AttachedNode_Disconnected(INetworkPeer node)
            {
                ListenerTrace.Info("Node Connection dropped : " + ToString(node.DisconnectReason));
            }

            private string ToString(NetworkPeerDisconnectReason reason)
            {
                if(reason == null)
                    return null;
                return reason.Reason + " " + reason.Exception == null ? "" : Utils.ExceptionToString(reason.Exception);
            }

            protected override void DetachCore()
            {
                AttachedPeer.StateChanged.Unregister(AttachedNode_StateChanged);
                AttachedPeer.MessageReceived.Unregister(_Listener.node_MessageReceived);
            }

            public override object Clone()
            {
                return new Behavior(_Listener, this.loggerFactory);
            }
        }
        private readonly QBitNinjaConfiguration _Configuration;
        private readonly IInitialBlockDownloadState initialBlockDownloadState;
        private readonly IConsensusManager consensusManager;
        private readonly IConnectionManager connectionManager;
        private readonly IPeerBanning peerBanning;
        private readonly ILoggerFactory loggerFactory;
        SubscriptionCollection _Subscriptions = null;
        ReaderWriterLock _SubscriptionSlimLock = new ReaderWriterLock();

        private Stratis.Bitcoin.Features.AzureIndexer.WalletRuleEntryCollection _Wallets;
        ReaderWriterLock _WalletsSlimLock = new ReaderWriterLock();

        object _LockBalance = new object();
        object _LockTransactions = new object();
        object _LockBlocks = new object();
        object _LockWallets = new object();
        object _LockSubscriptions = new object();

        public QBitNinjaConfiguration Configuration
        {
            get
            {
                return _Configuration;
            }
        }
        public QBitNinjaNodeListener(
            QBitNinjaConfiguration configuration,
            IInitialBlockDownloadState initialBlockDownloadState,
            IConsensusManager consensusManager,
            IConnectionManager connectionManager,
            IPeerBanning peerBanning,
            ILoggerFactory loggerFactory)
        {
            _Configuration = configuration;
            this.initialBlockDownloadState = initialBlockDownloadState;
            this.consensusManager = consensusManager;
            this.connectionManager = connectionManager;
            this.peerBanning = peerBanning;
            this.loggerFactory = loggerFactory;
        }

        private Stratis.Bitcoin.Features.AzureIndexer.AzureIndexer _Indexer;
        public Stratis.Bitcoin.Features.AzureIndexer.AzureIndexer Indexer
        {
            get
            {
                return _Indexer;
            }
        }

        CustomThreadPoolTaskScheduler _IndexerScheduler;

        List<IDisposable> _Disposables = new List<IDisposable>();
        public void Listen(ChainIndexer chain = null)
        {
            _Chain = new ChainIndexer(_Configuration.Indexer.Network);
            _Indexer = Configuration.Indexer.CreateIndexer();
            if(chain == null)
            {
                chain = new ChainIndexer(_Configuration.Indexer.Network);
            }
            _Chain = chain;
            ListenerTrace.Info("Fetching headers from " + _Chain.Tip.Height + " (from azure)");
            var client = Configuration.Indexer.CreateIndexerClient();
            client.SynchronizeChain(chain);
            ListenerTrace.Info("Headers fetched tip " + _Chain.Tip.Height);

            _Disposables.Add(_IndexerScheduler = new CustomThreadPoolTaskScheduler(50, 100, "Indexing Threads"));
            _Indexer.TaskScheduler = _IndexerScheduler;

            // _Group = new NodesGroup(Configuration.Indexer.Network); TODO: check if this is correct
            _Group = new NetworkPeerCollection();
            
            // TODO: Check if needed
            // _Disposables.Add(_Group);
            // _Group.AllowSameGroup = true;
            // _Group.MaximumNodeConnection = 2;

            //addrman.Add(new NetworkAddress(Utils.ParseIpEndpoint(_Configuration.Indexer.Node, Configuration.Indexer.Network.DefaultPort)),
            //            IPAddress.Parse("127.0.0.1"));
            this.connectionManager.AddNodeAddress(Utils.ParseIpEndpoint(_Configuration.Indexer.Node, Configuration.Indexer.Network.DefaultPort));

            // TODO: Check if needed
            // _Group.NodeConnectionParameters.TemplateBehaviors.Add(new ConnectionManagerBehavior(this.connectionManager, this.loggerFactory));
            // _Group.NodeConnectionParameters.TemplateBehaviors.Add(new ConsensusManagerBehavior(_Chain, this.initialBlockDownloadState, this.consensusManager, this.peerBanning, this.loggerFactory));
            // _Group.NodeConnectionParameters.TemplateBehaviors.Add(new Behavior(this, this.loggerFactory));



            ListenerTrace.Info("Fetching wallet rules...");
            _Wallets = _Configuration.Indexer.CreateIndexerClient().GetAllWalletRules();
            ListenerTrace.Info("Wallet rules fetched");

            ListenerTrace.Info("Fetching wallet subscriptions...");
            _Subscriptions = new SubscriptionCollection(_Configuration.GetSubscriptionsTable().Read());
            ListenerTrace.Info("Subscriptions fetched");

            // TODO: Check if needed
            // _Group.Connect();

            ListenerTrace.Info("Fetching transactions to broadcast...");

            _Disposables.Add(
                Configuration
                .Topics
                .BroadcastedTransactions
                .CreateConsumer("listener", true)
                .EnsureSubscriptionExists()
                .OnMessage((tx, ctl) =>
                {
                    uint256 hash = null;
                    var repo = Configuration.Indexer.CreateIndexerClient();
                    var rejects = Configuration.GetRejectTable();
                    try
                    {
                        hash = tx.Transaction.GetHash();
                        var indexedTx = repo.GetTransaction(hash);
                        ListenerTrace.Info("Broadcasting " + hash);
                        var reject = rejects.ReadOne(hash.ToString());
                        if(reject != null)
                        {
                            ListenerTrace.Info("Abort broadcasting of rejected");
                            return;
                        }

                        if(_Broadcasting.Count > 1000)
                            _Broadcasting.Clear();

                        _Broadcasting.TryAdd(hash, tx.Transaction);
                        if(indexedTx == null || !indexedTx.BlockIds.Any(id => Chain.Contains(id)))
                        {
                            var unused = SendMessageAsync(new InvPayload(tx.Transaction));
                        }

                        var reschedule = new[]
                        {
                            TimeSpan.FromMinutes(5),
                            TimeSpan.FromMinutes(10),
                            TimeSpan.FromHours(1),
                            TimeSpan.FromHours(6),
                            TimeSpan.FromHours(24),
                        };
                        if(tx.Tried <= reschedule.Length - 1)
                        {
                            ctl.RescheduleIn(reschedule[tx.Tried]);
                            tx.Tried++;
                        }
                    }
                    catch(Exception ex)
                    {
                        if(!_Disposed)
                        {
                            LastException = ex;
                            ListenerTrace.Error("Error for new broadcasted transaction " + hash, ex);
                            throw;
                        }
                    }
                }));
            ListenerTrace.Info("Transactions to broadcast fetched");

            _Disposables.Add(_Configuration
                .Topics
                .SubscriptionChanges
                .EnsureSubscriptionExists()
                .AddUnhandledExceptionHandler(ExceptionOnMessagePump)
                .OnMessage(c =>
                {
                    using(_SubscriptionSlimLock.LockWrite())
                    {
                        if(c.Added)
                            _Subscriptions.Add(c.Subscription);
                        else
                            _Subscriptions.Remove(c.Subscription.Id);
                    }
                }));

            _Disposables.Add(_Configuration
                .Topics
                .SendNotifications
                .AddUnhandledExceptionHandler(ExceptionOnMessagePump)
                .OnMessageAsync((n, act) =>
                {
                    return SendAsync(n, act).ContinueWith(t =>
                    {
                        if(!_Disposed)
                        {
                            if(t.Exception != null)
                            {
                                LastException = t.Exception;
                            }
                        }
                    });
                }, new OnMessageOptions
                {
                    MaxConcurrentCalls = 1000,
                    AutoComplete = true,
                    AutoRenewTimeout = TimeSpan.Zero
                }));

            _Disposables.Add(Configuration
               .Topics
               .AddedAddresses
               .CreateConsumer("updater", true)
               .EnsureSubscriptionExists()
               .AddUnhandledExceptionHandler(ExceptionOnMessagePump)
               .OnMessage(evt =>
               {
                   if(evt == null)
                       return;
                   ListenerTrace.Info("New wallet rule");
                   using(_WalletsSlimLock.LockWrite())
                   {
                       foreach(var address in evt)
                       {
                           _Wallets.Add(address.CreateWalletRuleEntry());
                       }
                   }
               }));

        }

        void ExceptionOnMessagePump(Exception ex)
        {
            if(!_Disposed)
            {
                ListenerTrace.Error("Error on azure message pumped", ex);
                LastException = ex;
            }
        }

        NetworkPeerCollection _Group;
        private async Task SendMessageAsync(Payload payload)
        {
            int[] delays = new int[] { 50, 100, 200, 300, 1000, 2000, 3000, 6000, 12000 };
            int i = 0;
            while(_Group.Count != 2)
            {
                i++;
                i = Math.Min(i, delays.Length - 1);
                await Task.Delay(delays[i]).ConfigureAwait(false);
            }
            await _Group.First().SendMessageAsync(payload).ConfigureAwait(false);
        }
        private async Task SendAsync(Notify notify, MessageControl act)
        {
            var n = notify.Notification;
            HttpClient http = new HttpClient();
            var message = new HttpRequestMessage(HttpMethod.Post, n.Subscription.Url);
            n.Tried++;
            var content = new StringContent(n.ToString(), Encoding.UTF8, "application/json");
            message.Content = content;
            CancellationTokenSource tcs = new CancellationTokenSource();
            tcs.CancelAfter(10000);

            var subscription = await Configuration.GetSubscriptionsTable().ReadOneAsync(n.Subscription.Id).ConfigureAwait(false);
            if(subscription == null)
                return;

            bool failed = false;
            try
            {
                var response = await http.SendAsync(message, tcs.Token).ConfigureAwait(false);
                failed = !response.IsSuccessStatusCode;
            }
            catch
            {
                failed = true;
            }
            var tries = new[] 
            { 
                TimeSpan.FromSeconds(0.0),
                TimeSpan.FromMinutes(1.0),
                TimeSpan.FromMinutes(5.0),
                TimeSpan.FromMinutes(30.0),
                TimeSpan.FromMinutes(60.0),
                TimeSpan.FromHours(7.0),
                TimeSpan.FromHours(14.0),
                TimeSpan.FromHours(24.0),
                TimeSpan.FromHours(24.0),
                TimeSpan.FromHours(24.0),
                TimeSpan.FromHours(24.0),
                TimeSpan.FromHours(24.0)
            };

            if(!notify.SendAndForget && failed && (n.Tried - 1) <= tries.Length - 1)
            {
                var wait = tries[n.Tried - 1];
                act.RescheduleIn(wait);
            }
        }

        void TryLock(object obj, Action act)
        {
            if(Monitor.TryEnter(obj))
            {
                try
                {
                    act();
                }
                finally
                {
                    Monitor.Exit(obj);
                }
            }
        }


        private ChainIndexer _Chain;
        public ChainIndexer Chain
        {
            get
            {
                return _Chain;
            }
        }

        public int WalletAddressCount
        {
            get
            {
                using(_WalletsSlimLock.LockRead())
                {
                    return _Wallets.Count;
                }
            }
        }

        ConcurrentDictionary<uint256, Transaction> _Broadcasting = new ConcurrentDictionary<uint256, Transaction>();
        ConcurrentDictionary<uint256, uint256> _KnownInvs = new ConcurrentDictionary<uint256, uint256>();

        uint256 _LastChainTip;

        Task node_MessageReceived(INetworkPeer node, IncomingMessage message)
        {
            if(_KnownInvs.Count == 1000)
                _KnownInvs.Clear();
            if(message.Message.Payload is InvPayload)
            {
                var inv = (InvPayload)message.Message.Payload;
                foreach(var inventory in inv.Inventory)
                {
                    Transaction tx;
                    if(_Broadcasting.TryRemove(inventory.Hash, out tx))
                        ListenerTrace.Info("Broadcasted reached mempool " + inventory);
                }
                var getdata = new GetDataPayload(inv.Inventory.Where(i => i.Type == InventoryType.MSG_TX && _KnownInvs.TryAdd(i.Hash, i.Hash)).ToArray());
                foreach(var data in getdata.Inventory)
                {
                    data.Type = node.AddSupportedOptions(InventoryType.MSG_TX);
                }
                if(getdata.Inventory.Count > 0)
                    node.SendMessageAsync(getdata);
            }
            if(message.Message.Payload is TxPayload)
            {
                var tx = ((TxPayload)message.Message.Payload).Obj;
                ListenerTrace.Verbose("Received Transaction " + tx.GetHash());
                _Indexer.IndexAsync(new TransactionEntry.Entity(tx.GetHash(), tx, null, _Configuration.Indexer.Network))
                        .ContinueWith(HandleException);
                _Indexer.IndexOrderedBalanceAsync(tx)
                    .ContinueWith(HandleException);
                Async(() =>
                {
                    var txId = tx.GetHash();
                    List<OrderedBalanceChange> balances;
                    using(_WalletsSlimLock.LockRead())
                    {
                        balances =
                            OrderedBalanceChange
                            .ExtractWalletBalances(txId, tx, null, null, int.MaxValue, _Wallets, _Configuration.Indexer.Network)
                            .AsEnumerable()
                            .ToList();
                    }
                    UpdateHDState(balances);

                    _Indexer.IndexAsync(balances)
                        .ContinueWith(HandleException);


                    Task notify = null;
                    using(_SubscriptionSlimLock.LockRead())
                    {
                        var topic = Configuration.Topics.SendNotifications;

                        notify = Task.WhenAll(_Subscriptions
                            .GetNewTransactions()
                            .Select(t => topic.AddAsync(new Notify()
                            {
                                SendAndForget = true,
                                Notification = new Notification()
                                {
                                    Subscription = t,
                                    Data = new NewTransactionNotificationData()
                                    {
                                        TransactionId = txId
                                    }
                                }
                            })).ToArray());

                    }
                    notify.Wait();
                });
                var unused = Configuration.Topics.NewTransactions.AddAsync(tx)
                                .ContinueWith(HandleException);
            }

            if(message.Message.Payload is BlockPayload)
            {
                var block = ((BlockPayload)message.Message.Payload).Obj;
                var blockId = block.GetHash();

                List<OrderedBalanceChange> balances = null;
                using(_WalletsSlimLock.LockRead())
                {
                    balances = block.Transactions.SelectMany(tx => OrderedBalanceChange.ExtractWalletBalances(null, tx, null, null, 0, _Wallets, _Configuration.Indexer.Network)).ToList();
                }
                UpdateHDState(balances);
            }

            if(message.Message.Payload is HeadersPayload)
            {
                if(_Chain.Tip.HashBlock != _LastChainTip)
                {
                    var header = _Chain.Tip.Header;
                    _LastChainTip = _Chain.Tip.HashBlock;

                    Configuration.Indexer.CreateIndexer().IndexChain(_Chain);

                    Async(() =>
                    {
                        CancellationTokenSource cancel = new CancellationTokenSource(TimeSpan.FromMinutes(30));
                        var repo = new CacheBlocksRepository(new FullNodeBlocksRepository(new FullNode())); // TODO: check if correct
                        TryLock(_LockBlocks, () =>
                        {
                            new IndexBlocksTask(Configuration.Indexer, this.loggerFactory)
                            {
                                EnsureIsSetup = false,

                            }.Index(new BlockFetcher(_Indexer.GetCheckpoint(IndexerCheckpoints.Blocks), repo, _Chain, null, this.loggerFactory) // TODO: check lastProcessed
                            {
                                CancellationToken = cancel.Token
                            }, _Indexer.TaskScheduler, _Configuration.Indexer.Network);
                        });
                        TryLock(_LockTransactions, () =>
                        {
                            new IndexTransactionsTask(Configuration.Indexer, this.loggerFactory, new AzureIndexerSettings())
                            {
                                EnsureIsSetup = false
                            }
                            .Index(new BlockFetcher(_Indexer.GetCheckpoint(IndexerCheckpoints.Transactions), repo, _Chain, null, loggerFactory) // TODO: check lastProcessed
                            {
                                CancellationToken = cancel.Token
                            }, _Indexer.TaskScheduler, _Configuration.Indexer.Network);
                        });
                        TryLock(_LockWallets, () =>
                        {
                            using(_WalletsSlimLock.LockRead())
                            {
                                new IndexBalanceTask(Configuration.Indexer, _Wallets, loggerFactory)
                                {
                                    EnsureIsSetup = false
                                }
                                .Index(new BlockFetcher(_Indexer.GetCheckpoint(IndexerCheckpoints.Wallets), repo, _Chain, null, loggerFactory) // TODO: check lastProcessed
                                {
                                    CancellationToken = cancel.Token
                                }, _Indexer.TaskScheduler, _Configuration.Indexer.Network);
                            }
                        });
                        TryLock(_LockBalance, () =>
                        {
                            new IndexBalanceTask(Configuration.Indexer, null, loggerFactory)
                            {
                                EnsureIsSetup = false
                            }.Index(new BlockFetcher(_Indexer.GetCheckpoint(IndexerCheckpoints.Balances), repo, _Chain, null, loggerFactory) // TODO: check lastProcessed
                            {
                                CancellationToken = cancel.Token
                            }, _Indexer.TaskScheduler, _Configuration.Indexer.Network);
                        });
                        TryLock(_LockSubscriptions, () =>
                        {
                            using(_SubscriptionSlimLock.LockRead())
                            {
                                new IndexNotificationsTask(Configuration, _Subscriptions, loggerFactory)
                                {
                                    EnsureIsSetup = false,
                                }
                                .Index(new BlockFetcher(_Indexer.GetCheckpointRepository().GetCheckpoint("subscriptions"), repo, _Chain, null, loggerFactory) // TODO: check lastProcessed
                                {
                                    CancellationToken = cancel.Token
                                }, _Indexer.TaskScheduler, _Configuration.Indexer.Network);
                            }
                        });
                        cancel.Dispose();
                        var unused = _Configuration.Topics.NewBlocks.AddAsync(header);
                    });
                }
            }
            if(message.Message.Payload is GetDataPayload)
            {
                var getData = (GetDataPayload)message.Message.Payload;
                foreach(var data in getData.Inventory)
                {
                    Transaction tx = null;
                    if(data.Type == InventoryType.MSG_TX && _Broadcasting.TryGetValue(data.Hash, out tx))
                    {
                        var payload = new TxPayload(tx);
                        node.SendMessageAsync(payload);
                        ListenerTrace.Info("Broadcasted " + data.Hash);
                    }
                }
            }
            if(message.Message.Payload is RejectPayload)
            {
                var reject = (RejectPayload)message.Message.Payload;
                uint256 txId = reject.Hash;
                if(txId != null)
                {
                    ListenerTrace.Info("Broadcasted transaction rejected (" + reject.Code + ") " + txId);
                    if(reject.Code != RejectCode.DUPLICATE)
                    {
                        Configuration.GetRejectTable().Create(txId.ToString(), reject);
                    }
                    Transaction tx;
                    _Broadcasting.TryRemove(txId, out tx);
                }
            }

            return Task.CompletedTask;
        }

        private void UpdateHDState(List<OrderedBalanceChange> balances)
        {
            foreach(var balance in balances)
            {
                UpdateHDState(balance);
            }

        }

        private void UpdateHDState(OrderedBalanceChange entry)
        {
            var repo = Configuration.CreateWalletRepository();
            IDisposable walletLock = null;
            try
            {
                foreach(var matchedAddress in entry.MatchedRules.Select(m => WalletAddress.TryParse(m.Rule.CustomData)).Where(m => m != null))
                {
                    if(matchedAddress.HDKeySet == null)
                        return;
                    var keySet = repo.GetKeySetData(matchedAddress.WalletName, matchedAddress.HDKeySet.Name);
                    if(keySet == null)
                        return;
                    var keyIndex = (int)matchedAddress.HDKey.Path.Indexes.Last();
                    if(keyIndex < keySet.State.NextUnused)
                        return;
                    walletLock = walletLock ?? _WalletsSlimLock.LockWrite();
                    foreach(var address in repo.Scan(matchedAddress.WalletName, keySet, keyIndex + 1, 20))
                    {
                        ListenerTrace.Info("New wallet rule");
                        var walletEntry = address.CreateWalletRuleEntry();
                        _Wallets.Add(walletEntry);
                    }
                }
            }
            finally
            {
                if(walletLock != null)
                    walletLock.Dispose();
            }
        }

        Task Async(Action act)
        {
            var t = new Task(() =>
            {
                try
                {
                    act();
                }
                catch(Exception ex)
                {
                    if(!_Disposed)
                    {
                        ListenerTrace.Error("Error during task.", ex);
                        LastException = ex;
                    }
                }
            });
            t.Start(TaskScheduler.Default);
            return t;
        }

        void HandleException(Task t)
        {
            if(t.IsFaulted)
            {
                if(!_Disposed)
                {
                    ListenerTrace.Error("Error during asynchronous task", t.Exception);
                    LastException = t.Exception;
                }
            }
        }

        public Exception LastException
        {
            get;
            set;
        }

        volatile bool _Disposed;
        #region IDisposable Members

        public void Dispose()
        {
            _Disposed = true;
            foreach(var dispo in _Disposables)
                dispo.Dispose();
            _Disposables.Clear();
            if(LastException == null)
                _Finished.SetResult(true);
            else
                _Finished.SetException(LastException);
        }

        #endregion
        TaskCompletionSource<bool> _Finished = new TaskCompletionSource<bool>();
        public Task Running
        {
            get
            {
                return _Finished.Task;
            }
        }
    }
}
