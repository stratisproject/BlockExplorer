using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using AzureIndexer.Api.Infrastructure;
using AzureIndexer.Api.Models;
using Microsoft.Extensions.Logging;
using NBitcoin;
using Stratis.Bitcoin.AsyncWork;
using Stratis.Bitcoin.Connection;
using Stratis.Bitcoin.Consensus;
using Stratis.Bitcoin.Interfaces;
using Stratis.Bitcoin.P2P.Peer;
using Stratis.Bitcoin.P2P.Protocol.Behaviors;
using Stratis.Bitcoin.P2P.Protocol.Payloads;
using Stratis.Features.AzureIndexer;
using Stratis.Features.AzureIndexer.Entities;
using Stratis.Features.AzureIndexer.Tokens;
using Stratis.SmartContracts.Core.Receipts;
using Stratis.SmartContracts.Core.State;
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
                if (node.State == NetworkPeerState.HandShaked)
                {
                    ListenerTrace.Info("Node handshaked");
                    // AttachedPeer.MessageReceived.Register(_Listener.node_MessageReceived);
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
                if (reason == null)
                {
                    return null;
                }

                return reason.Reason + " " + reason.Exception == null ? "" : Utils.ExceptionToString(reason.Exception);
            }

            protected override void DetachCore()
            {
                AttachedPeer.StateChanged.Unregister(AttachedNode_StateChanged);
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
        private readonly IReceiptRepository receiptRepository;
        private readonly IStateRepositoryRoot state;
        private readonly LogDeserializer logDeserializer;
        private readonly ServiceBusClient serviceBusClient;
        private readonly IAsyncProvider asyncProvider;
        SubscriptionCollection _Subscriptions = null;
        ReaderWriterLock _SubscriptionSlimLock = new ReaderWriterLock();

        private Stratis.Features.AzureIndexer.WalletRuleEntryCollection _Wallets;
        ReaderWriterLock _WalletsSlimLock = new ReaderWriterLock();

        object _LockBalance = new object();
        object _LockTransactions = new object();
        object _LockBlocks = new object();
        object _LockWallets = new object();
        object _LockSubscriptions = new object();
        object _LockTokenTransactions = new object();

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
            ILoggerFactory loggerFactory,
            IReceiptRepository receiptRepository,
            IStateRepositoryRoot state,
            LogDeserializer logDeserializer,
            ServiceBusClient serviceBusClient,
            IAsyncProvider asyncProvider)
        {
            _Configuration = configuration;
            this.initialBlockDownloadState = initialBlockDownloadState;
            this.consensusManager = consensusManager;
            this.connectionManager = connectionManager;
            this.peerBanning = peerBanning;
            this.loggerFactory = loggerFactory;
            this.receiptRepository = receiptRepository;
            this.state = state;
            this.logDeserializer = logDeserializer;
            this.serviceBusClient = serviceBusClient;
            this.asyncProvider = asyncProvider;
        }

        private Stratis.Features.AzureIndexer.AzureIndexer _Indexer;

        public Stratis.Features.AzureIndexer.AzureIndexer Indexer
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
            if (chain == null)
            {
                chain = new ChainIndexer(_Configuration.Indexer.Network);
            }

            _Chain = chain;
            ListenerTrace.Info("Fetching headers from " + _Chain.Tip.Height + " (from azure)");
            IndexerClient client = Configuration.Indexer.CreateIndexerClient();
            client.SynchronizeChain(chain);
            ListenerTrace.Info("Headers fetched tip " + _Chain.Tip.Height);

            _Disposables.Add(_IndexerScheduler = new CustomThreadPoolTaskScheduler(50, 100, "Indexing Threads"));
            _Indexer.TaskScheduler = _IndexerScheduler;

            _Group = this.connectionManager.ConnectedPeers;
            this.connectionManager.AddNodeAddress(Utils.ParseIpEndpoint(_Configuration.Indexer.Node, Configuration.Indexer.Network.DefaultPort));

            ListenerTrace.Info("Fetching wallet rules...");
            _Wallets = _Configuration.Indexer.CreateIndexerClient().GetAllWalletRules();
            ListenerTrace.Info("Wallet rules fetched");

            ListenerTrace.Info("Fetching wallet subscriptions...");
            _Subscriptions = new SubscriptionCollection(_Configuration.GetSubscriptionsTable().Read());
            ListenerTrace.Info("Subscriptions fetched");

            // TODO: Check if needed
            // _Group.Connect();

            ListenerTrace.Info("Fetching transactions to broadcast...");

            var receiver =
                serviceBusClient.CreateReceiver(nameof(BroadcastedTransaction), nameof(BroadcastedTransaction));

            var sender =
                serviceBusClient.CreateSender(nameof(BroadcastedTransaction));

            var asyncLoop = this.asyncProvider.CreateAndRunAsyncLoop(nameof(QBitNinjaNodeListener), async token =>
                {
                    var message = await receiver.ReceiveMessageAsync(cancellationToken: token);

                    var tx = message.Body.ToObjectFromJson<BroadcastedTransaction>();

                    uint256 hash = null;
                    IndexerClient repo = Configuration.Indexer.CreateIndexerClient();
                    CrudTable<RejectPayload> rejects = Configuration.GetRejectTable();
                    try
                    {
                        hash = tx.Transaction.GetHash();
                        TransactionEntry indexedTx = repo.GetTransaction(hash);
                        ListenerTrace.Info("Broadcasting " + hash);
                        RejectPayload reject = rejects.ReadOne(hash.ToString());
                        if (reject != null)
                        {
                            ListenerTrace.Info("Abort broadcasting of rejected");
                            return;
                        }

                        if (_Broadcasting.Count > 1000)
                        {
                            _Broadcasting.Clear();
                        }

                        _Broadcasting.TryAdd(hash, tx.Transaction);
                        if (indexedTx == null || !indexedTx.BlockIds.Any(id => Chain.Contains(id)))
                        {
                            Task unused = SendMessageAsync(new InvPayload(tx.Transaction));
                        }

                        TimeSpan[] reschedule =
                        {
                            TimeSpan.FromMinutes(5),
                            TimeSpan.FromMinutes(10),
                            TimeSpan.FromHours(1),
                            TimeSpan.FromHours(6),
                            TimeSpan.FromHours(24),
                        };

                        if (tx.Tried <= reschedule.Length - 1)
                        {
                            var serviceBusMessage = new ServiceBusMessage(message.Body);
                            await sender.ScheduleMessageAsync(serviceBusMessage, DateTimeOffset.UtcNow.Add(reschedule[tx.Tried]));
                            tx.Tried++;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (!_Disposed)
                        {
                            LastException = ex;
                            ListenerTrace.Error("Error for new broadcasted transaction " + hash, ex);
                            throw;
                        }
                    }

                    await receiver.CompleteMessageAsync(message, token);
                },
                default);

            _Disposables.Add(asyncLoop);
            ListenerTrace.Info("Transactions to broadcast fetched");
        }

        IReadOnlyNetworkPeerCollection _Group;

        private async Task SendMessageAsync(Payload payload)
        {
            int[] delays = new int[] { 50, 100, 200, 300, 1000, 2000, 3000, 6000, 12000 };
            int i = 0;
            while (_Group.Count() < 2)
            {
                i++;
                i = Math.Min(i, delays.Length - 1);
                await Task.Delay(delays[i]).ConfigureAwait(false);
            }

            await _Group.First().SendMessageAsync(payload).ConfigureAwait(false);
        }

        void TryLock(object obj, Action act)
        {
            if (Monitor.TryEnter(obj))
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

        public ChainIndexer Chain => _Chain;

        ConcurrentDictionary<uint256, Transaction> _Broadcasting = new ConcurrentDictionary<uint256, Transaction>();

        uint256 _LastChainTip;

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
            foreach (IDisposable dispo in _Disposables)
            {
                dispo.Dispose();
            }

            _Disposables.Clear();
            if (LastException == null)
            {
                _Finished.SetResult(true);
            }
            else
            {
                _Finished.SetException(LastException);
            }
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
