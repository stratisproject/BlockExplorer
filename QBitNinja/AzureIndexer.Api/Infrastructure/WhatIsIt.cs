using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AzureIndexer.Api.Controllers;
using AzureIndexer.Api.Models;
using AzureIndexer.Api.Models.Response;
using NBitcoin;
using NBitcoin.DataEncoders;

namespace AzureIndexer.Api.Infrastructure
{
    public class WhatIsIt
    {
        public WhatIsIt(MainController controller)
        {
            this.Controller = controller;
        }

        public MainController Controller { get; set; }

        public Network Network => this.Controller.Network;

        public QBitNinjaConfiguration Configuration => this.Controller.Configuration;


        public async Task<object> Find(string data, IMapper mapper)
        {
            data = data.Trim();
            var b58 = this.NoException(() => WhatIsBase58.GetFromBitcoinString(data));
            if (b58 != null)
            {
                if (b58 is WhatIsAddress address)
                {
                    this.TryFetchRedeemOrPubKey(address);
                }

                return mapper.Map<WhatIsAddressModel>(b58);
            }

            if (data.Length == 0x40)
            {
                try
                {
                    return await this.Controller.JsonTransaction(uint256.Parse(data), false);
                }
                catch
                {
                }
            }

            var b = this.NoException(() => this.Controller.JsonBlock(BlockFeature.Parse(data), true, false));
            if (b != null)
            {
                return b;
            }

            // Hash of pubkey or script
            if (data.Length == 0x28)
            {
                TxDestination dest = new KeyId(data);
                var address = new WhatIsAddress(dest.GetAddress(this.Network));
                if (this.TryFetchRedeemOrPubKey(address))
                {
                    return mapper.Map<WhatIsAddressModel>(address);
                }

                dest = new ScriptId(data);
                address = new WhatIsAddress(dest.GetAddress(this.Network));
                if (this.TryFetchRedeemOrPubKey(address))
                {
                    return mapper.Map<WhatIsAddressModel>(address);
                }
            }

            var script = this.NoException(() => GetScriptFromBytes(data));
            if (script != null)
            {
                var scriptModel = new WhatIsScript(script, this.Network);
                return mapper.Map<WhatIsScriptModel>(scriptModel);
            }

            script = this.NoException(() => GetScriptFromText(data));
            if (script != null)
            {
                var scriptModel = new WhatIsScript(script, this.Network);
                return mapper.Map<WhatIsScriptModel>(scriptModel);
            }

            var sig = this.NoException(() => new TransactionSignature(Encoders.Hex.DecodeData(data)));
            if (sig != null)
            {
                return new WhatIsTransactionSignature(sig);
            }

            var pubkeyBytes = this.NoException(() => Encoders.Hex.DecodeData(data));
            if (pubkeyBytes != null && PubKey.Check(pubkeyBytes, true))
            {
                var pubKey = this.NoException(() => new PubKey(data));
                if (pubKey != null)
                {
                    var pubKeyModel = new WhatIsPublicKey(pubKey, this.Network);
                    return mapper.Map<WhatIsPublicKeyModel>(pubKeyModel);
                }
            }

            if (data.Length == 80 * 2)
            {
                var blockHeader = this.NoException(() =>
                {
                    var h = new BlockHeader();
                    h.ReadWrite(Encoders.Hex.DecodeData(data));
                    return h;
                });
                if (blockHeader != null)
                {
                    return new WhatIsBlockHeader(blockHeader);
                }
            }

            return null;
        }

        private static Script GetScriptFromText(string data)
        {
            if (!data.Contains(' '))
            {
                return null;
            }

            return GetScriptFromBytes(Encoders.Hex.EncodeData(new Script(data).ToBytes(true)));
        }

        private static Script GetScriptFromBytes(string data)
        {
            var bytes = Encoders.Hex.DecodeData(data);
            var script = Script.FromBytesUnsafe(bytes);
            bool hasOps = false;
            var reader = script.CreateReader();
            foreach (var op in reader.ToEnumerable())
            {
                hasOps = true;
                if (op.IsInvalid || (op.Name == "OP_UNKNOWN" && op.PushData == null))
                    return null;
            }
            return !hasOps ? null : script;
        }


        private bool TryFetchRedeemOrPubKey(WhatIsAddress address)
        {
            if (address.IsP2SH)
            {
                address.RedeemScript = TryFetchRedeem(address);
                return address.RedeemScript != null;
            }
            address.PublicKey = TryFetchPublicKey(address);
            return address.PublicKey != null;
        }


        private Script FindScriptSig(WhatIsAddress address)
        {
            var indexer = Configuration.Indexer.CreateIndexerClient();
            var scriptSig = indexer
                            .GetOrderedBalance(address.ScriptPubKey.Raw)
                            .Where(b => b.SpentCoins.Count != 0)
                            .Select(b => new
                            {
                                SpentN = b.SpentIndices[0],
                                Tx = indexer.GetTransaction(b.TransactionId)
                            })
                            .Where(o => o.Tx != null)
                            .Select(o => o.Tx.Transaction.Inputs[o.SpentN].ScriptSig)
                            .FirstOrDefault();
            return scriptSig;
        }

        private WhatIsScript TryFetchRedeem(WhatIsAddress address)
        {
            var scriptSig = FindScriptSig(address);
            if (scriptSig == null)
                return null;
            var result = PayToScriptHashTemplate.Instance.ExtractScriptSigParameters(this.Network, scriptSig);
            return result == null ? null : new WhatIsScript(result.RedeemScript, Network);
        }

        private WhatIsPublicKey TryFetchPublicKey(WhatIsAddress address)
        {
            var scriptSig = FindScriptSig(address);
            if (scriptSig == null)
                return null;
            var result = PayToPubkeyHashTemplate.Instance.ExtractScriptSigParameters(this.Network, scriptSig);
            return result == null ? null : new WhatIsPublicKey(result.PublicKey, Network);
        }

        public T NoException<T>(Func<T> act) where T : class
        {
            try
            {
                return act();
            }
            catch
            {
                return null;
            }
        }
    }
}
