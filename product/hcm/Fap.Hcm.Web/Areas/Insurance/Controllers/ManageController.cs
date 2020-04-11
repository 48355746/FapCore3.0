using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Fap.AspNetCore.Infrastructure;
using Fap.AspNetCore.ViewModel;
using Fap.Hcm.Service.Insurance;
using Microsoft.AspNetCore.Mvc;

namespace Fap.Hcm.Web.Areas.Insurance.Controllers
{
    [Area("Insurance")]
    public class ManageController : FapController
    {
        public ManageController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        /// <summary>
        /// 保险待处理变动
        /// </summary>
        /// <returns></returns>
        public IActionResult Pending()
        {
            var jqModel = this.GetJqGridModel(nameof(InsToDo), (qs) =>
            {
                qs.GlobalWhere = "CaseUid in(select fid from InsCase where CreateBy=@Employee or fid in(select CaseUid from InsCaseEmployee where EmpUid=@Employee))";
                qs.AddParameter("Employee", _applicationContext.EmpUid);
                qs.InitWhere = "OperFlag=0";
            });         
            return View(jqModel);
        }
        public IActionResult InsSet()
        {
            JqGridViewModel model = this.GetJqGridModel(nameof(InsCase), qs =>
            {
                qs.GlobalWhere = "CreateBy=@EmpUid";
                qs.AddParameter("EmpUid", _applicationContext.EmpUid);
            });
            return View(model);

        }
        /// <summary>
        /// 保险项
        /// </summary>
        /// <param name="caseUid"></param>
        /// <returns></returns>
        public PartialViewResult InsItem(string caseUid)
        {
            JqGridViewModel insItemModel = this.GetJqGridModel(nameof(InsItem), (q) =>
            {
                q.GlobalWhere = "CaseUid=@CaseUid";
                q.AddParameter("CaseUid", caseUid);
            });
            return PartialView(insItemModel);
        }
        /// <summary>
        /// 保险组员工
        /// </summary>
        /// <param name="caseUid"></param>
        /// <returns></returns>
        public PartialViewResult InsEmployee()
        {
            JqGridViewModel model = this.GetJqGridModel("Employee");
            return PartialView(model);
        }
        /// <summary>
        /// 保险组权限
        /// </summary>
        /// <param name="caseUid"></param>
        /// <returns></returns>
        public PartialViewResult InsAuthority(string caseUid, string caseName)
        {
            JqGridViewModel model = this.GetJqGridModel("InsCaseEmployee", (q) =>
            {
                q.AddDefaultValue("CaseUid", caseUid);
                q.AddDefaultValue("CaseUidMC", caseName);
                q.GlobalWhere = "CaseUid=@CaseUid";
                q.AddParameter("CaseUid", caseUid);
            });
            return PartialView(model);
        }
        public IActionResult InsCalculate()
        {
            return View();
        }
        public IActionResult BaseAndRate()
        {
            JqGridViewModel model = this.GetJqGridModel("InsBaseRate");
            return View(model);
        }
    }
}