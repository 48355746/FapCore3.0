using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Fap.AspNetCore.Infrastructure;
using Fap.AspNetCore.ViewModel;
using Fap.Hcm.Service.Payroll;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fap.Hcm.Web.Areas.Payroll.Controllers
{
    [Area("Payroll")]
    [Produces(MediaTypeNames.Application.Json)]
    [Route("[area]/Api")]
    public class PayrollApiController : FapController
    {
        private readonly IPayrollService _payrollService;
        public PayrollApiController(IServiceProvider serviceProvider, IPayrollService payrollService) : base(serviceProvider)
        {
            _payrollService = payrollService;
        }
        /// <summary>
        /// 选择工资项
        /// </summary>
        /// <param name="caseCode"></param>
        /// <returns></returns>
        [HttpGet("PayItems/{caseUid}")]
        public JsonResult GetPayItems(string caseUid)
        {
            var payCaseItems= _payrollService.GetPayCaseItem(caseUid);
            
            return Json(ResponseViewModelUtils.Sueecss(payCaseItems));
        }
        [HttpPost("PayItem")]
        public JsonResult AddPayItem(string caseUid,string[] payItems)
        {
            Guard.Against.NullOrEmpty(caseUid, nameof(caseUid));
            _payrollService.AddPayItem(caseUid, payItems);
            return Json(ResponseViewModelUtils.Sueecss());
        }
    }
}