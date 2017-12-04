using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FaucetSite.Lib
{
    public interface ICaptchaClient
    {
        [Post("/recaptcha/api/siteverify")]
        Task<VerifyResponse> VerifyAsync(string secret, string response, string remoteip);
    }

    public class VerifyResponse
    {
        public bool Success { get; set; }
    }
}
