using System.Threading.Tasks;

namespace FaucetSite.Lib
{
    public interface IWalletUtils
    {
        Task<Balance> GetBalance();
        Task<Recipient> SendCoin(Recipient recipient);
        bool newRecipient(Recipient recipient);

    }
}
