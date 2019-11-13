using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Fap.Hcm.Web.Models;
using Fap.Core;
using Fap.Core.DataAccess;
using Fap.Core.Rbac;

namespace Fap.Hcm.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ILoginService _userService1;
       
        public HomeController(ILogger<HomeController> logger, ILoginService userService1)
        {
            _logger = logger;
            _userService1 = userService1;
        }

        public IActionResult Index()
        {
            ViewBag.CC = _userService1.Login("hr");
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
