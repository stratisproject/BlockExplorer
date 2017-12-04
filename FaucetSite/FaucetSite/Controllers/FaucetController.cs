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
        static AsyncLocker locker = new AsyncLocker();
        private IWalletUtils walletUtils;
        private IConfiguration config;
        private ICaptchaClient googleCaptcha;

        public FaucetController(IConfiguration config, ICaptchaClient googleCaptcha)
        {
            walletUtils = new WalletUtils(config);
            this.config = config;
            this.googleCaptcha = googleCaptcha;
        }


        [Route("/Error")]
        public IActionResult Error()
        {
            ViewData["RequestId"] = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            return View();
        }

        [HttpPost("SendCoin")]
        public async Task<IActionResult> SendCoin([FromBody] Recipient model)
        {
            var ipAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            var secretKey = config["Captcha:SecretKey"];
            var response = await googleCaptcha.VerifyAsync(secretKey, model.Captcha, ipAddress);

            if (!response.Success)
            {
                return BadRequest(new { ErrorMessage = "Invalid Captcha" });
            }

            using (await locker.LockAsync())
            {
                return Ok(await walletUtils.SendCoin(model));
            }
        }
    }
}
