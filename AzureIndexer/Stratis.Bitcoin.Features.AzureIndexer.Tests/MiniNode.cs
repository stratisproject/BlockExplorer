using NBitcoin.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBitcoin;

namespace Stratis.Bitcoin.Features.AzureIndexer.Tests
{
	public class MiniNode
	{
        // TODO: Find a NodeServer replacement and fix this code
        /*
		public MiniNode(IndexerTester tester, NodeServer server)
		{
			_Generator = new ChainBuilder(tester);
			_Server = server;
			server.AllMessages.AddMessageListener(new NewThreadMessageListener<IncomingMessage>(ProcessMessage));
		}

		private readonly NodeServer _Server;
		public NodeServer Server
		{
			get
			{
				return _Server;
			}
		}
        private readonly ChainBuilder _Generator;
        public ChainBuilder ChainBuilder
		{
			get
			{
				return _Generator;
			}
		}


		private void ProcessMessage(IncomingMessage message)
		{
			var getheader = message.Message.Payload as GetHeadersPayload;
			if(getheader != null)
			{
				ChainedBlock forkPos = null;
				int height = 0;
				foreach(var blk in getheader.BlockLocators.Blocks)
				{
					forkPos = ChainBuilder.Chain.GetBlock(blk);
					if(forkPos != null)
						break;
				}
				if(forkPos != null)
					height = forkPos.Height + 1;

				HeadersPayload getData = new HeadersPayload();
				while(height <= ChainBuilder.Chain.Height)
				{
					var block = ChainBuilder.Chain.GetBlock(height);
					getData.Headers.Add(block.Header);
					if(block.HashBlock == getheader.HashStop)
						break;
					height++;
				}
				message.Node.SendMessage(getData);
			}

			var mempool = message.Message.Payload as MempoolPayload;
			if(mempool != null)
			{
				var inv = ChainBuilder.Mempool.Select(kv => new InventoryVector()
									{
										Type = InventoryType.MSG_TX,
										Hash = kv.Key
									}).ToList();
				var payload = new InvPayload();
				payload.Inventory.AddRange(inv);
				message.Node.SendMessage(payload);
			}

			var gettx = message.Message.Payload as GetDataPayload;
			if(gettx != null)
			{
				foreach(var inv in gettx.Inventory)
				{
					if(inv.Type == InventoryType.MSG_TX)
					{
                        message.Node.SendMessage(new TxPayload(ChainBuilder.Mempool[inv.Hash]));
					}
                    if (inv.Type == InventoryType.MSG_BLOCK)
                    {
                        message.Node.SendMessage(new BlockPayload(ChainBuilder.Blocks[inv.Hash]));
                    }
				}
			}
		}
        */
	}
}
