using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fap.AspNetCore.Infrastructure;
using Fap.AspNetCore.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace Fap.Hcm.Web.Areas.Recruit.Controllers
{
    [Area("Recruit")]
    public class ManageController : FapController
    {
        public ManageController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
        public IActionResult Demand()
        {
            JqGridViewModel model = this.GetJqGridModel("RcrtDemand", (qs) =>
            {
                qs.GlobalWhere = "BillStatus='PASSED'";
            });
            return View(model);
        }



    }
}