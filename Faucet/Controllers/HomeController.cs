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
    public class HomeController : Controller
    {

        // [Route("/")]
        // [Route("Index")]
        public IActionResult Index()
        {
            return View();
        }

    }

}
