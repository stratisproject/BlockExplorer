using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using FaucetSite.Lib;
using FaucetSite.Models;

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

        [HttpPost("SendCoin")]
        public IActionResult SendCoin([FromBody] Recipient recipient)
        {
            try
            {
                recipient.ip_address = HttpContext.Connection.RemoteIpAddress.MapToIPv4().GetAddressBytes().ToString();

                if (Throttling.Transactions.Count > 100)
                {
                    throw new FaucetException("Too many faucet users");
                }

                Throttling.Transactions.GetOrAdd(recipient.address, recipient);
                return new JsonResult(Throttling.WaitForTransaction(recipient.address));

            }
            catch (FaucetException ex)
            {

                // TODO figure out the right way to do this for knockout::
                throw new ApplicationException( ex.Message );
            }
        }
    }
}
