using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Fap.AspNetCore.Infrastructure;
using Fap.AspNetCore.ViewModel;
using Fap.Core.DataAccess;
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
        private readonly IDbMetadataContext _metadataContext;
        public PayrollApiController(IServiceProvider serviceProvider, IDbMetadataContext metadataContext, IPayrollService payrollService) : base(serviceProvider)
        {
            _payrollService = payrollService;
            _metadataContext = metadataContext;
        }
        [HttpPost("UsePayTodo")]
        public JsonResult UsePayTodo(IEnumerable<string> payToDoFids)
        {
            Guard.Against.Null(payToDoFids, nameof(payToDoFids));
            var payToDos = _dbContext.QueryWhere<PayToDo>("Fid in @Fids", new Dapper.DynamicParameters(new { Fids = payToDoFids }));
            foreach (var payTodo in payToDos)
            {
                _payrollService.UsePayPending(payTodo);
            }
            return Json(ResponseViewModelUtils.Sueecss());
        }

        [HttpPost("ShelvePayTodo")]
        public JsonResult ShelvePayTodo(IEnumerable<string> payToDoFids)
        {
            Guard.Against.Null(payToDoFids, nameof(payToDoFids));
            var payToDos = _dbContext.QueryWhere<PayToDo>("Fid in @Fids", new Dapper.DynamicParameters(new { Fids = payToDoFids }));
            foreach (var payTodo in payToDos)
            {
                payTodo.OperFlag="2";
            }
            _dbContext.UpdateBatch(payToDos);
            return Json(ResponseViewModelUtils.Sueecss());
        }
        [HttpGet("PaySet/{caseUid}")]
        public JsonResult GetPaySet(string caseUid)
        {
            Guard.Against.NullOrEmpty(caseUid, nameof(caseUid));
            var pc = _dbContext.Get<PayCase>(caseUid);
            return Json(ResponseViewModelUtils.Sueecss(pc));
        }
        /// <summary>
        /// 选择工资项
        /// </summary>
        /// <param name="caseCode"></param>
        /// <returns></returns>
        [HttpGet("PayItems/{caseUid}")]
        public JsonResult GetPayItems(string caseUid)
        {
            Guard.Against.NullOrEmpty(caseUid, nameof(caseUid));
            var payCaseItems = _payrollService.GetPayCaseItem(caseUid);

            return Json(ResponseViewModelUtils.Sueecss(payCaseItems));
        }
        [HttpPost("PayItem")]
        public JsonResult AddPayItem(string caseUid, string[] payItems)
        {
            Guard.Against.NullOrEmpty(caseUid, nameof(caseUid));
            _payrollService.AddPayItem(caseUid, payItems);
            return Json(ResponseViewModelUtils.Sueecss());
        }
        [HttpPost("PayCondition")]
        public JsonResult SavePayCaseEmployeeContion(string caseUid, string filters)
        {
            Guard.Against.NullOrEmpty(caseUid, nameof(caseUid));
            var payCase = _dbContext.Get<PayCase>(caseUid);
            payCase.EmpCondition = filters;
            _dbContext.Update(payCase);
            return Json(ResponseViewModelUtils.Sueecss());
        }
        [HttpPost("GeneratePayCase")]
        public JsonResult GeneratePayCase(string caseUid)
        {
            long tbId = _payrollService.GenericPayCase(caseUid);
            _metadataContext.CreateTable(tbId);
            return Json(ResponseViewModelUtils.Sueecss());
        }
    }
}