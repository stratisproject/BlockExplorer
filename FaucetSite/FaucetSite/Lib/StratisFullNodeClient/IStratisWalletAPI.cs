using System;
using System.Threading.Tasks;
using Refit;
using FaucetSite.request;

namespace FaucetSite
{

    public interface IStratisWalletAPI
    {
        [Post("/api/wallet/build-transaction")]
        Task<response.Transaction> BuildTransaction([Body] BuildTransaction createWallet);

        [Post("/api/wallet/send-transaction")]
        Task<String> SendTransaction([Body] SendTransaction createWallet);
    }
}
