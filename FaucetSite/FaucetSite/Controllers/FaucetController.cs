using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using FaucetSite.Lib;

namespace FaucetSite.Controllers

{
    [Route("api/[controller]")]
    public class FaucetController : Controller
    {
        private IWalletUtils walletUtils;

        public FaucetController(IConfiguration config)
        {
            walletUtils = new WalletUtils(config);
        }


        [Route("/Error")]
        public IActionResult Error()
        {
            ViewData["RequestId"] = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            return View();
        }

        [HttpGet("GetBalance")]
        public async Task<Balance> GetBalance()
        {
          return await walletUtils.GetBalance();
        }

        [HttpPost("SendCoin")]
        public Recipient SendCoin([FromBody] Recipient recipient)
        {

            recipient.ip_address = HttpContext.Connection.RemoteIpAddress.MapToIPv4().GetAddressBytes().ToString();

            if (Throttling.Transactions.Count > 100)
            {
                throw new FaucetException("Too many faucet users");
            }

            Throttling.Transactions.GetOrAdd(recipient.address, recipient);
            return Throttling.WaitForTransaction(recipient.address);
        }
    }
}
