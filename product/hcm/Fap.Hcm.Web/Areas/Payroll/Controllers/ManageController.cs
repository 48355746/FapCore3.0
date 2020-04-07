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
            JqGridViewModel model = this.GetJqGridModel("PayCase",qs=> {
                qs.GlobalWhere = "CreateBy=@EmpUid";
                qs.AddParameter("EmpUid", _applicationContext.EmpUid);
            });         
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
        public PartialViewResult PayEmployee()
        {
            JqGridViewModel model = this.GetJqGridModel("Employee");
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
        public IActionResult PayCalculate()
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("EmpUid", _applicationContext.EmpUid);
            IEnumerable<PayCase> payCases = _dbContext.QueryWhere<PayCase>("TableName!='' and CreateBy=@EmpUid or  Fid in(select CaseUid from PayCaseEmployee where EmpUid=@EmpUid)",param);
            return View(payCases);
        }
        public PartialViewResult PayInfo(string fid)
        {
            var pc= _dbContext.Get<PayCase>(fid);
            var model = GetJqGridModel(pc.TableName,qs=> {
                qs.AddDefaultValue("PayCaseUid", pc.Fid);
                qs.AddDefaultValue("PayCaseUidMC", pc.CaseName);
            });
            return PartialView(model);
        }
    }
}