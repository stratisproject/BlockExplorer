using System.Threading.Tasks;

namespace stratfaucet.Lib
{
    public interface IWalletUtils
    {
        Task<Balance> GetBalance();
        Task<Transaction> SendCoin(Recipient recipient);
        bool newRecipient(Recipient recipient);

    }
}
