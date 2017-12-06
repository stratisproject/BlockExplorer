using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Refit;
using FaucetSite.request;

namespace FaucetSite.Lib
{
    public class WalletUtils : IWalletUtils
    {
        private IConfiguration _config;

        private String apiUrl;

        private string password;

        private string accountName;

        private string walletName;

        private IStratisWalletAPI stratApi;

        public WalletUtils(IConfiguration config)
        {
            _config = config;
            apiUrl = (_config["Faucet:FullNodeApiUrl"] != null ? _config["Faucet:FullNodeApiUrl"] : "http://127.0.0.1:37220");
            password = _config["Faucet:FullNodePassword"];
            accountName = _config["Faucet:FullNodeAccountName"];
            walletName = _config["Faucet:FullNodeWalletName"];

            stratApi = RestService.For<IStratisWalletAPI>(apiUrl, new RefitSettings { });
        }
        public async Task<Transaction> SendCoin(string address)
        {
            BuildTransaction buildTransaction = new BuildTransaction
            {
                WalletName = walletName,
                AccountName = accountName,
                CoinType = 105,
                Password = password,
                DestinationAddress = address,
                Amount = 100m,
                FeeType = "low",
                AllowUnconfirmed = true
            };

            var transaction = await stratApi.BuildTransaction(buildTransaction);

            var resp = await stratApi.SendTransaction(new SendTransaction { Hex = transaction.Hex });

            return new Transaction
            {
                TransactionId = transaction.TransactionId
            };

        }
    }
}
