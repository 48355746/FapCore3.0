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
    public class LiePinParseEmailService : ParseEmailService
    {
        public LiePinParseEmailService(IDbContext dbContext, ILoggerFactory loggerFactory, IFapFileService fapFileService) : base(dbContext, loggerFactory, fapFileService)
        {
        }

        public override RcrtResume ParseHtmlContent(MimeMessage message)
        {
            string subject = message.Subject;
            if (subject.Contains("来自猎聘网", Sys.StringComparison.OrdinalIgnoreCase))
            {
                RcrtResume model = new RcrtResume();
                model.MessageId = message.MessageId;
                model.MessageFrom = string.Join(";", message.From.Mailboxes.Select(a => a.Address));

                model.ResumeStatus = RcrtResumeStatus.Created;
                model.MessageDate = message.Date.DateTime.ToString("yyyy-MM-dd:HH:mm:ss");
                model.Source = "LiePin";
                //获取txt内容，用于解析
                string msgText = string.Empty;
                //获取html内容
                string htmlPart = message.HtmlBody;
                if (htmlPart.IsPresent())
                {
                    model.HtmlContent = htmlPart;
                    HtmlDocument hdoc = new HtmlDocument();
                    hdoc.LoadHtml(htmlPart);
                    string title = hdoc.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[1]/div[2]/table[1]//table[2]/tbody[1]/tr[2]/td[1]/p[1]/span[1]").InnerText;
                    string[] titles = title.Split('|');
                    if (titles.Length > 1)
                    {
                        model.Address = titles[1].RemoveSpace();
                    }
                    model.ResumeName = titles[0].RemoveSpace();
                    model.Emails = hdoc.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[1]/div[2]/table[1]//table[2]/tbody[1]/tr[2]/td[1]/table[1]/tbody[1]/tr[4]/td[2]").InnerText.RemoveSpace();
                    model.FullName = hdoc.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[1]/div[2]/table[1]//table[2]/tbody[1]/tr[2]/td[1]/table[1]/tbody[1]/tr[2]/td[2]").InnerText.RemoveSpace();

                    model.Mobile = hdoc.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[1]/div[2]/table[1]//table[2]/tbody[1]/tr[2]/td[1]/table[1]/tbody[1]/tr[3]/td[2]").InnerText.RemoveSpace();
                    model.Gender = GetGender(hdoc.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[1]/div[2]/table[1]//table[2]/tbody[1]/tr[2]/td[1]/table[1]/tbody[1]/tr[3]/td[4]").InnerText.RemoveSpace());
                    model.WorkLift = hdoc.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[1]/div[2]/table[1]//table[2]/tbody[1]/tr[2]/td[1]/table[1]/tbody[1]/tr[2]/td[2]").InnerText.RemoveSpace();

                    model.Education = hdoc.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[1]/div[2]/table[1]//table[2]/tbody[1]/tr[2]/td[1]/table[1]/tbody[1]/tr[5]/td[2]").InnerText.RemoveSpace();
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
