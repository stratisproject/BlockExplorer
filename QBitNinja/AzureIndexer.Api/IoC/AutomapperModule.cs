using Stratis.Bitcoin.Features.AzureIndexer;

namespace AzureIndexer.Api.IoC
{
    using System.Net;
    using Autofac;
    using AutoMapper;
    using Models;
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
                cfg.CreateMap<TransactionResponse, Models.Response.TransactionResponseModel>();
                cfg.CreateMap<Transaction, Models.Response.TransactionModel>();
                cfg.CreateMap<BroadcastResponse, Models.Response.BroadcastResponseModel>();
                cfg.CreateMap<BroadcastError, Models.Response.BroadcastErrorModel>();
                cfg.CreateMap<TransactionResponse, Models.Response.TransactionModel>();
                cfg.CreateMap<BlockInformation, Models.Response.BlockInformationModel>();
                cfg.CreateMap<ICoin, Models.Response.CoinModel>();
                cfg.CreateMap<Money, Models.Response.MoneyModel>();
                //cfg.CreateMap<LockTime, Models.Response.LockTime>();
                cfg.CreateMap<TxIn, Models.Response.TxInModel>();
                cfg.CreateMap<TxOut, Models.Response.TxOutModel>();
                cfg.CreateMap<BlockHeader, Models.Response.BlockHeaderModel>();
                cfg.CreateMap<OutPoint, Models.Response.OutPointModel>();
                cfg.CreateMap<Sequence, Models.Response.SequenceModel>();
                cfg.CreateMap<Script, Models.Response.ScriptModel>();
                cfg.CreateMap<WitScript, Models.Response.WitScriptModel>();
                cfg.CreateMap<Target, Models.Response.TargetModel>();
                cfg.CreateMap<ScriptId, Models.Response.ScriptIdModel>();
                cfg.CreateMap<WitScriptId, Models.Response.WitScriptIdModel>();
                cfg.CreateMap<WalletModel, Models.Response.WalletModel>();
                cfg.CreateMap<Subscription, Models.Response.SubscriptionModel>();
                cfg.CreateMap<BalanceModel, Models.Response.BalanceResponseModel>().ForMember(d => d.Continuation, d => d.MapFrom(s => s.Continuation == null ? null : s.Continuation.ToString()));
                cfg.CreateMap<BalanceOperation, Models.Response.BalanceOperationModel>();
                cfg.CreateMap<InsertWalletAddress, Models.Response.InsertWalletAddress>();
                cfg.CreateMap<IDestination, Models.Response.DestinationModel>();
                cfg.CreateMap<WalletAddress, Models.Response.WalletAddressModel>();
                cfg.CreateMap<HDKeySet, Models.Response.HDKeySetModel>();
                cfg.CreateMap<HDKeyData, Models.Response.HDKeyDataModel>();
                cfg.CreateMap<BitcoinExtPubKey, Models.Response.BitcoinExtPubKeyModel>();
                cfg.CreateMap<KeyPath, Models.Response.KeyPathModel>();
                cfg.CreateMap<BitcoinAddress, Models.Response.BitcoinAddressModel>();
                cfg.CreateMap<ExtPubKey, Models.Response.ExtPubKeyModel>();
                cfg.CreateMap<Network, Models.Response.NetworkModel>();
                cfg.CreateMap<PubKey, Models.Response.PubKeyModel>();
                cfg.CreateMap<IConsensus, Models.Response.ConsensusModel>();
                cfg.CreateMap<NetworkAddress, Models.Response.NetworkAddressModel>();
                cfg.CreateMap<DNSSeedData, Models.Response.DNSSeedDataModel>();
                cfg.CreateMap<CheckpointInfo, Models.Response.CheckpointInfoModel>();
                cfg.CreateMap<Bech32Encoder, Models.Response.Bech32EncoderModel>();
                cfg.CreateMap<KeyId, Models.Response.KeyIdModel>();
                cfg.CreateMap<WitKeyId, Models.Response.WitKeyIdModel>();
                cfg.CreateMap<ConsensusOptions, Models.Response.ConsensusOptionsModel>();
                cfg.CreateMap<IBIP9DeploymentsArray, Models.Response.IBIP9DeploymentsArray>();
                cfg.CreateMap<BigInteger, Models.Response.BigIntegerModel>();
                cfg.CreateMap<IPEndPoint, Models.Response.IPEndPointModel>();
                cfg.CreateMap<IPAddress, Models.Response.IPAddressModel>();
                cfg.CreateMap<KeySetData, Models.Response.KeySetDataModel>();
                cfg.CreateMap<HDKeyState, Models.Response.HDKeyStateModel>();
                cfg.CreateMap<BalanceSummary, Models.Response.BalanceSummaryModel>();
                cfg.CreateMap<BalanceSummaryDetails, Models.Response.BalanceSummaryDetailsModel>();
                cfg.CreateMap<AssetBalanceSummaryDetails, Models.Response.AssetBalanceSummaryDetailsModel>();
                cfg.CreateMap<BitcoinAssetId, Models.Response.BitcoinAssetIdModel>();
                cfg.CreateMap<WhatIsBlockHeader, Models.Response.WhatIsBlockHeaderModel>();
                cfg.CreateMap<VersionStatsResponse, Models.Response.VersionStatsResponseModel>();
                cfg.CreateMap<VersionStats, Models.Response.VersionStatsModel>();
                cfg.CreateMap<VersionStatsItem, Models.Response.VersionStatsItemModel>();
            });

            IMapper mapper = new Mapper(config);
            builder.RegisterInstance(mapper).As<IMapper>();
        }
    }
}
