using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Fap.AspNetCore.Infrastructure;
using Fap.Hcm.Service.Insurance;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Ardalis.GuardClauses;
using Fap.AspNetCore.ViewModel;
using Fap.Core.Extensions;
using Dapper;
using Fap.Core.DataAccess;
using Fap.AspNetCore.Model;

namespace Fap.Hcm.Web.Areas.Insurance.Controllers
{
    [Area("Insurance")]
    [Produces(MediaTypeNames.Application.Json)]
    [Route("[area]/Api")]
    public class InsuranceApiController : FapController
    {
        private readonly IInsuranceService _insuranceService;
        private readonly IDbMetadataContext _metadataContext;
        public InsuranceApiController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _insuranceService = _serviceProvider.GetService<IInsuranceService>();
            _metadataContext = _serviceProvider.GetService<IDbMetadataContext>();
        }
        [HttpPost("UseInsTodo")]
        public JsonResult UseInsTodo(IEnumerable<string> insToDoFids)
        {
            Guard.Against.Null(insToDoFids, nameof(insToDoFids));
            var insToDos = _dbContext.QueryWhere<InsToDo>("Fid in @Fids", new Dapper.DynamicParameters(new { Fids = insToDoFids }));
            foreach (var insTodo in insToDos)
            {
                _insuranceService.UseInsPending(insTodo);
            }
            return Json(ResponseViewModelUtils.Sueecss());
        }
        [HttpPost("ShelveInsTodo")]
        public JsonResult ShelveInsTodo(IEnumerable<string> insToDoFids)
        {
            Guard.Against.Null(insToDoFids, nameof(insToDoFids));
            var insToDos = _dbContext.QueryWhere<InsToDo>("Fid in @Fids", new Dapper.DynamicParameters(new { Fids = insToDoFids })).AsList();
            foreach (var insTodo in insToDos)
            {
                insTodo.OperFlag = "2";
            }
            _dbContext.UpdateBatch(insToDos);
            return Json(ResponseViewModelUtils.Sueecss());
        }
        [HttpGet("InsSet/{caseUid}")]
        public JsonResult GetInsSet(string caseUid)
        {
            Guard.Against.NullOrEmpty(caseUid, nameof(caseUid));
            var pc = _dbContext.Get<InsCase>(caseUid);
            return Json(ResponseViewModelUtils.Sueecss(pc));
        }
        [HttpGet("InsItems/{caseUid}")]
        public JsonResult GetInsItems(string caseUid)
        {
            var data= _insuranceService.GetInsItems(caseUid);
            return Json(ResponseViewModelUtils.Sueecss(data));
        }
        [HttpPost("InsItems")]
        public JsonResult AddInsItem(string caseUid, string[] insItems)
        {
            Guard.Against.NullOrEmpty(caseUid, nameof(caseUid));
            _insuranceService.AddInsItems(caseUid, insItems);
            return Json(ResponseViewModelUtils.Sueecss());
        }
        [HttpPost("CreateInsCase")]
        public JsonResult CreateInsCase(string caseUid)
        {
            long tbId = _insuranceService.CreateInsCase(caseUid);
            _metadataContext.CreateTable(tbId);
            return Json(ResponseViewModelUtils.Sueecss());
        }
        [HttpPost("InsCondition")]
        public JsonResult SaveInsCaseEmployeeContion(string caseUid, string filters)
        {
            Guard.Against.NullOrEmpty(caseUid, nameof(caseUid));
            var insCase = _dbContext.Get<InsCase>(caseUid);
            insCase.EmpCondition = filters;
            _dbContext.Update(insCase);
            return Json(ResponseViewModelUtils.Sueecss());
        }
        [HttpPost("InitInscaseEmployee")]
        public JsonResult InitInscaseEmployee(string caseUid)
        {
            var insCase = _dbContext.Get<InsCase>(caseUid);
            if (insCase.TableName.IsMissing())
            {
                return Json(ResponseViewModelUtils.Failure("请先生成保险项"));
            }
            if (insCase.EmpCondition.IsMissing())
            {
                return Json(ResponseViewModelUtils.Failure("请先保存员工条件"));
            }
            if (insCase.Unchanged == 1)
            {
                return Json(ResponseViewModelUtils.Failure("已经存在保险记录，不能再初始化员工了"));
            }
            JsonFilterToSql jfs = new JsonFilterToSql(_dbContext);
            string filterWhere = jfs.BuilderFilter("Employee", insCase.EmpCondition);
            _insuranceService.InitEmployeeToInsCase(insCase, filterWhere);
            return Json(ResponseViewModelUtils.Sueecss());
        }
    }
}