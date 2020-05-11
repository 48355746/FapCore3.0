using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Dapper;
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
        public JsonResult PublishWebsite(string demandUid, string[] websites)
        {
            Guard.Against.NullOrEmpty(demandUid, nameof(demandUid));
            Guard.Against.Null(websites, nameof(websites));
            _recruitService.PublishWebsite(demandUid, string.Join(',', websites));
            return Json(ResponseViewModelUtils.Sueecss());
        }
        [HttpPost("ExecStatus")]
        public JsonResult SetDemandExecStatus(string demandUid, string status)
        {
            Guard.Against.NullOrEmpty(demandUid, nameof(demandUid));
            Guard.Against.Null(status, nameof(status));
            _recruitService.DemandExecStatus(demandUid, status);
            return Json(ResponseViewModelUtils.Sueecss());
        }
        [HttpPost("ResumeStatus")]
        public JsonResult SetResumeStatus(List<string> fids, string status)
        {
            Guard.Against.Null(fids, nameof(fids));
            Guard.Against.NullOrEmpty(status, nameof(status));
            _recruitService.ResumeStatus(fids, status);
            return Json(ResponseViewModelUtils.Sueecss());
        }
        [HttpPost("ResumeToDept")]
        public JsonResult SendResumeToDept(List<string> resumeUids,string demandUid)
        {
            _recruitService.SendResumeToDept(resumeUids, demandUid);
            return Json(ResponseViewModelUtils.Sueecss());
        }
        [HttpGet("ReceiveResume")]
        public JsonResult ReceiveResume()
        {
            var mails = _dbContext.QueryWhere<RcrtMail>("Enabled=1");
            if (!mails.Any())
            {
                return Json(ResponseViewModelUtils.Failure("请配置招聘邮箱[基础设置]"));
            }
            _recruitService.ReceiveResume(mails);
            return Json(ResponseViewModelUtils.Sueecss());
        }
        [HttpPost("InterviewNotice")]
        public JsonResult InterviewNotice(InterviewNoticeViewModel interviewNotice)
        {
            Guard.Against.Null(interviewNotice,nameof(interviewNotice));
            _recruitService.InterviewNotice(interviewNotice);
            return Json(ResponseViewModelUtils.Sueecss());
        }
        [HttpPost("OfferNotice")]
        public JsonResult OfferNotice(OfferNoticeViewModel offerNotice)
        {
            Guard.Against.Null(offerNotice, nameof(offerNotice));
            _recruitService.OfferNotice(offerNotice);
            return Json(ResponseViewModelUtils.Sueecss());
        }
        [HttpPost("OfferStatus")]
        public JsonResult OfferStatus(string fid,string status)
        {
            _dbContext.Execute("Update RcrtBizOffer set OfferStatus =@Status where Fid = @Fid", new DynamicParameters(new { Fid = fid,Status=status }));
            return Json(ResponseViewModelUtils.Sueecss());
        }
        [HttpPost("Entry")]
        public JsonResult Entry(string offerUid,string entryUid)
        {
            _recruitService.Entry(offerUid, entryUid);
            return Json(ResponseViewModelUtils.Sueecss());
        }
        [HttpPost("LinkJob")]
        public JsonResult LinkJob(List<string> fids,string jobName)
        {
            Guard.Against.Null(fids, nameof(fids));
            if (fids.Any())
            {
                _dbContext.Execute("update RcrtResume set ResumeName=@JobName where Fid in @Fids", new DynamicParameters(new { Fids = fids, JobName = jobName }));
            }
            return Json(ResponseViewModelUtils.Sueecss());
        }
    }
}