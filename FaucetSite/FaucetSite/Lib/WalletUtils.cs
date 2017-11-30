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
        private decimal coinDivisor = 100000000M;

        private string ERROR_WALLET = "An error occurred when contacting the wallet";

        private string ERROR_DUPE = "We have already sent you coins";
        public WalletUtils(IConfiguration config)
        {
            _config = config;
            apiUrl = (_config["Faucet:FullNodeApiUrl"] != null ? _config["Faucet:FullNodeApiUrl"] : "http://127.0.0.1:37220");
            password = _config["Faucet:FullNodePassword"];
            accountName = _config["Faucet:FullNodeAccountName"];
            walletName = _config["Faucet:FullNodeWalletName"];

            stratApi = RestService.For<IStratisWalletAPI>(apiUrl, new RefitSettings { });
        }

        public WalletUtils() {
            
        }
        public async Task<Balance> GetBalance()
        {
            try
            {
                var bal = await stratApi.GetBalance(walletName);
                var address = await stratApi.GetUnusedAddress(walletName, accountName, 0);
                return new Balance
                {
                    balance = (bal.BalancesList.First().AmountConfirmed / coinDivisor),
                    returnAddress = address.Substring(address.IndexOf("\"") + 1, address.LastIndexOf("\"") - 1)
                };
            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
                return new Balance
                {
                    balance = 0
                };
            }
        }
        private void UpdateRecipient(Recipient recipient)
        {
            Throttling.Transactions.AddOrUpdate(recipient.address, recipient,  (key, oldValue) => recipient);
        }

        public async Task<Recipient> SendCoin(Recipient recipient)
        {
            var amount = 10m;

            if (newRecipient(recipient))
            {
                try
                {
                    BuildTransaction buildTransaction = new BuildTransaction
                    {
                        WalletName = walletName,
                        AccountName = accountName,
                        CoinType = 105,
                        Password = password,
                        DestinationAddress = recipient.address,
                        Amount = amount,
                        FeeType = "low",
                        AllowUnconfirmed = true
                    };
                    var transaction = await stratApi.BuildTransaction(buildTransaction);

                    SendTransaction sendTransaction = new SendTransaction
                    {
                        Hex = transaction.Hex
                    };

                    var resp = await stratApi.SendTransaction(sendTransaction);
                   
                    recipient.is_sent = true;
                    UpdateRecipient(recipient);

                    Throttling.AddressesSeen.Enqueue(recipient.address);
                    Throttling.IPAddressesSeen.Enqueue(recipient.ip_address);

                    return recipient;
                }
                catch (Refit.ApiException exc)
                {
                    Console.WriteLine(exc.ToString());
                    Console.WriteLine(exc.Content);
                    recipient.is_error = true;
                    UpdateRecipient(recipient);
                    throw new FaucetException(ERROR_WALLET);
                }

            }
            else
            {
                throw new FaucetException(ERROR_DUPE);
            }
        }
        public bool newRecipient(Recipient recipient)
        {
            if (Throttling.IPAddressesSeen.Contains(recipient.ip_address) || Throttling.AddressesSeen.Contains(recipient.address))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

    }
}
