using FaucetSite.Lib;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace FaucetSite.Controllers

{
    [Route("api/[controller]")]
    public class FaucetController : Controller
    {
        private TransactionQueue transactionQueue;
        private IConfiguration config;
        private ICaptchaClient googleCaptcha;

        public FaucetController(IConfiguration config, ICaptchaClient googleCaptcha, TransactionQueue transactionQueue)
        {
            this.config = config;
            this.googleCaptcha = googleCaptcha;
            this.transactionQueue = transactionQueue;
        }


        [Route("/Error")]
        public IActionResult Error()
        {
            ViewData["RequestId"] = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            return View();
        }

        [HttpPost("SendCoin")]
        public async Task<IActionResult> SendCoin(Recipient model)
        {
            var ipAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            var secretKey = config["Captcha:SecretKey"];
            var response = await googleCaptcha.VerifyAsync(secretKey, model.Captcha, ipAddress);

            if (!response.Success)
            {
                return BadRequest(new { ErrorMessage = "Invalid Captcha" });
            }

            var task = transactionQueue.EnqueueAsync(model.Address);

            string transactionId = null;
            if (transactionQueue.Count < 3) // dont wait queued item if it has more than 3 items in queue
            {
                await Task.WhenAny(task, Task.Delay(TimeSpan.FromSeconds(20))); // Wait some for completion

                transactionId = task.IsCompletedSuccessfully ? task.Result.TransactionId : null;
            }

            return Json(new { TransactionId = transactionId });
        }
    }
}
