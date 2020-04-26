﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fap.AspNetCore.Infrastructure;
using Fap.AspNetCore.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Fap.Core.Extensions;
using Ardalis.GuardClauses;
using Fap.Hcm.Service.Recruit;

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
        public IActionResult WebsiteSelector()
        {
            var websites= _dbContext.Query<RcrtWebsite>($"select {nameof(RcrtWebsite.WebName)},{nameof(RcrtWebsite.WebUrl)} from {nameof(RcrtWebsite)}");
            return PartialView(websites);
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