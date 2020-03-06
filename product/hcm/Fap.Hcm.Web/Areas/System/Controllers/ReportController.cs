using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fap.AspNetCore.Infrastructure;
using Fap.AspNetCore.ViewModel;
using Fap.ExcelReport;
using Microsoft.AspNetCore.Mvc;

namespace Fap.Hcm.Web.Areas.System.Controllers
{
    [Area("System")]
    public class ReportController : FapController
    {
        
        public ReportController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
           
        }

        public IActionResult Template()
        {
            var model = GetJqGridModel("RptSimpleTemplate");

            return View(model);
        }
        public IActionResult Render()
        {
            return View();
        }

    }
}