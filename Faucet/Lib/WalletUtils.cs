using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Refit;
using stratfaucet.request;

namespace stratfaucet.Lib
{
    public class WalletUtils : IWalletUtils
    {
      private readonly string _password;
        private readonly string _accountName;
        private readonly string _walletName;
        private readonly int _coinType;
        private readonly IStratisWalletAPI _stratApi;

        private decimal coinDivisor = 100000000M;
        public WalletUtils(IConfiguration config)
        {
            var apiUrl = config["Faucet:FullNodeApiUrl"] ?? "http://127.0.0.1:37220";
            _password = config["Faucet:FullNodePassword"];
            _accountName = config["Faucet:FullNodeAccountName"];
            _walletName = config["Faucet:FullNodeWalletName"];
            _coinType = int.Parse(config["Faucet:FullNodeCoinType"]);

            _stratApi = RestService.For<IStratisWalletAPI>(apiUrl, new RefitSettings());
        }
        public async Task<Balance> GetBalance()
        {
            try
            {
                var bal = await _stratApi.GetBalance(_walletName);
                var address = await _stratApi.GetUnusedAddress(_walletName, _accountName, 0);
                return new Balance
                {
                    balance = (bal.BalancesList.First().AmountConfirmed / coinDivisor),
                    returnAddress = address.Substring(
                      address.IndexOf("\"", StringComparison.OrdinalIgnoreCase) + 1,
                      address.LastIndexOf("\"", StringComparison.OrdinalIgnoreCase) - 1)
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
        public async Task<Transaction> SendCoin(Recipient recipient)
        {
            var amount = (await GetBalance()).balance / 100;

            var buildTransaction = new BuildTransaction
            {
                WalletName = _walletName,
                AccountName = _accountName,
                CoinType = _coinType,
                Password = _password,
                DestinationAddress = recipient.address,
                Amount = amount.ToString(CultureInfo.InvariantCulture),
                FeeType = "low",
                AllowUnconfirmed = true
            };
            var transaction = await _stratApi.BuildTransaction(buildTransaction);

            var sendTransaction = new SendTransaction
            {
                Hex = transaction.Hex
            };

          var resp =  await _stratApi.SendTransaction(sendTransaction);
          Console.Write(resp);

          return new Transaction
          {
              confirmation = transaction.TransactionId
          };
        }
        public bool newRecipient(Recipient recipient)
        {
            return true;
        }
    }
}
