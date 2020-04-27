using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using MimeKit;
using Fap.Core.Extensions;
using HtmlAgilityPack;
using Fap.Core.DataAccess;
using Fap.Core.Annex;
using Sys = System;
using Fap.Core.DI;

namespace Fap.Hcm.Service.Recruit
{
    [Service]
    public class Job51ParseEmailService :ParseEmailService
    {
        public Job51ParseEmailService(IDbContext dbContext, ILoggerFactory loggerFactory, IFapFileService fapFileService) : base(dbContext, loggerFactory, fapFileService)
        {
        }      
        public override RcrtResume ParseHtmlContent(MimeMessage message)
        {
            string subject = message.Subject;
            if (subject.Contains("51job.com",Sys.StringComparison.OrdinalIgnoreCase))
            {
                RcrtResume model = new RcrtResume();
                model.MessageId = message.MessageId;
                model.MessageFrom = string.Join(";", message.From.Mailboxes.Select(a => a.Address));

                model.ResumeStatus = RcrtResumeStatus.Created;
                model.MessageDate = message.Date.DateTime.ToString("yyyy-MM-dd:HH:mm:ss");
                model.Source = "ZhongHuaYingCai";
                //获取txt内容，用于解析
                string msgText = string.Empty;
                //获取html内容
                string htmlPart = message.HtmlBody;
                if (htmlPart.IsPresent())
                {
                    model.HtmlContent = htmlPart;
                    HtmlDocument hdoc = new HtmlDocument();
                    hdoc.LoadHtml(htmlPart);
                    model.ResumeName = hdoc.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[1]/div[2]/table[1]/tbody[1]/tr[1]/td[2]/table[1]/tbody[1]/tr[1]/td[1]/table[1]/tbody[1]/tr[1]/td[2]").InnerText.RemoveSpace();
                    //hdoc.DocumentNode.SelectSingleNode("//table[1]//td[2]/table[1]//td[2]").InnerText;
                    model.Emails = hdoc.DocumentNode.SelectSingleNode("//table[2]//table[1]//table[2]/tbody[1]//tr[1]/td[2]//td[2]/a").InnerHtml.RemoveSpace();
                    model.FullName = hdoc.DocumentNode.SelectSingleNode("//table[2]//table[1]//table[1]//td[1]/strong[1]").InnerText.RemoveSpace();
                    string[] baseInfo = hdoc.DocumentNode.SelectSingleNode("//table[2]//table[1]//table[1]//td[1]").InnerText.Trim().Replace("&nbsp;", "",Sys.StringComparison.OrdinalIgnoreCase).Split('|');

                    model.Address = hdoc.DocumentNode.SelectSingleNode("//table[2]//table[1]//table[2]/tbody[1]//tr[2]/td[1]//td[2]").InnerText.RemoveSpace();
                    model.Mobile = hdoc.DocumentNode.SelectSingleNode("//table[2]//table[1]//table[2]/tbody[1]//table[1]//td[2]").InnerText.RemoveSpace();
                    model.Gender = GetGender(baseInfo[1].RemoveSpace());
                    model.WorkLift = baseInfo[3].RemoveSpace();
                    model.Education = hdoc.DocumentNode.SelectSingleNode("//table[2]/tbody[1]/tr[1]/td[1]/table[2]//tbody[1]/tr[1]/td[1]/table//tbody[1]/tr[1]/td[2]/table[1]/tbody[1]/tr[4]/td[2]").InnerText.RemoveSpace();
                    model.University = hdoc.DocumentNode.SelectSingleNode("//table[2]/tbody[1]/tr[1]/td[1]/table[2]//tbody[1]/tr[1]/td[1]/table//tbody[1]/tr[1]/td[2]/table[1]/tbody[1]/tr[3]/td[2]").InnerText.RemoveSpace();
                    model.Major = hdoc.DocumentNode.SelectSingleNode("//table[2]/tbody[1]/tr[1]/td[1]/table[2]//tbody[1]/tr[1]/td[1]/table//tbody[1]/tr[1]/td[2]/table[1]/tbody[1]/tr[2]/td[2]").InnerText.RemoveSpace();
                }
                else
                {
                    msgText = message.GetTextBody(MimeKit.Text.TextFormat.Plain);
                }
                return model;

            }
            return null;
        }
    }
}
