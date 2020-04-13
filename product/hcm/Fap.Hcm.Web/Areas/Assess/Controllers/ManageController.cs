using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fap.AspNetCore.Infrastructure;
using Fap.AspNetCore.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace Fap.Hcm.Web.Areas.Assess.Controllers
{
    /// <summary>
    /// 绩效评估
    /// </summary>
    [Area("Assess")]
    public class ManageController : FapController
    {
        public ManageController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
        /// <summary>
        /// 考核方案
        /// </summary>
        /// <returns></returns>
        public IActionResult Scheme()
        {
            JqGridViewModel model = this.GetJqGridModel("PerfProgram");
            return View(model);
        }
        /// <summary>
        /// 考核指标
        /// </summary>
        /// <param name="fid">方案Uid</param>
        /// <param name="prm">方案名称</param>
        /// <returns></returns>
        public ActionResult KPI(string fid, string prm)
        {
            JqGridViewModel model = this.GetJqGridModel("PerfKPIs",
                qs =>
                {
                    qs.InitWhere = "ProgramUid=@PrmUid";
                    qs.AddParameter("PrmUid",
                        fid );
                }
                );
            model.TempData.Add("prmUid", fid);
            model.TempData.Add("prmName", prm);
            ViewBag.PrmUid = fid;
            ViewBag.PrmName = prm;
            return View(model);
        }
    }
}