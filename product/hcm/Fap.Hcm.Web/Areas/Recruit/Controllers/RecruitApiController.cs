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
            _recruitService.PublishWebsite(demandUid, string.Join(',', websites));
            return Json(ResponseViewModelUtils.Sueecss());
        }
        [HttpPost("ExecStatus")]
        public JsonResult SetDemandExecStatus(string demandUid,string status)
        {
            Guard.Against.NullOrEmpty(demandUid, nameof(demandUid));
            Guard.Against.Null(status, nameof(status));
            _recruitService.DemandExecStatus(demandUid, status);
            return Json(ResponseViewModelUtils.Sueecss());
        }
        [HttpPost("ResumeStatus")]
        public JsonResult SetResumeStatus(List<string> fids,string status)
        {
            Guard.Against.Null(fids, nameof(fids));
            Guard.Against.NullOrEmpty(status, nameof(status));
            _recruitService.ResumeStatus(fids, status);
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