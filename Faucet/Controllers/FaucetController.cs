using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using stratfaucet.Lib;

namespace stratfaucet.Controllers

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
        public async Task<Balance> GetBalance() {
            return await walletUtils.GetBalance();
        }

        [HttpPost("SendCoin")]
        public async Task<Transaction> SendCoin([FromBody] Recipient address) {
            return await walletUtils.SendCoin(address);
        }
    }
}
