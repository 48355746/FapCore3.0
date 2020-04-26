using System.Collections.Generic;
using System.Linq;
using Fap.Core.Annex;
using Fap.Core.DataAccess;
using Fap.Core.DI;
using Fap.Core.Extensions;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using MimeKit;
using Sys = System;

namespace Fap.Hcm.Service.Recruit
{
    /// <summary>
    /// Boss直聘邮件解析
    /// </summary>
    [Service]
    public class BossParseEmailService : ParseEmailService
    {
        public BossParseEmailService(IDbContext dbContext, ILoggerFactory loggerFactory, IFapFileService fapFileService) : base(dbContext, loggerFactory, fapFileService)
        {
        }

        public override RcrtResume ParseHtmlContent(MimeMessage message)
        {
            string subject = message.Subject;
            if (subject.Contains("Boss直聘", Sys.StringComparison.OrdinalIgnoreCase))
            {
                subject = subject.ReplaceIgnoreCase("转发:", "").Trim().Split(' ')[3];
                RcrtResume model = new RcrtResume();
                model.MessageId = message.MessageId;
                model.MessageFrom = string.Join(";", message.From.Mailboxes.Select(a => a.Address));
                model.ResumeName = subject;
                model.ResumeStatus = RcrtResumeStatus.Created;
                model.MessageDate = message.Date.DateTime.ToString("yyyy-MM-dd:HH:mm:ss");
                //获取txt内容，用于解析
                string msgText = string.Empty;
                //获取html内容
                string htmlPart = message.HtmlBody;
                if (htmlPart.IsPresent())
                {
                    model.HtmlContent = htmlPart;
                    HtmlDocument hdoc = new HtmlDocument();
                    hdoc.LoadHtml(htmlPart);
                    HtmlNode table = hdoc.DocumentNode.SelectSingleNode("//table");
                    msgText = table.InnerText;
                }
                else
                {
                    msgText = message.GetTextBody(MimeKit.Text.TextFormat.Plain);
                }
                if (msgText.IsMissing())
                    return model;
                model.Source = "Boss";
                using Sys.IO.StringReader sr = new Sys.IO.StringReader(msgText);
                string scontent = string.Empty;
                int i = 0;
                while ((scontent = sr.ReadLine()) != null)
                {
                    // 对s2操作
                    if (scontent.Trim().IsPresent())
                    {
                        i++;
                    }
                    else
                    {
                        continue;
                    }
                    if (i < 4)
                    {
                        continue;
                    }
                    if (i == 4)
                    {
                        //姓名
                        model.FullName = scontent;
                    }
                    if (i == 5)
                    {
                        model.Gender = GetGender(scontent);
                    }
                    if (i == 7)
                    {
                        string[] otherInfo = scontent.Split(' ');
                        //工作年限
                        if (otherInfo.Length > 1)
                        {
                            model.WorkLift = otherInfo[1];
                            model.Education = otherInfo[0];
                        }
                    }
                    return model;
                }
            }
            return null;
        }
    }
}
