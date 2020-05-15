using System;
using Fap.Core.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fap.AspNetCore.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Fap.Hcm.Service.Organization;
using Fap.Core.Infrastructure.Metadata;
using Fap.Hcm.Web.Models;
using NPOI.SS.Formula.Functions;

namespace Fap.Hcm.Web.Areas.SelfService.Controllers
{
    [Area("SelfService")]
    public class MssController : FapController
    {
        private readonly IOrganizationService _organizationService;
        public MssController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _organizationService = serviceProvider.GetService<IOrganizationService>();
        }
        /// <summary>
        /// 部门员工信息
        /// </summary>
        /// <returns></returns>
        public IActionResult DeptEmployee()
        {
            ViewBag.EmpStatus = _dbContext.Dictionarys("EmpStatus");
            IEnumerable<FapTable> empChilds = _dbContext.Tables(t => t.TableCategory == "EmpSub");//||t.TableCategory=="EmpBiz");
            var gvms = empChilds.Select(t => new GridViewModel { TableLabel = t.TableComment, TableName = t.TableName, Condition = "EmpUid='" + _applicationContext.EmpUid + "'" });
            ViewBag.SubInfo = gvms.ToJson();
            var deptUids = _organizationService.GetDominationDepartment().Select(d => $"'{d.Fid}'");
            var model = GetJqGridModel("Employee", qs =>
            {
                qs.GlobalWhere = deptUids.Any() ? $"DeptUid in({string.Join(',', deptUids)})" : "1=2";
            });
            return View(model);
        }
        public IActionResult DeptEmployeeSelector()
        {
            var deptUids = _organizationService.GetDominationDepartment().Select(d => $"'{d.Fid}'");
            var model = GetJqGridModel("Employee",qs=>
            {
                qs.GlobalWhere = deptUids.Any() ? $"DeptUid in({string.Join(',', deptUids)})" : "1=2";
            });
            return PartialView(model);
        }
        /// <summary>
        /// 部门周报日报月报
        /// </summary>
        /// <returns></returns>
        public IActionResult EmpReport(string empUid)
        {
            var model = this.GetJqGridModel("EssReport", (q) =>
            {
                q.GlobalWhere = $"EmpUid=@EmpUid";
                q.AddParameter("EmpUid", empUid);
            });
            return PartialView(model);
        }
        /// <summary>
        /// 部门日历
        /// </summary>
        /// <returns></returns>
        public IActionResult DeptCalendar()
        {
            return View();
        }


    }
}