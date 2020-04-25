using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fap.AspNetCore.Infrastructure;
using Fap.AspNetCore.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Fap.Core.Extensions;
using Ardalis.GuardClauses;

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
            var cols= _dbContext.Columns("RcrtDemand").Where(c => !c.ColProperty.EqualsWithIgnoreCase("3"))?.Select(c=>c.ColName);
            Guard.Against.Null(cols, nameof(cols));
            JqGridViewModel model = this.GetJqGridModel("RcrtDemand", (qs) =>
            {
                qs.QueryCols =string.Join(',',cols);
                qs.GlobalWhere = "BillStatus='PASSED'";
            });
            return View(model);
        }



    }
}