using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using DBreeze;
using NBitcoin;
using NBitcoin.Protocol;
using NBitcoin.RPC;
using Newtonsoft.Json.Linq;
using Stratis.Bitcoin.Builder;
using Stratis.Bitcoin.Configuration;
using Stratis.Bitcoin.Features.Consensus;
using Stratis.Bitcoin.Features.RPC;
using Xunit;

namespace Stratis.Bitcoin.Features.IndexStore.Tests
{
    public class IndexStoreTests
    {
        /// <summary>
        /// Creates a full node with data folder set to the test folder.
        /// </summary>
        /// <param name="folderName">Data folder to use.</param>
        /// <param name="setup">Specifies startup options for the index store.</param>
        /// <returns></returns>
        private IFullNode CreateTestNode([CallerMemberName]string folderName = null, Action<IndexSettings> setup = null)
        {
            if (System.IO.Directory.Exists(folderName))
                System.IO.Directory.Delete(folderName, true);

            System.IO.Directory.CreateDirectory(folderName);

            NodeSettings nodeSettings = new NodeSettings("stratis", Network.StratisTest, ProtocolVersion.ALT_PROTOCOL_VERSION);
            nodeSettings = nodeSettings.LoadArguments(new string[] { $"-datadir={folderName}" });

            return new FullNodeBuilder()
                .UseNodeSettings(nodeSettings)
                .UseStratisConsensus()
                .UseIndexStore(setup)
                .AddRPC(s =>
                {
                    s.RpcUser = "test";
                    s.RpcPassword = "test";
                })
                .Build();
        }

        /// <summary>
        /// Creates an RPC client for the specified full node.
        /// </summary>
        /// <param name="fullNode">The fill node to create the RPC client for.</param>
        /// <returns>An RPC client created for the specified full node.</returns>
        private RPCClient CreateRPCClient(IFullNode fullNode)
        {
            var rpcSettings = fullNode.NodeService<RpcSettings>();
            System.Net.IPEndPoint nodeEndPoint = rpcSettings.Bind.Where(b => b.Address.ToString() == "127.0.0.1").FirstOrDefault() ?? rpcSettings.Bind[0];
            return new RPCClient($"{rpcSettings.RpcUser}:{rpcSettings.RpcPassword}", new Uri($"http://{nodeEndPoint}"), fullNode.Network);
        }

        /// <summary>
        /// Tests whether new indexes can be added via RPC.
        /// </summary>
        [Fact]
        public void CanCreateIndexFromRPC()
        {
            using (var node = this.CreateTestNode())
            {
                node.Start();
                var client = this.CreateRPCClient(node);
                var response = bool.Parse((string)client.SendCommand("createindex", "Output", false,
                    "(t,b,n) => t.Inputs.Select((i, N) => new object[] { new object[] { i.PrevOut.Hash, i.PrevOut.N }, t.GetHash() })").Result);

                Assert.True(response);
            }
        }

        /// <summary>
        /// Tests whether indexes can be dropped via RPC.
        /// </summary>
        [Fact]
        public void CanDropIndexFromRPC()
        {
            using (var node = this.CreateTestNode())
            {
                node.Start();
                var client = this.CreateRPCClient(node);
                bool response1 = bool.Parse((string)client.SendCommand("createindex", "Output", false,
                    "(t,b,n) => t.Inputs.Select((i, N) => new object[] { new object[] { i.PrevOut.Hash, i.PrevOut.N }, t.GetHash() })").Result);
                bool response2 = bool.Parse((string)client.SendCommand("dropindex", "Output").Result);

                Assert.True(response1);
                Assert.True(response2);
            }
        }

        /// <summary>
        /// Tests whether indexes can be listed via RPC.
        /// </summary>
        [Fact]
        public void CanListIndexesFromRPC()
        {
            using (var node = this.CreateTestNode())
            {
                node.Start();
                var client = this.CreateRPCClient(node);
                bool response1 = bool.Parse((string)client.SendCommand("createindex", "Output", false,
                    "(t,b,n) => t.Inputs.Select((i, N) => new object[] { new object[] { i.PrevOut.Hash, i.PrevOut.N }, t.GetHash() })").Result);
                bool response2 = bool.Parse((string)client.SendCommand("createindex", "Script", true,
                    "(t,b,n) => t.Outputs.Select((o, N) => new { Item = o, Index = N }).Where(o => o.Item.ScriptPubKey.GetDestinationAddress(n) != null).Select(o => new object[] { new uint160(o.Item.ScriptPubKey.Hash.ToBytes()), new object[] { t.GetHash(), (uint)o.Index } })").Result);
                var result = client.SendCommand("listindexnames").Result?.ToObject<JArray>();

                Assert.True(response1);
                Assert.True(response2);
                Assert.Equal(2, result?.Count);
                Assert.Contains("Output", result);
                Assert.Contains("Script", result);
            }
        }

        /// <summary>
        /// Tests whether indexes can be described via RPC.
        /// </summary>
        [Fact]
        public void CanDescribeIndexFromRPC()
        {
            using (var node = this.CreateTestNode())
            {
                node.Start();
                var client = this.CreateRPCClient(node);
                var expr = "(t,b,n) => t.Inputs.Select((i, N) => new object[] { new object[] { i.PrevOut.Hash, i.PrevOut.N }, t.GetHash() })";
                bool response = bool.Parse((string)client.SendCommand("createindex", "Output", false, expr).Result);
                string description = (string)client.SendCommand("describeindex", "Output").Result?.ToObject<JArray>()?[0];

                Assert.True(response);
                Assert.Equal("{\"Name\":\"Output\",\"Table\":\"Index_Output\",\"Builder\":\"" + expr + "\",\"Many\":false,\"Uses\":[\"System\",\"System.Linq\",\"System.Linq.Expressions\",\"System.Collections.Generic\",\"NBitcoin\"]}", description);
            }
        }

        /// <summary>
        /// Tests whether a configured single-value index is automatically updated.
        /// </summary>
        [Fact]
        public void CanRegisterSingleValueIndexFromIndexStoreSettings()
        {
            using (var node = this.CreateTestNode(setup: settings =>
            {
                settings.RegisterIndex("Output", "(t,b,n) => t.Inputs.Select((i, N) => new object[] { new object[] { i.PrevOut.Hash, i.PrevOut.N }, t.GetHash() })", false);
            }))
            {
                node.Start();

                // Transaction has outputs
                var block = new Block();
                var trans = new Transaction();
                Key key = new Key(); // generate a random private key
                var scriptPubKeyOut = PayToPubkeyHashTemplate.Instance.GenerateScriptPubKey(key.PubKey);
                trans.Outputs.Add(new TxOut(100, scriptPubKeyOut));
                block.Transactions.Add(trans);
                var hash = trans.GetHash().ToBytes();

                // Transaction has inputs (i.PrevOut)
                var block2 = new Block();
                block2.Header.HashPrevBlock = block.GetHash();
                var trans2 = new Transaction();
                trans2.Inputs.Add(new TxIn(new OutPoint(trans, 0)));
                block2.Transactions.Add(trans2);
                var hash2 = trans2.GetHash().ToBytes();

                var repository = node.NodeService<IIndexRepository>() as IndexRepository;

                repository.PutAsync(block.GetHash(), new List<Block> { block, block2 }).GetAwaiter().GetResult();

                var indexTable = repository.Indexes["Output"].Table;
                var expectedJSON = repository.Indexes["Output"].ToString();

                repository.Dispose();

                using (var engine = new DBreezeEngine(node.NodeService<DataFolder>().IndexPath))
                {
                    var transaction = engine.GetTransaction();

                    var indexKeyRow = transaction.Select<string, string>("Common", indexTable);
                    Assert.True(indexKeyRow.Exists && indexKeyRow.Value != null);
                    Assert.Equal(expectedJSON, indexKeyRow.Value);

                    // Block 2 has been indexed?
                    var indexKey = new byte[hash.Length + 4];
                    hash.CopyTo(indexKey, 0);
                    var IndexedRow = transaction.Select<byte[], byte[]>(indexTable, indexKey);
                    Assert.True(IndexedRow.Exists);
                    // Correct value indexed?
                    var compare = new byte[32];
                    hash2.CopyTo(compare, 0);
                    Assert.Equal(compare, IndexedRow.Value);
                }
            }
        }

        /// <summary>
        /// Tests whether the RPC method "createindex" can be called and returns the expected string result suitable for console output.
        /// We are also testing whether all arguments can be passed as strings.
        /// </summary>
        [Fact]
        public void CanCreateIndexByStringArgs()
        {
            using (var node = this.CreateTestNode())
            {
                node.Start();

                RPCClient rpc = this.CreateRPCClient(node);

                var args = new List<string>();
                args.Add("testindex");
                args.Add("false");
                args.Add("(t,b,n) => t.Inputs.Select((i, N) => new object[] { new object[] { i.PrevOut.Hash, i.PrevOut.N }, t.GetHash() })");
                var resp = rpc.SendCommand("createindex", args.ToArray()).ResultString;

                Assert.Equal("True", resp);
            }
        }
    }
}
