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
using Fap.AspNetCore.Infrastructure;

namespace Fap.Hcm.Web.Controllers
{
    public class HomeController : FapController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ILoginService _loginService;
        private readonly IRbacService _rbacService;
        public HomeController(IServiceProvider serviceProvider,ILoginService loginService,IRbacService rbacService) : base(serviceProvider)
        {
            _logger = _loggerFactory.CreateLogger<HomeController>();
            _loginService = loginService;
            _rbacService = rbacService;
        }

        public IActionResult Index()
        {
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
