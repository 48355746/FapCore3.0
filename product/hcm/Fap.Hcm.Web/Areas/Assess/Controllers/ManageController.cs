using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fap.AspNetCore.Infrastructure;
using Fap.AspNetCore.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Fap.Core.Extensions;
using Fap.AspNetCore.Controls.JqGrid;
using Fap.Hcm.Service.Assess;
using Dapper;

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
        /// <param name="schemeName">方案名称</param>
        /// <returns></returns>
        public IActionResult KPI(string fid, string schemeName)
        {
            JqGridViewModel model = this.GetJqGridModel("PerfKPIs", qs =>
            {
                qs.GlobalWhere = "ProgramUid=@PrmUid";
                qs.AddParameter("PrmUid", fid);
                qs.AddDefaultValue("ProgramUid", fid);
                qs.AddDefaultValue("ProgramUidMC", schemeName);
            });
            return PartialView(model);
        }
        public IActionResult KPIType(string fid, string schemeName)
        {
            var model = GetJqGridModel("PerfKPIType", qs =>
            {
                qs.GlobalWhere = "PerfProgram=@Scheme";
                qs.AddParameter("Scheme", fid);
                qs.AddDefaultValue("PerfProgram", fid);
                qs.AddDefaultValue("PerfProgramMC", schemeName);
            });
            return PartialView(model);
        }
        /// <summary>
        /// 考核对象
        /// </summary>
        public ActionResult Objectives(string fid)
        {            
            JqGridViewModel model = this.GetJqGridModel("PerfObject", qs =>
            {
                qs.InitWhere = "ProgramUid=@PrmUid";
                qs.AddParameter("PrmUid",fid);
            });
            return PartialView(model);                     
        }
        /// <summary>
        /// 考核对象选择
        /// </summary>
        /// <param name="objType">对象类型</param>
        /// <returns></returns>
        public ActionResult ObjectivesSelector(string objType)
        {
            string tn = "Employee";
            string cols = "Id,Fid,EmpCode,EmpName,EmpPinYin,EmpCategory,DeptUid";
            if (objType.EqualsWithIgnoreCase("OrgDept"))
            {
                tn = "OrgDept";
                cols = "Id,Fid,DeptCode,DeptName,FullName,DeptType,DeptManager,Director";
            }
            JqGridViewModel model = this.GetJqGridModel(tn, qs =>
            {
                qs.QueryCols = cols;
            });
            return PartialView(model);
        }
        /// <summary>
        /// 考核方式
        /// </summary>
        /// <returns></returns>
        public IActionResult Measure()
        {
            return PartialView();
        }
        /// <summary>
        /// 考核人
        /// </summary>
        /// <returns></returns>
        public IActionResult Examiner(string schemeUid,string objUid)
        {
            JqGridViewModel model = this.GetJqGridModel("PerfExaminer", (q) =>
            {
                q.QueryCols = "Id,Fid,ObjectUid,ProgramUid,AssessModel,EmpUid,Weight";
                q.GlobalWhere = "ProgramUid=@PrmUid and ObjectUid=@ObjUid";
                q.AddParameter("PrmUid", schemeUid);
                q.AddParameter("ObjUid", objUid);
            });
            return PartialView(model);
        }
        public IActionResult ScoreModel()
        {
            var model = GetJqGridModel("PerfScoreModel");
            return PartialView(model);
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
        public IActionResult Consequent()
        {
            ViewBag.SchemeList= _dbContext.Query<PerfProgram>("select * from PerfProgram",null,true);
            JqGridViewModel model = this.GetJqGridModel("PerfObject");           
            return View(model);
        }
        public IActionResult AssessChart(string schemeUid)
        {
            ViewBag.SchemeUid = schemeUid;
            return PartialView();
           
        }
    }
}