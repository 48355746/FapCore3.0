using System.Collections.Generic;
using System.Linq;
using MimeKit;
using Microsoft.Extensions.Logging;
using HtmlAgilityPack;
using Fap.Core.DataAccess;
using Fap.Core.Annex;
using Fap.Core.Extensions;
using Sys = System;
using Fap.Core.Utility;
using Fap.Core.DI;

namespace Fap.Hcm.Service.Recruit
{
    [Service]
    public class ZhiLianParseEmailService : ParseEmailService
    {
        public ZhiLianParseEmailService(IDbContext dbContext, ILoggerFactory loggerFactory, IFapFileService fapFileService) : base(dbContext, loggerFactory, fapFileService)
        {
        }
        public override RcrtResume ParseHtmlContent(MimeMessage message)
        {
            if (message.Subject.Contains("Zhaopin.com", Sys.StringComparison.OrdinalIgnoreCase))
            {
                int start = message.Subject.IndexOf("(Zhaopin.com) 应聘", Sys.StringComparison.OrdinalIgnoreCase) + "(Zhaopin.com) 应聘".Length;
                int length = message.Subject.LastIndexOf('-') - start;
                string subject = message.Subject.Substring(start, length);
                RcrtResume model = new RcrtResume();
                model.MessageId = message.MessageId;
                model.MessageFrom = string.Join(";", message.From.Mailboxes.Select(a => a.Address));
                model.ResumeName = subject.ReplaceIgnoreCase("转发：", "").Trim();
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
                    HtmlNode table = hdoc.DocumentNode.SelectSingleNode("//table[2]");
                    msgText = table.InnerText;
                }
                else
                {
                    msgText = message.GetTextBody(MimeKit.Text.TextFormat.Plain);
                }
                if (msgText.IsMissing())
                    return model;
                model.Source = "ZhiLianZhaoPin";
                using Sys.IO.StringReader sr = new Sys.IO.StringReader(msgText);
                string scontent = string.Empty;
                int i = 0;
                while ((scontent = sr.ReadLine()) != null)
                {
                    if (scontent.Trim().StartsWith("ID:", Sys.StringComparison.OrdinalIgnoreCase))
                    {
                        i = 2;
                        continue;
                    }
                    // 对s2操作
                    if (scontent.Trim().IsPresent())
                    {
                        i++;
                    }
                    else
                    {
                        continue;
                    }
                    if (i == 3 || i < 3)
                    {
                        i = 3;
                        //姓名
                        model.FullName = scontent;
                    }
                    if (i == 4)
                    {
                        //男|3年工作经验|1992年6月|未婚
                        string[] baseInfo = scontent.Split('|');
                        //性别
                        model.Gender = GetGender(baseInfo[0]);
                        //工作年限
                        model.WorkLift = baseInfo[1];
                    }
                    if (i == 5)
                    {
                        string[] otherInfo = scontent.Split('|');
                        if (otherInfo.Length > 1)
                        {
                            model.Address = otherInfo[0];
                            model.Education = otherInfo[1];
                        }
                    }
                    if (i == 6)
                    {
                        model.Mobile = RegexUtils.IsMobileNo(scontent) ? scontent : "";
                    }
                    if (i == 7)
                    {
                        model.Emails = RegexUtils.IsEmail(scontent) ? scontent : "";
                    }

                }
                return model;

            }
            return null;
        }
    }
}