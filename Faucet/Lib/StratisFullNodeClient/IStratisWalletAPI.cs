using System;
using System.Threading.Tasks;
using Refit;
using stratfaucet.request;

namespace stratfaucet
{

    public interface IStratisWalletAPI
    {
        [Get("/api/wallet/balance?walletname={walletname}&cointype={cointype}")]
        Task<response.Balances> GetBalance(string walletname, int cointype = 0);

        [Post("/api/wallet/create")]
        Task<String> CreateWallet([Body] CreateWallet createWallet);


        [Post("/api/wallet/build-transaction")]
        Task<response.Transaction> BuildTransaction([Body] BuildTransaction createWallet);

        [Post("/api/wallet/send-transaction")]
        Task<String> SendTransaction([Body] SendTransaction createWallet);


        [Get("/api/wallet/address?walletName={walletName}&accountName={accountName}&coinType=<{coinType}")]
        Task<String> GetUnusedAddress(string walletName, string accountName, int coinType);


    }
}
