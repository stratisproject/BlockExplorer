using System.Threading.Tasks;

namespace FaucetSite.Lib
{
    public interface IWalletUtils
    {
        Task SendCoin(string address);
    }
}
