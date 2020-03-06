using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using NBitcoin;
using Stratis.Bitcoin.Features.SmartContracts.Models;
using Stratis.SmartContracts.CLR;
using Stratis.SmartContracts.Core.Receipts;
using Stratis.SmartContracts.Core.State;

namespace Stratis.Bitcoin.Features.AzureIndexer.Tokens
{
    public class TokenTransferDetails
    {
        public string From { get; set; }

        public string To { get; set; }

        public string TokenAddress { get; set; }

        public string TokenSymbol { get; set; }

        public string Amount { get; set; }
    }

    /// <summary>
    /// Returns information about a token transfer.
    /// </summary>
    public class TokenDetailProvider
    {
        private readonly IReceiptRepository receiptRepository;
        private readonly IStateRepositoryRoot state;
        private readonly LogDeserializer logDeserializer;
        private readonly TokenIdentifier tokenIdentifier;

        public TokenDetailProvider(IReceiptRepository receiptRepository, IStateRepositoryRoot state, LogDeserializer logDeserializer)
        {
            this.receiptRepository = receiptRepository;
            this.state = state;
            this.logDeserializer = logDeserializer;
            this.tokenIdentifier = new TokenIdentifier(state); // TODO inject
        }

        /// <summary>
        /// Returns an ordered list of all the token transactions that occurred in a particular transaction.
        /// </summary>
        /// <param name="txHash"></param>
        /// <returns></returns>
        public List<TokenTransferDetails> Get(uint256 txHash)
        {
            var result = new List<TokenTransferDetails>();

            Receipt receipt = this.receiptRepository.Retrieve(txHash);

            // Ignore non-SC transactions or failed transactions
            if (receipt == null || receipt.Success == false)
            {
                return result;
            }

            // TODO test case for logs with internal token transfers

            // Look for transfer log events.
            foreach (Log log in receipt.Logs)
            {
                if (log.Topics.Count == 0)
                {
                    continue;
                }

                // Get receipt struct name
                string eventTypeName = Encoding.UTF8.GetString(log.Topics[0]);

                // Ignore non-transfer events.
                if (eventTypeName != nameof(TransferLog))
                {
                    continue;
                }

                // Deserialize it. If it fails we have an event with the right name but the wrong fields or data types.
                try
                {
                    dynamic deserializedLog = this.logDeserializer.DeserializeLogData(log.Data, typeof(TransferLog));

                    result.Add(new TokenTransferDetails
                    {
                        From = deserializedLog.From,
                        To = deserializedLog.To,
                        Amount = deserializedLog.Amount.ToString(),
                        TokenAddress = log.Address.ToBase58Address(this.logDeserializer.Network)
                    });
                }
                catch (Exception e)
                {
                }
            }

            return result;
        }
    }
}