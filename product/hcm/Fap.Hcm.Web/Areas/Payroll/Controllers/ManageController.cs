using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Fap.AspNetCore.Infrastructure;
using Fap.AspNetCore.ViewModel;
using Fap.Hcm.Service.Payroll;
using Microsoft.AspNetCore.Mvc;

namespace Fap.Hcm.Web.Areas.Payroll.Controllers
{
    [Area("Payroll")]
    public class ManageController : FapController
    {
        public ManageController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
        /// <summary>
        /// 薪资待处理变动
        /// </summary>
        /// <returns></returns>
        public IActionResult Pending()
        {
            var jqModel = GetJqGridModel("PayToDo", (qs) =>
            {
                qs.GlobalWhere = "CaseUid in(select fid from PayCase where CreateBy=@EmpUid or fid in(select CaseUid from PayCaseEmployee where EmpUid=@EmpUid))";
                qs.AddParameter("EmpUid", _applicationContext.EmpUid);
                qs.InitWhere = "OperFlag=0";
            });
            DynamicParameters param = new DynamicParameters();
            param.Add("EmpUid", _applicationContext.EmpUid);
            var payCases = _dbContext.Query<PayCase>("select * from PayCase where CreateBy=@EmpUid or fid in(select CaseUid from PayCaseEmployee where EmpUid=@EmpUid)", param);
         
            ViewBag.PayCase = payCases;
            return View(jqModel);
        }
        /// <summary>
        /// 薪资套
        /// </summary>
        /// <returns></returns>
        public IActionResult PaySet()
        {
            JqGridViewModel model = this.GetJqGridModel("PayCase");         
            return View(model);
        }
        /// <summary>
        /// 薪资项
        /// </summary>
        /// <param name="caseUid"></param>
        /// <returns></returns>
        public PartialViewResult PayItem(string caseUid)
        {
            JqGridViewModel payItemModel = this.GetJqGridModel("PayItem", (q) =>
            {
                q.GlobalWhere = "CaseUid=@CaseUid";
                q.AddParameter("CaseUid", caseUid);
            });
            return PartialView(payItemModel);
        }
        /// <summary>
        /// 薪资员工
        /// </summary>
        /// <param name="caseUid"></param>
        /// <returns></returns>
        public PartialViewResult PayEmployee(string caseUid)
        {
            JqGridViewModel model = this.GetJqGridModel("PayCaseEmployee", (q) =>
            {
                q.GlobalWhere = "CaseUid=@CaseUid";
                q.AddParameter("CaseUid", caseUid);
            });
            return PartialView(model);
        }
        /// <summary>
        /// 薪资套权限
        /// </summary>
        /// <param name="caseUid"></param>
        /// <returns></returns>
        public PartialViewResult PayAuthority(string caseUid,string caseName)
        {
            JqGridViewModel model = this.GetJqGridModel("PayCaseEmployee", (q) =>
            {
                q.AddDefaultValue("CaseUid", caseUid);
                q.AddDefaultValue("CaseUidMC", caseName);
                q.GlobalWhere = "CaseUid=@CaseUid";
                q.AddParameter("CaseUid", caseUid);
            });
            return PartialView(model);
        }
        /// <summary>
        /// 薪资套公式
        /// </summary>
        /// <param name="caseUid"></param>
        /// <returns></returns>
        public PartialViewResult PayFormula(string caseUid)
        {
            return PartialView();
        }
    }
}