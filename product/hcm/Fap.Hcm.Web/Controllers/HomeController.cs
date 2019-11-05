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

namespace Fap.Hcm.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConnectionFactory _userService1;
        private readonly IUser _userService;
        public HomeController(ILogger<HomeController> logger,IConnectionFactory userService1,IUser user)
        {
            _logger = logger;
            _userService1 = userService1;
            _userService = user;
        }

        public IActionResult Index()
        {
            ViewBag.CC = _userService.Get("zhangsan");
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
