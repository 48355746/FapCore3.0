using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fap.AspNetCore.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Fap.Hcm.Web.Areas.Employee.Controllers
{
    [Area("Employee")]
    public class ManageController : FapController
    {
        public ManageController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public IActionResult EmpIndex()
        {
            var model = this.GetJqGridModel("Employee");
            ViewBag.EmpCategory = _dbContext.Dictionarys("EmpCategory");
            ViewBag.EmpStatus = _dbContext.Dictionarys("EmpStatus");
            return View(model);
        }
    }
}