using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Mime;
using Ardalis.GuardClauses;
using Dapper;
using Fap.AspNetCore.Infrastructure;
using Fap.AspNetCore.ViewModel;
using Fap.Core.Annex;
using Fap.Core.Extensions;
using Fap.Core.Infrastructure;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.Infrastructure.Model;
using Fap.Core.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Fap.Hcm.Web.Areas.System.Controllers
{
    [Area("System")]
    [Produces(MediaTypeNames.Application.Json)]
    [Route("[area]/Api/Survey")]
    public class SurveyApiController : FapController
    {
        private readonly IFapFileService _fapFileService;
        private readonly ILogger<SurveyApiController> _logger;
        private readonly ISurveyService _surveyService;
        public SurveyApiController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _fapFileService = serviceProvider.GetService<IFapFileService>();
            _logger = _loggerFactory.CreateLogger<SurveyApiController>();
            _surveyService = serviceProvider.GetService<ISurveyService>();
        }
        [HttpPost("UploadFile/{fid}")]
        public JsonResult UploadFile(string fid)
        {
            Guard.Against.NullOrWhiteSpace(fid, nameof(fid));
            try
            {
                var files = Request.Form.Files;
                if (files != null && files.Count > 0)
                {
                    if (fid.EqualsWithIgnoreCase("0"))
                    {
                        fid = UUIDUtils.Fid;
                    }
                    var file = files[0];
                    FapAttachment attachment = new FapAttachment();
                    attachment.Bid = fid;
                    attachment.FileName = file.FileName;
                    attachment.FileType = file.ContentType;
                    using Image myImage = Image.FromStream(file.OpenReadStream());
                    //获得图片宽和高 
                    int widths = myImage.Width;
                    int heights = myImage.Height;
                    using var stream = new MemoryStream();
                    myImage.Save(stream, ImageFormat.Png);
                    stream.Position = 0;
                    string attFid = _fapFileService.UploadFile(stream, attachment);
                    return Json(new
                    {
                        status = 0,
                        img_url = _applicationContext.BaseUrl + "/Component/Photo/" + fid,
                        img_id = attFid,
                        width = widths,
                        height = heights
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return Json(new
            {
                status = 1,
                img_url = ""
            });

        }
        [HttpPost("Design")]
        public JsonResult DesignSurvey(string content)
        {
            Newtonsoft.Json.Linq.JObject jobject = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(content);
            long id = jobject.GetStringValue("survey_id").ToInt();
            Survey survey = _dbContext.Get<Survey>(id);
            survey.SurName = jobject.GetStringValue("survey_name");
            survey.SurContent = jobject.GetStringValue("test_content");
            survey.JSONContent = jobject.ToString();
            //同时生成预览视图
            _surveyService.PreviewSurvey(survey);
            _dbContext.Update(survey);
            return Json(ResponseViewModelUtils.Sueecss());
        }
        [HttpPost("Publish")]
        public JsonResult PublishSurvey(SurFilter surFilter)
        {
            Guard.Against.Null(surFilter, nameof(surFilter));
            surFilter.SurStartDate = surFilter.PublishTime = DateTimeUtils.CurrentDateTimeStr;
            _surveyService.PublishSurvey(surFilter);
            return Json(ResponseViewModelUtils.Sueecss());
        }
        [AllowAnonymous]
        [HttpPost("FillIn")]
        public JsonResult FillInSurvey(string content)
        {
            JObject jobject = (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(content);
            bool result = _surveyService.SaveSelfCollectionSurvey(jobject);
            return Json(new ResponseViewModel { success = result });
        }
        [HttpGet("Close/{fid}")]
        public JsonResult GetCloseSurvey(string fid)
        {
            Guard.Against.NullOrEmpty(fid, nameof(fid));
            var survey = _dbContext.Get<Survey>(fid);
            if (survey != null)
            {
                survey.SurStatus = SurveyStatus.Completed;
                _dbContext.Update(survey);
            }
            return Json(ResponseViewModelUtils.Sueecss());
        }
        [HttpGet("Report/{fid}/{order}")]
        public JsonResult GetSurveyReportData(string fid, int order)
        {
            JObject jobj = _surveyService.GetSurveyReportResult(fid, order);

            return Json(jobj);
        }
        [HttpGet("ExportReport/{fid}")]
        public JsonResult ExportReport(string fid)
        {
            string fileName = $"{UUIDUtils.Fid}.xls";
            string filePath = Path.Combine(Environment.CurrentDirectory, FapPlatformConstants.TemporaryFolder, fileName);
            try
            {
                _surveyService.ExportSurveyStat(filePath, fid);
                return Json(new { error_code = 0, exportStatus = 0, fn = $"{FapPlatformConstants.TemporaryFolder}/{fileName}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return Json(new { error_code = 1, exportStatus = 0 });
            }
        }
        [HttpGet]
        [Route("ExportUserReport")]
        public JsonResult ExportReportUserData(string survey_id, string sheet)
        {
            string fileName = $"{UUIDUtils.Fid}.xls";
            string filePath = Path.Combine(Environment.CurrentDirectory, FapPlatformConstants.TemporaryFolder, fileName);
            try
            {
                _surveyService.ExportSurveyUserDataStat(filePath, survey_id, sheet);
                return Json(new { error_code = 0, exportStatus = 0, fn = $"{FapPlatformConstants.TemporaryFolder}/{fileName}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return Json(new { error_code = 1, exportStatus = 0 });
            }
        }
        [HttpGet("ReportFilter/{fid}")]
        public JsonResult GetSurveyReportFilter(string fid)
        {
            var jarry = _surveyService.GetReportFilter(fid);
            return Json(jarry);
        }
        [HttpPost("ReportFilter")]
        public JsonResult PostSurveyReportFilter(string survey_id, string filters)
        {
            var list = _surveyService.SaveReportFilters(survey_id, filters);
            JObject jfilter = new JObject();
            jfilter["error"] = "1";
            jfilter["msg"] = "success";
            JArray jfs = new JArray();
            jfilter["filters"] = jfs;

            foreach (var rf in list)
            {
                jfs.Add(_surveyService.SurReportFilterToJSON(rf));
            }
            return Json(jfilter);
        }

        [HttpPost("DeleteReportFilter")]
        public JsonResult DeleteSurveyReportFilter(string id, string survey_id)
        {
            string fid = id;
            string surveyUid = survey_id;
            _dbContext.DeleteExec(nameof(SurReportFilter), "Fid=@Fid", new Dapper.DynamicParameters(new { Fid = fid }));
            DynamicParameters param = new DynamicParameters();
            param.Add("SurveyUid", surveyUid);
            var list = _dbContext.QueryWhere<SurReportFilter>("SurveyUid=@SurveyUid", param);
            JArray jfs = new JArray();
            foreach (var rf in list)
            {
                jfs.Add(_surveyService.SurReportFilterToJSON(rf));
            }

            return Json(jfs);
        }
        //用户答卷列表
        [HttpGet("Tester/{survey_fid}")]
        public JsonResult GetResponseList(string survey_fid)
        {
            var obj= _surveyService.GetResponseList(survey_fid);
            return Json(obj);
        }
        //用户答卷详情[上一条，下一条]
        [HttpGet("TesterPrev")]
        public JsonResult GetSurveyResponsePrev(string res_id,string survey_id,string status,int res_order)
        {
            int resOrder = res_order - 1;
            var jobj= _surveyService.GetSurveyResponse(res_id, survey_id, status, resOrder);
            return Json(jobj);
        }
        [HttpGet("TesterNext")]
        public JsonResult GetSurveyResponseNext(string res_id, string survey_id, string status, int res_order)
        {
            int resOrder = res_order + 1;
            var jobj = _surveyService.GetSurveyResponse(res_id, survey_id, status, resOrder);
            return Json(jobj);
        }
    }
}