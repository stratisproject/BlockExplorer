using System.Threading.Tasks;

namespace FaucetSite.Lib
{
    public interface IWalletUtils
    {
        Task<Transaction> SendCoin(string address);
    }
}
