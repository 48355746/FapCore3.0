using System;
using System.Collections.Generic;
using System.Linq;
using Fap.AspNetCore.Infrastructure;
using Fap.AspNetCore.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Fap.Core.Extensions;
using Ardalis.GuardClauses;
using Fap.Hcm.Service.Recruit;
using Fap.Core.Infrastructure.Metadata;
using Fap.Core.Infrastructure.Config;
using System.Text.RegularExpressions;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.Infrastructure.Enums;
using Dapper;
using Fap.AspNetCore.Controls.DataForm;

namespace Fap.Hcm.Web.Areas.Recruit.Controllers
{
    [Area("Recruit")]
    public class ManageController : FapController
    {
        public ManageController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
        /// <summary>
        /// 招聘需求
        /// </summary>
        /// <returns></returns>
        public IActionResult Demand()
        {
            var cols = _dbContext.Columns("RcrtDemand").Where(c => !c.ColProperty.EqualsWithIgnoreCase("3"))?.Select(c => c.ColName);
            Guard.Against.Null(cols, nameof(cols));
            JqGridViewModel model = this.GetJqGridModel("RcrtDemand", (qs) =>
            {
                qs.QueryCols = string.Join(',', cols);
                qs.GlobalWhere = "BillStatus='PASSED'";
            });
            return View(model);
        }
        /// <summary>
        /// 入职管理
        /// </summary>
        /// <returns></returns>
        public IActionResult Entry()
        {
            var cols = _dbContext.Columns("RcrtBizOffer").Where(c => !c.ColProperty.EqualsWithIgnoreCase("3"))?.Select(c => c.ColName);
            var model = GetJqGridModel("RcrtBizOffer", qs =>
            {
                qs.QueryCols = string.Join(',', cols);
                qs.GlobalWhere = $"BillStatus='{BillStatus.PASSED}'";
            });
            return View(model);
        }
        public IActionResult ItemPreparation(string offerUid, string offerName)
        {
            var model = GetJqGridModel("RcrtEntryPreparation", qs =>
            {
                qs.GlobalWhere = "OfferUid=@OfferUid";
                qs.AddParameter("OfferUid", offerUid);
                qs.AddDefaultValue("OfferUid", offerUid);
                qs.AddDefaultValue("OfferUidMC", offerName);
            });
            return PartialView(model);
        }
        /// <summary>
        /// 简历库
        /// </summary>
        /// <returns></returns>
        public IActionResult Resume()
        {
            var cols = _dbContext.Columns("RcrtResume").Where(c => c.CtrlType != FapColumn.CTRL_TYPE_RICHTEXTBOX).Select(c => c.ColName);
            var model = GetJqGridModel("RcrtResume", qs =>
            {
                qs.QueryCols = string.Join(',', cols);
            });
            return View(model);
        }
        /// <summary>
        /// 经理自助--招聘职位
        /// </summary>
        /// <returns></returns>
        public IActionResult DeptDemand()
        {
            var cols = _dbContext.Columns("RcrtDemand").Where(c => !c.ColProperty.EqualsWithIgnoreCase("3"))?.Select(c => c.ColName);
            Guard.Against.Null(cols, nameof(cols));
            JqGridViewModel model = this.GetJqGridModel("RcrtDemand", (qs) =>
            {
                qs.QueryCols = string.Join(',', cols);
                qs.GlobalWhere = "BillStatus='PASSED' and Leader=@EmpUid";
                qs.AddParameter("EmpUid", _applicationContext.EmpUid);
            });
            return View(model);
        }
        public IActionResult WebsiteSelector()
        {
            var websites = _dbContext.Query<RcrtWebsite>($"select {nameof(RcrtWebsite.WebName)},{nameof(RcrtWebsite.WebUrl)} from {nameof(RcrtWebsite)}");
            return PartialView(websites);
        }
        /// <summary>
        /// 职位简历
        /// </summary>
        /// <returns></returns>
        public IActionResult JobResume(string title)
        {
            var cols = _dbContext.Columns("RcrtResume").Where(c => c.CtrlType != FapColumn.CTRL_TYPE_RICHTEXTBOX).Select(c => c.ColName);
            var model = GetJqGridModel("RcrtResume", qs =>
            {
                qs.QueryCols = string.Join(',', cols);
                qs.GlobalWhere = $"{nameof(RcrtResume.ResumeName)}=@Title and {nameof(RcrtResume.ResumeStatus)} not in ('{RcrtResumeStatus.BlackList}','{RcrtResumeStatus.TalentPool}','{RcrtResumeStatus.Reserve}')";
                qs.AddParameter("Title", title);
            });
            return PartialView(model);
        }
        /// <summary>
        /// 部门职位简历
        /// </summary>
        /// <returns></returns>
        public IActionResult DeptResume(string title)
        {
            var cols = _dbContext.Columns("RcrtResume").Where(c => c.CtrlType != FapColumn.CTRL_TYPE_RICHTEXTBOX).Select(c => c.ColName);
            var model = GetJqGridModel("RcrtResume", qs =>
            {
                qs.QueryCols = string.Join(',', cols);
                qs.GlobalWhere = $"{nameof(RcrtResume.ResumeName)}=@Title and {nameof(RcrtResume.ResumeStatus)} not in ('{RcrtResumeStatus.Created}','{RcrtResumeStatus.BlackList}','{RcrtResumeStatus.TalentPool}','{RcrtResumeStatus.Reserve}')";
                qs.AddParameter("Title", title);
            });
            return PartialView("JobResume", model);
        }
        /// <summary>
        /// 简历评估
        /// </summary>
        /// <param name="fid"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public IActionResult ResumeAssess(string fid, string name)
        {
            var model = GetJqGridModel("RcrtResumeReview", qs =>
            {
                qs.GlobalWhere = "ResumeUid=@Fid";
                qs.AddParameter("Fid", fid);
                qs.AddDefaultValue("ResumeUid", fid);
                qs.AddDefaultValue("ResumeUidMC", name);
            });
            return PartialView(model);
        }
        /// <summary>
        /// 面试评价
        /// </summary>
        /// <param name="resumeUid"></param>
        /// <returns></returns>
        public IActionResult InterviewAssess(string resumeUid)
        {
            var model = GetJqGridModel("RcrtInterview", qs =>
            {
                qs.QueryCols = "Id,Fid,EmpUid,Rounds,Locations,IvTime,IvResult,Ability,IvReview";
                qs.GlobalWhere = "ResumeUid=@ResumeUid";
                qs.AddParameter("ResumeUid", resumeUid);
            });
            return PartialView(model);
        }
        /// <summary>
        /// 面试通知
        /// </summary>
        /// <returns></returns>
        public IActionResult InterviewNotice(string resumeUid)
        {
            Guard.Against.NullOrEmpty(resumeUid, nameof(resumeUid));
            var resume = _dbContext.Get("RcrtResume", resumeUid) as IDictionary<string, object>;
            var cols = _dbContext.Columns("RcrtResume");
            var mailTemplates = _dbContext.QueryWhere<CfgEmailTemplate>("ModuleUid='RecruitMailTmpl' and TableName='RcrtResume'");
            foreach (var template in mailTemplates)
            {
                Regex regex = new Regex(FapPlatformConstants.VariablePattern, RegexOptions.IgnoreCase);
                var mat = regex.Matches(template.TemplateContent);
                foreach (Match item in mat)
                {
                    var fieldName = item.ToString().Substring(2, item.ToString().Length - 3);
                    var col = cols.FirstOrDefault(c => c.ColComment.EqualsWithIgnoreCase(fieldName));
                    if (col != null && resume.ContainsKey(col.ColName))
                    {
                        template.TemplateContent = template.TemplateContent.Replace(item.ToString(), resume[col.ColName].ToString(), StringComparison.OrdinalIgnoreCase);
                    }
                }
            }
            ViewBag.MailBox = resume["Emails"].ToString();
            return PartialView(mailTemplates);
        }
        /// <summary>
        /// offer通知
        /// </summary>
        /// <param name="offerUid"></param>
        /// <returns></returns>
        public IActionResult OfferNotice(string offerUid)
        {
            Guard.Against.NullOrEmpty(offerUid, nameof(offerUid));
            var offer = _dbContext.Get("RcrtBizOffer", offerUid) as IDictionary<string, object>;
            var cols = _dbContext.Columns("RcrtBizOffer");
            var mailTemplates = _dbContext.QueryWhere<CfgEmailTemplate>("ModuleUid='RecruitMailTmpl' and TableName='RcrtBizOffer'");
            foreach (var template in mailTemplates)
            {
                Regex regex = new Regex(FapPlatformConstants.VariablePattern, RegexOptions.IgnoreCase);
                var mat = regex.Matches(template.TemplateContent);
                foreach (Match item in mat)
                {
                    var fieldName = item.ToString().Substring(2, item.ToString().Length - 3);
                    var col = cols.FirstOrDefault(c => c.ColComment.EqualsWithIgnoreCase(fieldName));
                    if (col != null && offer.ContainsKey(col.ColName))
                    {
                        template.TemplateContent = template.TemplateContent.Replace(item.ToString(), offer[col.ColName].ToString(), StringComparison.OrdinalIgnoreCase);
                    }
                }
            }
            //生成入职信息
            var entryFid = _dbContext.ExecuteScalar<string>("select Fid from EmpEntryInfo where OfferUid=@OfferUid", new Dapper.DynamicParameters(new { OfferUid = offerUid }));
            if (entryFid.IsMissing())
            {
                FapDynamicObject fdo = new FapDynamicObject(_dbContext.Columns("EmpEntryInfo"));
                var dictCodes = _dbContext.GetBillCode("Employee");
                if (dictCodes.Any() && dictCodes.ContainsKey("EmpCode"))
                {
                    fdo.SetValue("EmpCode", dictCodes["EmpCode"]);
                }
                fdo.SetValue("DeptUid", offer["DeptUid"]);
                fdo.SetValue("DeptCode", offer["DeptCode"]);
                fdo.SetValue("EntryDate", offer["ArrivalDate"]);
                fdo.SetValue("OfferUid", offerUid);
                fdo.SetValue("PJobTitle", offer["PJobTitle"]);
                fdo.SetValue("PJobGroupGrade", offer["PJobGroupGrade"]);
                fdo.SetValue("MJobTitle", offer["MJobTitle"]);
                fdo.SetValue("MJobGroupGrade", offer["MJobGroupGrade"]);
                fdo.SetValue("JobGrade", offer["JobGrade"]);
                _dbContext.InsertDynamicData(fdo);
                entryFid = fdo.Get("Fid").ToString();
            }
            ViewBag.Url = _applicationContext.BaseUrl + "/Home/Tourist/" + entryFid;
            ViewBag.MailBox = _dbContext.ExecuteScalar<string>("select Emails from RcrtResume where Fid=@Fid", new Dapper.DynamicParameters(new { Fid = offer["ResumeUid"]?.ToString() }));
            return PartialView(mailTemplates);
        }
        public IActionResult CheckProfile(string offerUid, FormStatus formStatus)
        {
            Guard.Against.NullOrEmpty(offerUid, nameof(offerUid));
            var fid= _dbContext.ExecuteScalar<string>("select Fid from EmpEntryInfo where  OfferUid=@OfferUid", new DynamicParameters(new { OfferUid = offerUid }));
            var model = GetFormViewModel("EmpEntryInfo", "profile", fid);
            model.FormStatus = formStatus;
            return PartialView(model);
        }
        /// <summary>
        /// 信息维护(对外)
        /// </summary>
        /// <param name="fid"></param>
        /// <returns></returns>
        public IActionResult Profile(string fid)
        {
            var offerUid = _dbContext.ExecuteScalar<string>("select OfferUid from  EmpEntryInfo where Fid=@Fid", new DynamicParameters(new { Fid = fid }));

            if (offerUid.IsMissing())
            {
                return Content("链接已失效");
            }
            var offerStatus = _dbContext.ExecuteScalar<string>("select OfferStatus from RcrtBizOffer where Fid= @Fid", new DynamicParameters(new { Fid = offerUid }));
            if (offerStatus.EqualsWithIgnoreCase("Entry") 
                || offerStatus.EqualsWithIgnoreCase("Invalid")
                ||offerStatus.EqualsWithIgnoreCase("Checked"))
            {
                return Content("链接已失效");
            }
            var cols = _dbContext.Columns("EmpEntryInfo").Where(c => !c.ColProperty.Equals("3", StringComparison.OrdinalIgnoreCase)).Select(c => c.ColName);
            var model = GetFormViewModel("EmpEntryInfo", "profile", fid, qs =>
            {
                qs.QueryCols = string.Join(',', cols);
            });
            ViewBag.OfferUid = offerUid;
            return View(model);
        }



        /// <summary>
        /// 我的招聘
        /// </summary>
        /// <returns></returns>
        public IActionResult MyRecruit()
        {
            MultiJqGridViewModel multi = new MultiJqGridViewModel();
            //简历评价
            var resumeAssess = GetJqGridModel("RcrtResumeReview", qs =>
            {
                qs.GlobalWhere = "EmpUid=@EmpUid";
                qs.AddParameter("EmpUid", _applicationContext.EmpUid);
                qs.ReadOnlyCols = "EmpUid";
            });
            //面试
            var interviewAssess = GetJqGridModel("RcrtInterview", qs =>
            {
                qs.GlobalWhere = "EmpUid=@EmpUid";
                qs.AddParameter("EmpUid", _applicationContext.EmpUid);
            });
            var cols = _dbContext.Columns("RcrtDemand").Where(c => !c.ColProperty.EqualsWithIgnoreCase("3"))?.Select(c => c.ColName);
            //内推
            var innerRecommend = GetJqGridModel("RcrtDemand", qs =>
            {
                qs.QueryCols = string.Join(',', cols);
                qs.GlobalWhere = "InternalAble=1";
            });
            multi.JqGridViewModels.Add("resume", resumeAssess);
            multi.JqGridViewModels.Add("interview", interviewAssess);
            multi.JqGridViewModels.Add("recommend", innerRecommend);
            return View(multi);
        }
        public IActionResult MyRecommend(string demandName)
        {
            var model = GetJqGridModel(nameof(RcrtResume), qs =>
            {
                qs.GlobalWhere = "EmpUid=@EmpUid";
                qs.AddParameter("EmpUid", _applicationContext.EmpUid);
                qs.AddDefaultValue("ResumeName", demandName);
                qs.AddDefaultValue("ResumeStatus", RcrtResumeStatus.Created);
                qs.AddDefaultValue("EmpUid", _applicationContext.EmpUid);
                qs.AddDefaultValue("EmpUidMC", _applicationContext.EmpName);

            });
            return PartialView(model);
        }
        //基础设置
        public IActionResult BasicSettings()
        {
            JqGridViewModel modelWebsite = this.GetJqGridModel("RcrtWebsite");
            var modelMail = GetJqGridModel("RcrtMail");
            var modelTemplate = GetJqGridModel("CfgEmailTemplate", qs =>
            {
                qs.QueryCols = "Id,Fid,Name,ModuleUid,TableName,TemplateContent,Enabled";
                qs.GlobalWhere = "ModuleUid='RecruitMailTmpl'";
                qs.AddDefaultValue("ModuleUid", "RecruitMailTmpl");
                qs.AddDefaultValue("TableName", "RcrtResume");
                qs.AddDefaultValue("TableNameMC", "招聘简历");
            });
            MultiJqGridViewModel multiModel = new MultiJqGridViewModel();
            multiModel.JqGridViewModels.Add("website", modelWebsite);
            multiModel.JqGridViewModels.Add("mail", modelMail);
            multiModel.JqGridViewModels.Add("template", modelTemplate);
            return View(multiModel);
        }


    }
}