using System.Threading.Tasks;

namespace FaucetSite.Lib
{
    public interface IWalletUtils
    {

        Task<Recipient> SendCoin(Recipient recipient);
    }
}
