using Stratis.Bitcoin.Features.AzureIndexer.Helpers;

namespace Stratis.IndexerD
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using NBitcoin.Protocol;
    using Stratis.Bitcoin;
    using Stratis.Bitcoin.Builder;
    using Stratis.Bitcoin.Configuration;
    using Stratis.Bitcoin.Features.Api;
    using Stratis.Bitcoin.Features.AzureIndexer;
    using Stratis.Bitcoin.Features.BlockStore;
    using Stratis.Bitcoin.Features.Consensus;
    using Stratis.Bitcoin.Features.MemoryPool;
    using Stratis.Bitcoin.Features.RPC;
    using Stratis.Bitcoin.Features.SmartContracts;
    using Stratis.Bitcoin.Features.SmartContracts.PoA;
    using Stratis.Bitcoin.Features.SmartContracts.Wallet;
    using Stratis.Bitcoin.Networks;
    using Stratis.Bitcoin.Utilities;
    using Stratis.Sidechains.Networks;

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
                        .UseSmartContractPoAConsensus()
                        .UseSmartContractPoAMining()
                        .UseSmartContractWallet()
                        .UseApi()
                        .UseMempool()
                        .UseAzureIndexer()
                        .Build();
                }
                else
                {
                    nodeSettings = new NodeSettings(networksSelector: Networks.Stratis, protocolVersion: ProtocolVersion.PROVEN_HEADER_VERSION, args: args)
                    {
                        MinProtocolVersion = ProtocolVersion.ALT_PROTOCOL_VERSION
                    };
                    node = new FullNodeBuilder()
                        .UseNodeSettings(nodeSettings)
                        .UseBlockStore()
                        .UsePosConsensus()
                        .UseMempool()
                        .UseApi()
                        .AddRPC()
                        .UseAzureIndexer()
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
