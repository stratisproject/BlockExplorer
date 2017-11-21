using NBitcoin;
using NBitcoin.Protocol;
using Stratis.Bitcoin.Builder;
using Stratis.Bitcoin.Configuration;
using Stratis.Bitcoin.Features.Consensus;
using Stratis.Bitcoin.Features.IndexStore;
using Stratis.Bitcoin.Features.MemoryPool;
using Stratis.Bitcoin.Utilities;
using System.Linq;
using System.Threading.Tasks;

namespace Stratis.IndexStoreD
{
    public class Program
    {
        public static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }

        public static async Task MainAsync(string[] args)
        {
            Network network = args.Contains("-testnet") ? Network.StratisTest : Network.StratisMain;
            NodeSettings nodeSettings = new NodeSettings( "stratis", network, ProtocolVersion.ALT_PROTOCOL_VERSION);
            nodeSettings = nodeSettings.LoadArguments(args);

            // NOTES: running BTC and STRAT side by side is not possible yet as the flags for serialization are static

            var node = new FullNodeBuilder()
                .UseNodeSettings(nodeSettings)
                .UseStratisConsensus()
                .UseIndexStore()
                .UseMempool()
                .Build();

            await node.RunAsync();
        }
    }
}