using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Fap.Hcm.Web.Areas.Employee.Controllers
{
    public class EmpManageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}