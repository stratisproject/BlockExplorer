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
    using Stratis.Bitcoin.Features.PoA.BasePoAFeatureConsensusRules;
    using Stratis.Bitcoin.Features.RPC;
    using Stratis.Bitcoin.Features.SmartContracts;
    using Stratis.Bitcoin.Features.SmartContracts.PoA;
    using Stratis.Bitcoin.Features.SmartContracts.Wallet;
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
                   nodeSettings = new NodeSettings(networksSelector: FederatedPegNetwork.NetworksSelector, protocolVersion: ProtocolVersion.ALT_PROTOCOL_VERSION, args: args);
                   node = new FullNodeBuilder()
                       .UseNodeSettings(nodeSettings)
                       .UseBlockStore()
                       .UseMempool()
                       .AddSmartContracts()
                       .UseSmartContractPoAConsensus()
                       .UseSmartContractPoAMining()
                       .UseSmartContractWallet()
                       .UseReflectionExecutor()
                       .UseApi()
                       .AddRPC()
                       .UseAzureIndexer()
                       .Build();
                }
                else
                {
                    nodeSettings = new NodeSettings(networksSelector: Stratis.Bitcoin.Networks.Networks.Stratis, protocolVersion: ProtocolVersion.PROVEN_HEADER_VERSION, args: args);
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
                System.Console.WriteLine("There was a problem initializing the node. Details: '{0}'", ex.Message);
            }
        }
    }
}
