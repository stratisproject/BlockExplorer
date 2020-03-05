using NBitcoin;
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
        private readonly TokenIdentifier tokenIdentifier;

        public TokenDetailProvider(IReceiptRepository receiptRepository, IStateRepositoryRoot state)
        {
            this.receiptRepository = receiptRepository;
            this.state = state;
            this.tokenIdentifier = new TokenIdentifier(state); // TODO inject
        }

        public TokenTransferDetails Get(int blockHeight, uint256 txHash)
        {
            Receipt receipt = this.receiptRepository.Retrieve(txHash);

            // Ignore non-SC transactions or failed transactions
            if (receipt == null || receipt.Success == false)
            {
                return null;
            }

            // Ignore non token-transfers
            if (!this.tokenIdentifier.IsToken(receipt))
            {
                return null;
            }

            return new TokenTransferDetails();
        }
    }
}