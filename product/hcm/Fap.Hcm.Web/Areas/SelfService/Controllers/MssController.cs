using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fap.AspNetCore.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Fap.Hcm.Web.Areas.SelfService.Controllers
{
    [Area("SelfService")]
    public class MssController : FapController
    {
        public MssController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}