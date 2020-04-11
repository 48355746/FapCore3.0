using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Dapper;
using Fap.AspNetCore.Infrastructure;
using Fap.AspNetCore.Model;
using Fap.AspNetCore.ViewModel;
using Fap.Core.DataAccess;
using Fap.Core.Extensions;
using Fap.Core.Infrastructure.Model;
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
            var payToDos = _dbContext.QueryWhere<PayToDo>("Fid in @Fids", new Dapper.DynamicParameters(new { Fids = payToDoFids })).AsList();
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
      
        [HttpPost("PayItems")]
        public JsonResult AddPayItem(string caseUid, string[] payItems)
        {
            Guard.Against.NullOrEmpty(caseUid, nameof(caseUid));
            _payrollService.AddPayItem(caseUid, payItems);
            return Json(ResponseViewModelUtils.Sueecss());
        }
        /// <summary>
        /// 薪资条项
        /// </summary>
        /// <param name="caseUid"></param>
        /// <returns></returns>
        [HttpGet("PayrollItems/{caseUid}")]
        public JsonResult GetPayrollItems(string caseUid)
        {
            Guard.Against.NullOrEmpty(caseUid, nameof(caseUid));
            var payrollItems = _dbContext.QueryWhere<PayItem>("CaseUid=@CaseUid", new DynamicParameters(new { CaseUid = caseUid }), true);

            return Json(ResponseViewModelUtils.Sueecss(payrollItems));
        }
        [HttpPost("PayrollItem")]
        public JsonResult AddPayrollItem(string caseUid, string[] payItems)
        {
            Guard.Against.NullOrEmpty(caseUid, nameof(caseUid));
            _dbContext.Execute("update PayItem set ShowCard=1 where Fid in @Fids", new DynamicParameters(new { Fids = payItems }));
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
        [HttpPost("CreatePayCase")]
        public JsonResult CreatePayCase(string caseUid)
        {
            long tbId = _payrollService.CreatePayCase(caseUid);
            _metadataContext.CreateTable(tbId);
            return Json(ResponseViewModelUtils.Sueecss());
        }
        [HttpPost("InitPaycaseEmployee")]
        public JsonResult InitPaycaseEmployee(string caseUid)
        {
            var payCase= _dbContext.Get<PayCase>(caseUid);
            if (payCase.TableName.IsMissing())
            {
                return Json(ResponseViewModelUtils.Failure("请先生成薪资项"));
            }
            if (payCase.EmpCondition.IsMissing())
            {
                return Json(ResponseViewModelUtils.Failure("请先保存员工条件"));
            }
            if (payCase.Unchanged == 1)
            {
                return Json(ResponseViewModelUtils.Failure("已经有发放记录，不能再初始化员工了"));
            }
            JsonFilterToSql jfs = new JsonFilterToSql(_dbContext);
            string filterWhere = jfs.BuilderFilter("Employee", payCase.EmpCondition);
            _payrollService.InitEmployeeToPayCase(payCase, filterWhere);
            return Json(ResponseViewModelUtils.Sueecss());
        }
        [HttpPost("InitPayrollData")]
        public JsonResult InitPayrollData(PayrollInitDataViewModel model)
        {
            _payrollService.InitPayrollData(model);
            return Json(ResponseViewModelUtils.Sueecss());
        }
        [HttpGet("FormulaCase")]
        public JsonResult FormulaCase(string caseUid)
        {
            var payCase = _dbContext.Get<PayCase>(caseUid);
            if (payCase.PayFlag == 1)
            {
                return Json(ResponseViewModelUtils.Failure("薪资已发放不需要再计算"));
            }
            string tableName = payCase.TableName;
            var data= _dbContext.QueryWhere<FapFormulaCase>("TableName=@TableName", new Dapper.DynamicParameters(new { TableName = tableName }));
            return Json(ResponseViewModelUtils.Sueecss(data));
        }
        [HttpPost("PayrollCalculate")]
        public JsonResult PayrollCalculate(string formulaCaseUid)
        {
            var errorList = _payrollService.PayrollCalculate(formulaCaseUid);
            return Json(ResponseViewModelUtils.Sueecss(errorList));
        }
        [HttpPost("PayOff")]
        public JsonResult PayOff(string caseUid)
        {
            _payrollService.PayrollOff(caseUid);
            return Json(ResponseViewModelUtils.Sueecss());
        }
        [HttpPost("PayOffCancel")]
        public JsonResult PayOffCancel(string caseUid)
        {
            _payrollService.PayrollOffCancel(caseUid);
            return Json(ResponseViewModelUtils.Sueecss());
        }
        [HttpGet("PayGapAnalysis/{recordUid}")]
        public JsonResult PayGapAnalysis(string recordUid)
        {
            var emps= _payrollService.PayGapAnalysis(recordUid);
            return Json(ResponseViewModelUtils.Sueecss(emps));
        }
        [HttpPost("PayOffNotice")]
        public JsonResult PayOffNotice(string caseUid)
        {
            _payrollService.PayrollOffNotice(caseUid);
            return Json(ResponseViewModelUtils.Sueecss());
        }
    }
}