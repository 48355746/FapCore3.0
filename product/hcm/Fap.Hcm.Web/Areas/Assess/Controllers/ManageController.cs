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
            JqGridViewModel model = this.GetJqGridModel("PerfKPIs", qs =>
                 {
                     qs.InitWhere = "ProgramUid=@PrmUid";
                     qs.AddParameter("PrmUid",
                         fid);
                 });
            model.TempData.Add("prmUid", fid);
            model.TempData.Add("prmName", prm);
            ViewBag.PrmUid = fid;
            ViewBag.PrmName = prm;
            return View(model);
        }
        /// <summary>
        /// 考核对象
        /// </summary>
        /// <param name="id">方案UID</param>
        /// <param name="prm">方案名称</param>
        /// <returns></returns>
        public ActionResult Objectives(string id, string prm)
        {
            //PerfObjectType ot = _perfService.GetObjectTypeByPrmUid(id);
            //string objTableName = "Employee";
            //string displayCols = "EmpCategory,EmpCode,EmpName,DeptUid,EmpPosition";
            //if (ot != null && ot.Entity.IsNotNullOrEmpty())
            //{
            //    objTableName = ot.Entity;
            //    displayCols = ot.DisplayProp;
            //}
            //MultiJqGridViewModel model = new MultiJqGridViewModel();

            //JqGridViewModel jmObject = this.GetJqGridModel("PerfObject", qs =>
            //{
            //    qs.InitWhere = "ProgramUid=@PrmUid";
            //    qs.Parameters.Add(new Parameter { ParamKey = "PrmUid", ParamValue = id });
            //});
            //model.JqGridViewModels.Add("PerfObject", jmObject);

            //JqGridViewModel jmOri = this.GetJqGridModel(objTableName, qs =>
            //{
            //    qs.GlobalWhere = "fid not in (select objUid from  perfobject where ProgramUid=@PrmUid and " + _dataAccessor.ValidConditions() + ")";
            //    qs.Parameters.Add(new Parameter { ParamKey = "PrmUid", ParamValue = id });
            //    qs.QueryCols = "Id,Fid," + displayCols;
            //});
            //model.JqGridViewModels.Add(objTableName, jmOri);
            //model.TempData.Add("prmUid", id);
            //model.TempData.Add("prmName", prm);
            //return View(model);
            return View();
        }
        /// <summary>
        /// 考核方式
        /// </summary>
        /// <returns></returns>
        public IActionResult Measure()
        {
            return View();
        }
        /// <summary>
        /// 考核打分
        /// </summary>
        /// <returns></returns>
        public IActionResult Scoring()
        {
            return View();
        }
        public IActionResult Scored()
        {
            return View();
        }
    }
}