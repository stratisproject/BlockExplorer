using System;
using System.Linq;
using System.Threading.Tasks;
using NBitcoin.Protocol;
using Stratis.Bitcoin;
using Stratis.Bitcoin.Builder;
using Stratis.Bitcoin.Configuration;
using Stratis.Bitcoin.Features.Api;
using Stratis.Bitcoin.Features.BlockStore;
using Stratis.Bitcoin.Features.Consensus;
using Stratis.Bitcoin.Features.MemoryPool;
using Stratis.Bitcoin.Features.RPC;
using Stratis.Bitcoin.Features.SmartContracts;
using Stratis.Bitcoin.Features.SmartContracts.PoA;
using Stratis.Bitcoin.Features.SmartContracts.Wallet;
using Stratis.Bitcoin.Networks;
using Stratis.Bitcoin.Utilities;
using Stratis.Features.AzureIndexer.Helpers;
using Stratis.Features.Collateral;
using Stratis.Features.Collateral.CounterChain;
using Stratis.Features.SQLiteWalletRepository;
using Stratis.Sidechains.Networks;

namespace Stratis.IndexerD
{
    public class Program
    {
        public static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }

        public static async Task MainAsync(string[] args)
        {
            var isSideChain = false;

            try
            {
                NodeSettings nodeSettings;
                IFullNode node = null;
                if (args.Length > 0)
                {
                    isSideChain = args.Contains("-sidechain");
                }

                if (isSideChain)
                {
                    nodeSettings = new NodeSettings(networksSelector: CirrusNetwork.NetworksSelector, protocolVersion: ProtocolVersion.CIRRUS_MIN_SUPPORTED_VERSION, args: args)
                    {
                        MinProtocolVersion = ProtocolVersion.CIRRUS_MIN_SUPPORTED_VERSION
                    };

                    node = new FullNodeBuilder()
                        .UseNodeSettings(nodeSettings)
                        .UseBlockStore()
                        .AddRPC()
                        .AddSmartContracts(options =>
                        {
                            options.UseReflectionExecutor();
                            options.UsePoAWhitelistedContracts();
                        })
                        .AddPoAFeature()
                        .UsePoAConsensus()
                        .CheckCollateralCommitment()
                        .SetCounterChainNetwork(StraxNetwork.MainChainNetworks[nodeSettings.Network.NetworkType]())
                        .UseSmartContractWallet()
                        .AddSQLiteWalletRepository()
                        .UseAzureIndexerOnSideChain()
                        .UseApi()
                        .UseMempool()
                        .Build();
                }
                else
                {
                    nodeSettings = new NodeSettings(networksSelector: Networks.Strax, protocolVersion: ProtocolVersion.PROVEN_HEADER_VERSION, args: args)
                    {
                        MinProtocolVersion = ProtocolVersion.PROVEN_HEADER_VERSION
                    };

                    node = new FullNodeBuilder()
                        .UseNodeSettings(nodeSettings)
                        .UseBlockStore()
                        .UsePosConsensus()
                        .UseMempool()
                        .UseApi()
                        .AddRPC()
                        .UseAzureIndexer()
                        .AddSQLiteWalletRepository()
                        .Build();
                }

                // Run node.
                if (node != null)
                {
                    await node.RunAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("There was a problem initializing the node. Details: '{0}'", ex.Message);
            }
        }
    }
}
