using Stratis.SmartContracts.Core.Receipts;
using Stratis.SmartContracts.Core.State;

namespace Stratis.Bitcoin.Features.AzureIndexer.Tokens
{
    public interface ITokenIdentifier
    {
        bool IsToken(Receipt receipt);
    }

    /// <summary>
    /// Identifies whether or not something is a token.
    ///
    /// TODO cache known token addresses in memory.
    /// </summary>
    public class TokenIdentifier : ITokenIdentifier
    {
        private readonly IStateRepositoryRoot state;

        public TokenIdentifier(IStateRepositoryRoot state)
        {
            this.state = state;
        }

        public bool IsToken(Receipt receipt)
        {
            // TODO base this off the actual interface. For now, just use any calls that are TransferFrom or TransferTo.
            return receipt.MethodName == "TransferFrom" || receipt.MethodName == "TransferTo";
        }
    }
}