using Stratis.Bitcoin.Features.SmartContracts;
using Stratis.Bitcoin.Features.SmartContracts.PoA;
using Stratis.Bitcoin.Features.SmartContracts.Wallet;
using Stratis.SmartContracts.Networks;

namespace Stratis.Bitcoin.Indexer.Console
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using NBitcoin;
    using NBitcoin.Protocol;
    using Stratis.Bitcoin.Builder;
    using Stratis.Bitcoin.Configuration;
    using Stratis.Bitcoin.Features.AzureIndexer;
    using Stratis.Bitcoin.Features.BlockStore;
    using Stratis.Bitcoin.Features.Consensus;
    using Stratis.Bitcoin.Utilities;

    public class Program
    {
        public static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }

        public static async Task MainAsync(string[] args)
        {
            try
            {
                // NodeSettings nodeSettings =
                 NodeSettings nodeSettings = new NodeSettings(new SmartContractsPoATest(), ProtocolVersion.ALT_PROTOCOL_VERSION, "StratisSC", args: args); //new NodeSettings(networksSelector: Networks.Networks.Stratis, protocolVersion: ProtocolVersion.PROVEN_HEADER_VERSION, "StratisSC", args: args);

                IFullNode node = new FullNodeBuilder()
                    .UseNodeSettings(nodeSettings)
                    .UsePosConsensus()
                    .AddSmartContracts()
                    .UseSmartContractPoAConsensus()
                    //.UseSmartContractPoAMining()
                    //.UseSmartContractWallet()
                    .UseReflectionExecutor()
                    //.UseApi()
                    .UseBlockStore()
                    .UseAzureIndexer()
                    .Build();

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
