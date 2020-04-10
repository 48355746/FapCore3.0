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
using Dapper;

namespace Fap.Hcm.Web.Areas.Insurance.Controllers
{
    [Area("Insurance")]
    [Produces(MediaTypeNames.Application.Json)]
    [Route("[area]/Api")]
    public class InsuranceApiController : FapController
    {
        private readonly IInsuranceService _insuranceService;
        public InsuranceApiController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _insuranceService = _serviceProvider.GetService<IInsuranceService>();
        }
        [HttpPost("UseInsTodo")]
        public JsonResult UsePayTodo(IEnumerable<string> insToDoFids)
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
    }
}