using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Fap.AspNetCore.Infrastructure;
using Fap.AspNetCore.ViewModel;
using Fap.Hcm.Service.Recruit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
namespace Fap.Hcm.Web.Areas.Recruit.Controllers
{
    [Area("Recruit")]
    [Produces(MediaTypeNames.Application.Json)]
    [Route("[area]/Api")]
    public class RecruitApiController : FapController
    {
        private readonly IRecruitService _recruitService;
        public RecruitApiController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _recruitService = serviceProvider.GetService<IRecruitService>();
        }
        [HttpPost("PublishWebsite")]
        public JsonResult PublishWebsite(string demandUid,string[] websites)
        {
            Guard.Against.NullOrEmpty(demandUid, nameof(demandUid));
            Guard.Against.Null(websites, nameof(websites));
            _dbContext.Execute("Update RcrtDemand set PublishedIn=@Websites,ExecStatus='Processing' where Fid=@Fid", new Dapper.DynamicParameters(new {Fid=demandUid, Websites = string.Join(',', websites) }));
            return Json(ResponseViewModelUtils.Sueecss());
        }
        [HttpPost("ExecStatus")]
        public JsonResult SetExecStatus(string demandUid,string status)
        {
            Guard.Against.NullOrEmpty(demandUid, nameof(demandUid));
            Guard.Against.Null(status, nameof(status));
            _dbContext.Execute("Update RcrtDemand set ExecStatus=@Status where Fid=@Fid", new Dapper.DynamicParameters(new { Fid = demandUid, Status = status }));
            return Json(ResponseViewModelUtils.Sueecss());
        }
        [HttpGet("ReceiveResume")]
        public JsonResult ReceiveResume()
        {
            _recruitService.ReceiveResume();
            return Json(ResponseViewModelUtils.Sueecss());
        }
    }
}