using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Fap.AspNetCore.Infrastructure;
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
    }
}