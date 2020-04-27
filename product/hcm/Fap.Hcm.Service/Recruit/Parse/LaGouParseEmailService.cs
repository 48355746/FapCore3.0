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
    /// <summary>
    /// 拉钩网 邮件解析
    /// </summary>
    [Service]
    public class LaGouParseEmailService : ParseEmailService
    {
        public LaGouParseEmailService(IDbContext dbContext, ILoggerFactory loggerFactory, IFapFileService fapFileService) : base(dbContext, loggerFactory, fapFileService)
        {
        }

    
        public override RcrtResume ParseHtmlContent(MimeMessage message)
        {
            string subject = message.Subject;
            if (subject.Contains("来自拉勾", Sys.StringComparison.OrdinalIgnoreCase))
            {
                RcrtResume model = new RcrtResume();
                model.MessageId = message.MessageId;
                model.MessageFrom = string.Join(";", message.From.Mailboxes.Select(a => a.Address));

                model.ResumeStatus = RcrtResumeStatus.Created;
                model.MessageDate = message.Date.DateTime.ToString("yyyy-MM-dd:HH:mm:ss");
                model.Source = "LaGou";
                //获取txt内容，用于解析
                string msgText = string.Empty;
                //获取html内容
                string htmlPart = message.HtmlBody;
                if (htmlPart.IsPresent())
                {
                    model.HtmlContent = htmlPart;
                    HtmlDocument hdoc = new HtmlDocument();
                    hdoc.LoadHtml(htmlPart);
                    model.ResumeName = hdoc.DocumentNode.SelectSingleNode("//table/tbody[1]/tr[2]/td[1]/div[2]/span[1]").InnerText.RemoveSpace();
                    string[] baseInfo = hdoc.DocumentNode.SelectSingleNode("//table/tbody[1]/tr[2]/td[1]/div[3]/div[1]").InnerText.Trim().Replace("&nbsp;", "", Sys.StringComparison.OrdinalIgnoreCase).Split('|');
                    if (baseInfo.Length > 4)
                    {
                        string[] address = baseInfo[4].Split("\r\n");
                        model.Address = address[0];
                        string[] edu = address[1].Substring("毕业院校：".Length).Split('·');
                        if (edu.Length > 1)
                        {
                            model.University = edu[1];
                        }
                    }
                    model.Education = baseInfo[2];

                    model.FullName = baseInfo[0].RemoveSpace();

                    model.Gender = GetGender(baseInfo[1].RemoveSpace());
                    model.WorkLift = baseInfo[3].RemoveSpace();

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
