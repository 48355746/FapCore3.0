using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fap.AspNetCore.Infrastructure;
using Fap.AspNetCore.Model;
using Fap.Core.Extensions;
using Fap.Core.Infrastructure;
using Fap.Core.Infrastructure.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.DependencyInjection;
using Fap.Core.Utility;
using Ardalis.GuardClauses;

namespace Fap.Hcm.Web.Areas.System.Controllers
{
    [Area("System")]
    public class SurveyController : FapController
    {
        private readonly ISurveyService _surveyService;
        public SurveyController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _surveyService = serviceProvider.GetService<ISurveyService>();
        }

        public IActionResult Index()
        {
            var model = GetJqGridModel("Survey", qs =>
            {
                qs.QueryCols = "Id,Fid,SurName,IsShare,FilterModel,CreateTime,Completed,SurStatus";
                qs.AddDefaultValue("FilterModel", "Self");
                qs.AddDefaultValue("CreateTime", Fap.Core.Utility.DateTimeUtils.CurrentDateTimeStr);
                qs.AddDefaultValue("SurStatus", "0");
            });
            return View(model);
        }
        public IActionResult Create(string fid)
        {
            var survey = _dbContext.Get<Survey>(fid);
            return View(survey);
        }
        /// <summary>
        /// 设计问卷
        /// </summary>
        /// <param name="fid"></param>
        /// <returns></returns>
        public IActionResult Design(string fid)
        {
            var survey = _dbContext.Get<Survey>(fid);
            if (survey.JSONContent.IsMissing())
            {
                survey.JSONContent = "{\"survey_id\": " + survey.Id + ",\"survey_name\":\"" + survey.SurName + "\", \"test_content\": \"" + survey.SurContent + "\", \"status\": 0, \"content\": [ ], \"redirect_relation\": null, \"logic_condition\": null   }";
            }
            return View(survey);
        }
        public IActionResult Preview(string fid)
        {
            Survey survey = _dbContext.Get<Survey>(fid);
            survey.JSONPreview += ",0,";
            JObject jobj = new JObject();
            jobj["id"] = survey.Id;
            jobj["pro_name"] = survey.SurName;
            jobj["status"] = "0";
            jobj["test_content"] = survey.SurContent;
            jobj["publish_time"] = survey.PublishTime;
            jobj["tester_count"] = "0";
            survey.JSONPreview += jobj.ToString();
            //, 0, {\"id\":\"110095\",\"pro_name\":\"111\",\"status\":\"0\",\"test_content\":\"欢迎参加调查！答卷数据仅用于统计分析，请放心填写。题目选项无对错之分，按照实际情况选择即可。感谢您的帮助！\",\"publish_time\":null,\"tester_count\":\"0\"
            return View(survey);
        }
        /// <summary>
        /// 收集问卷
        /// </summary>
        /// <param name="fid"></param>
        /// <returns></returns>
        public PartialViewResult Filter(string surveyUid)
        {
            var model = GetJqGridModel("Employee", qs =>
            {
                qs.QueryCols = "Id,Fid,EmpName,EmpCode,DeptUid,DeptCode,Gender,Age,EmpCategory,Education,Degree,Nation,Political,EmpStatus";
                qs.GlobalWhere = "IsMainJob=1";
            });
            var surFilter = _dbContext.QueryFirstOrDefaultWhere<SurFilter>($"{nameof(SurFilter.SurveyUid)}=@SurveyUid", new Dapper.DynamicParameters(new { surveyUid }));
            ViewBag.Filter = surFilter != null ? surFilter.FilterCondition : string.Empty;
            return PartialView(model);
        }
        public IActionResult FillIn(string fid)
        {
            Guard.Against.NullOrEmpty(fid, nameof(fid));
            string surveyUid = fid;
            var survey = _dbContext.Get<Survey>(surveyUid);
            //超期
            if (survey==null||(survey.SurEndDate.IsPresent() && DateTimeUtils.ToDateTime(survey.SurEndDate) < DateTime.Now))
            {
                return NotFound();
            }
            //已完成
            if (survey.SurStatus == SurveyStatus.Completed)
            {
                return NotFound();
            }
            survey.JSONPublish += ",0,";
            JObject jobj = new JObject();
            jobj["id"] = survey.Fid;
            jobj["pro_name"] = survey.SurName;
            jobj["status"] = "4";
            jobj["score"] = "0";
            jobj["bonus_score"] = "0";
            jobj["start_date"] = survey.SurStartDate;
            jobj["end_date"] = survey.SurEndDate;
            jobj["vote_type"] = "0";
            jobj["viewResult"] = "0";
            jobj["isClosed"] = "0";
            jobj["isUnlocked"] = true;
            jobj["isOnline"] = true;
            jobj["type"] = "0";
            jobj["telephone_filter"] = "0";
            jobj["tester_count"] = "0";
            jobj["tester_status"] = 1;
            jobj["token"] = survey.Fid;
            jobj["referer"] = _applicationContext.EmpUid;
            jobj["platform"] = "1";
            jobj["test_content"] = survey.SurContent;
            jobj["publish_time"] = survey.PublishTime;
            survey.JSONPublish += jobj.ToString();
            //判断是否答过此问卷
            SurResponseList ul = _dbContext.QueryFirstOrDefaultWhere<SurResponseList>("SurveyUid=@SurveyUid and EmpUid=@EmpUid", new Dapper.DynamicParameters(new { SurveyUid = surveyUid, EmpUid = _applicationContext.EmpUid }));
            int exists = 0;
            if (ul != null)
            {
                exists = 1;
            }
            ViewBag.Exists = exists;
            return View(survey);

        }
        [AllowAnonymous]
        public IActionResult Finish()
        {
            return View();
        }
        public IActionResult Details(string fid)
        {
            var survey = _dbContext.Get<Survey>(fid, true);
            var filter = _dbContext.QueryFirstOrDefaultWhere<SurFilter>("SurveyUid=@SurveyUid", new Dapper.DynamicParameters(new { SurveyUid = fid }));
            string condition = filter.FilterCondition;
            if (condition.IsMissing())
            {
                survey.FilterModelMC = "全部";
            }
            JsonFilterToSql d = new JsonFilterToSql(_dbContext);
            IEnumerable<FilterDescModel> filterDesc = d.BuilderFilterDesc("Employee", condition);
            ViewBag.SurFilter = filter;
            ViewBag.Filters = filterDesc;
            return View(survey);
        }
        public IActionResult Report(string fid)
        {
            Survey survey = _dbContext.Get<Survey>(fid);
            ViewBag.Project = _surveyService.GetProject(survey);
            ViewBag.SurveyQuestion = _surveyService.GetSurveyQuestion(survey);
            ViewBag.ReportFilter = _surveyService.GetReportFilter(fid);
            return View(survey);
        }
        public IActionResult Result(string fid)
        {
            Survey survey = _dbContext.Get<Survey>(fid, true);
            JObject jHead = new JObject();
            jHead["survey_id"] = survey.Fid;
            jHead["status"] = survey.SurStatus;
            jHead["surveyName"] = survey.SurName;
            jHead["target"] = survey.Amounted;
            var ts = DateTime.Now.Subtract(DateTimeUtils.ToDateTime(survey.PublishTime)).Duration();
            jHead["onlineTime"] = ts.Days.ToString() + "天" + ts.Hours.ToString() + "小时" + ts.Minutes.ToString() + "分钟" + ts.Seconds.ToString() + "秒";
            jHead["type"] = 0;
            ViewBag.Header = jHead;
            return View(survey);
        }
        public IActionResult TesterReview(string resuid, int resorder, string suruid)
        {
            ViewBag.Response = _surveyService.GetSurveyResponse(resuid, suruid, "2", resorder);
            return View();
        }
    }
}