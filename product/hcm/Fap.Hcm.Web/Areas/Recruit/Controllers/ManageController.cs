using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fap.AspNetCore.Infrastructure;
using Fap.AspNetCore.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Fap.Core.Extensions;
using Ardalis.GuardClauses;
using Fap.Hcm.Service.Recruit;
using Fap.Core.Infrastructure.Metadata;

namespace Fap.Hcm.Web.Areas.Recruit.Controllers
{
    [Area("Recruit")]
    public class ManageController : FapController
    {
        public ManageController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
        public IActionResult Demand()
        {
            var cols= _dbContext.Columns("RcrtDemand").Where(c => !c.ColProperty.EqualsWithIgnoreCase("3"))?.Select(c=>c.ColName);
            Guard.Against.Null(cols, nameof(cols));
            JqGridViewModel model = this.GetJqGridModel("RcrtDemand", (qs) =>
            {
                qs.QueryCols =string.Join(',',cols);
                qs.GlobalWhere = "BillStatus='PASSED'";
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
            var websites= _dbContext.Query<RcrtWebsite>($"select {nameof(RcrtWebsite.WebName)},{nameof(RcrtWebsite.WebUrl)} from {nameof(RcrtWebsite)}");
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
                qs.QueryCols =string.Join(',', cols);
                qs.GlobalWhere = "ResumeName=@Title";
                qs.AddParameter("Title", title);
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
            multiModel.JqGridViewModels.Add("template",modelTemplate);
            return View(multiModel);
        }


    }
}