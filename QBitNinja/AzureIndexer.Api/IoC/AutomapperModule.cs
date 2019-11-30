using System;

namespace AzureIndexer.Api.IoC
{
    using System.Linq;
    using System.Net;
    using Autofac;
    using AutoMapper;
    using CommonServiceLocator;
    using Infrastructure;
    using Models;
    using Models.Response;
    using NBitcoin;
    using NBitcoin.BouncyCastle.Math;
    using NBitcoin.DataEncoders;
    using NBitcoin.OpenAsset;
    using NBitcoin.Protocol;

    public class AutomapperModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TransactionResponse, TransactionResponseModel>();
                cfg.CreateMap<Transaction, TransactionModel>();
                cfg.CreateMap<BroadcastResponse, BroadcastResponseModel>();
                cfg.CreateMap<BroadcastError, BroadcastErrorModel>();
                cfg.CreateMap<TransactionResponse, TransactionModel>();
                cfg.CreateMap<BlockInformation, BlockInformationModel>();
                cfg.CreateMap<ICoin, CoinModel>();
                cfg.CreateMap<Money, MoneyModel>();
                cfg.CreateMap<TxIn, TxInModel>();
                cfg.CreateMap<TxOut, TxOutModel>();
                cfg.CreateMap<WhatIsAddress, WhatIsAddressModel>();
                cfg.CreateMap<WhatIsScript, WhatIsScriptModel>();
                cfg.CreateMap<BlockHeader, BlockHeaderModel>();
                cfg.CreateMap<OutPoint, OutPointModel>();
                cfg.CreateMap<WhatIsPublicKey, WhatIsPublicKeyModel>();
                cfg.CreateMap<WhatIsSmartContract, WhatIsSmartContractModel>();
                cfg.CreateMap<Sequence, SequenceModel>();
                cfg.CreateMap<Script, ScriptModel>().ConvertUsing<ScriptTypeConverter>();
                cfg.CreateMap<TransactionResponseModel, TransactionSummaryModel>().ConvertUsing<TransactionResponseSummaryConverter>();
                cfg.CreateMap<Target, TargetModel>();
                cfg.CreateMap<ScriptId, ScriptIdModel>();
                cfg.CreateMap<WitScriptId, WitScriptIdModel>();
                cfg.CreateMap<WalletModel, WalletResponseModel>();
                cfg.CreateMap<Subscription, SubscriptionModel>();
                cfg.CreateMap<BalanceModel, BalanceResponseModel>()
                    .ForMember(d => d.Continuation, d => d.MapFrom(s => s.Continuation == null ? null : s.Continuation.ToString()));
                cfg.CreateMap<BalanceOperation, BalanceOperationModel>();
                cfg.CreateMap<InsertWalletAddress, InsertWalletAddressModel>();
                cfg.CreateMap<IDestination, DestinationModel>();
                cfg.CreateMap<WalletAddress, WalletAddressModel>();
                cfg.CreateMap<HDKeySet, HDKeySetModel>();
                cfg.CreateMap<HDKeyData, HDKeyDataModel>();
                cfg.CreateMap<BitcoinExtPubKey, BitcoinExtPubKeyModel>();
                cfg.CreateMap<KeyPath, KeyPathModel>();
                cfg.CreateMap<BitcoinAddress, BitcoinAddressModel>();
                cfg.CreateMap<ExtPubKey, ExtPubKeyModel>();
                cfg.CreateMap<Network, NetworkModel>();
                cfg.CreateMap<PubKey, PubKeyModel>();
                cfg.CreateMap<IConsensus, ConsensusModel>();
                cfg.CreateMap<NetworkAddress, NetworkAddressModel>();
                cfg.CreateMap<DNSSeedData, DNSSeedDataModel>();
                cfg.CreateMap<CheckpointInfo, CheckpointInfoModel>();
                cfg.CreateMap<Bech32Encoder, Bech32EncoderModel>();
                cfg.CreateMap<KeyId, KeyIdModel>();
                cfg.CreateMap<WitKeyId, WitKeyIdModel>();
                cfg.CreateMap<ConsensusOptions, ConsensusOptionsModel>();
                cfg.CreateMap<IBIP9DeploymentsArray, IBIP9DeploymentsArrayModel>();
                cfg.CreateMap<BigInteger, BigIntegerModel>();
                cfg.CreateMap<IPEndPoint, IPEndPointModel>();
                cfg.CreateMap<IPAddress, IPAddressModel>();
                cfg.CreateMap<KeySetData, KeySetDataModel>();
                cfg.CreateMap<HDKeyState, HDKeyStateModel>();
                cfg.CreateMap<BalanceSummary, BalanceSummaryModel>();
                cfg.CreateMap<BalanceSummaryDetails, BalanceSummaryDetailsModel>();
                cfg.CreateMap<AssetBalanceSummaryDetails, AssetBalanceSummaryDetailsModel>();
                cfg.CreateMap<BitcoinAssetId, BitcoinAssetIdModel>();
                cfg.CreateMap<BlockHeaderResponse, WhatIsBlockHeaderModel>();
                cfg.CreateMap<VersionStatsResponse, VersionStatsResponseModel>();
                cfg.CreateMap<VersionStats, VersionStatsModel>();
                cfg.CreateMap<VersionStatsItem, VersionStatsItemModel>();
                cfg.CreateMap<BlockResponse, BlockResponseModel>();
                cfg.CreateMap<ExtendedBlockInformation, ExtendedBlockInformationModel>();
                cfg.CreateMap<Block, BlockModel>().ForMember(m => m.Transactions, d => d.Ignore()).ForMember(m => m.TransactionIds, d => d.MapFrom(m => m.Transactions.Select(t => t.GetHash().ToString())));
                cfg.CreateMap<BlockHeaderResponse, BlockHeaderResponseModel>();
            });

            IMapper mapper = new Mapper(config);
            builder.RegisterInstance(mapper).As<IMapper>();
        }

        public class ScriptTypeConverter : ITypeConverter<Script, ScriptModel>
        {
            public ScriptModel Convert(Script source, ScriptModel destination, ResolutionContext context)
            {
                var network = ServiceLocator.Current.GetInstance<QBitNinjaConfiguration>().Indexer.Network;
                string address = string.Empty;
                var isScript = source.IsPayToScriptHash(network);
                if (isScript)
                {
                    if (PayToScriptHashTemplate.Instance.CheckScriptPubKey(source))
                    {
                        address = source.GetDestinationAddress(network).ToString();
                    }
                }
                else
                {
                    address = source.GetAddressFromScript(network);
                }

                var scriptModel = new ScriptModel
                {
                    Addresses = new[] { address },
                    HasCanonicalPushes = source.HasCanonicalPushes,
                    Hash = source.ToString(),
                    IsPushOnly = source.IsPushOnly,
                    IsUnspendable = source.IsUnspendable,
                    IsValid = source.IsValid,
                    Length = source.Length,
                    PaymentScriptHash = source.PaymentScript?.ToString()
                };

                return scriptModel;
            }
        }

        public class TransactionResponseSummaryConverter : ITypeConverter<TransactionResponseModel, TransactionSummaryModel>
        {
            public TransactionSummaryModel Convert(TransactionResponseModel source, TransactionSummaryModel destination, ResolutionContext context) {
                var chain = ServiceLocator.Current.GetInstance<ChainIndexer>();
                var summary = new TransactionSummaryModel {
                    Hash = source.TransactionId,
                    Time = source.Block?.BlockHeader?.Time ?? source.Transaction.Time,
                    Confirmations = (chain.Tip.Height - source.Block?.Height ?? 0) + 1,
                    Fee = source.Fees,
                    Amount = source.Transaction.TotalOut,
                    Height = source.Block?.Height,
                    Spent = source.Transaction.TotalOut.Satoshi < 0,
                    IsCoinbase = source.Transaction.IsCoinBase ?? false,
                    IsCoinstake = source.Transaction.IsCoinStake ?? false,
                    IsSmartContract = source.IsSmartContract,
                    In = source.SpentCoins.Select(coin => {
                        return new BlockTransactionIn {
                            Address = coin.TxOut.ScriptPubKey.Addresses.FirstOrDefault(),
                            PrevOut = coin.Outpoint,
                            Amount = coin.TxOut.Value.Satoshi
                        };
                    }).ToList(),
                    Out = source.ReceivedCoins.Select((coin, index) => {
                        return new BlockTransactionOut {
                            Address = coin.TxOut.ScriptPubKey.Addresses.FirstOrDefault(),
                            Amount = coin.TxOut.Value.Satoshi,
                            N = index,
                            IsUnspendable = coin.TxOut.ScriptPubKey.IsUnspendable.GetValueOrDefault(false)
                        };
                    }).ToList(),
                    FirstSeen = source.FirstSeen
                };

                return summary;
            }
        }

        public class TransactionIdConverter : ITypeConverter<Transaction, string>
        {
            public string Convert(Transaction source, string destination, ResolutionContext context)
            {
                var chain = ServiceLocator.Current.GetInstance<ChainIndexer>();
                var summary = source.GetHash().ToString();

                return summary;
            }
        }
    }
}
