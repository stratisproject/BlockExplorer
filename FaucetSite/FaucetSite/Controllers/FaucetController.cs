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
        private readonly IConfiguration config;

        public FaucetController(IConfiguration config)
        {
            walletUtils = new WalletUtils(config);
            this.config = config;
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
        public async Task<Transaction> SendCoin([FromBody] Recipient model)
        {
            return await walletUtils.SendCoin(model);
        }

        [HttpGet("Configs")]
        public IActionResult Configs()
        {
            var apiUrl = config["Faucet:FullNodeApiUrl"];
            var password = config["Faucet:FullNodePassword"];

            return Json(new { apiUrl, password });
        }
    }
}
