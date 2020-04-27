using System.Collections.Generic;
using System.Linq;
using MimeKit;
using Microsoft.Extensions.Logging;
using Fap.Core.DI;
using Fap.Core.Extensions;
using Fap.Core.DataAccess;
using Fap.Core.Annex;
using System;

namespace Fap.Hcm.Service.Recruit
{
    /// <summary>
    /// 广告门 邮件解析
    /// </summary>
    [Service]
    public class ADDoorParseEmailService : ParseEmailService
    {
        public ADDoorParseEmailService(IDbContext dbContext, ILoggerFactory loggerFactory, IFapFileService fapFileService) : base(dbContext, loggerFactory, fapFileService)
        {
        }

        public override RcrtResume ParseHtmlContent(MimeMessage mimeMessage)
        {
            string subject = mimeMessage.Subject;
            if (subject.Contains("来自广告门", StringComparison.OrdinalIgnoreCase))
            {
                RcrtResume resume = new RcrtResume();
                RcrtResume model = new RcrtResume();
                model.MessageId = mimeMessage.MessageId;
                model.MessageFrom = string.Join(";", mimeMessage.From.Mailboxes.Select(a => a.Address));
                subject = subject.Substring(0, subject.LastIndexOf(':') + 1);
                model.ResumeName = subject.ReplaceIgnoreCase("转发：", "").Trim();
                model.ResumeStatus = RcrtResumeStatus.Created;
                model.MessageDate = mimeMessage.Date.DateTime.ToString("yyyy-MM-dd:HH:mm:ss");
                //获取html内容
                string htmlPart = mimeMessage.HtmlBody;
                if (htmlPart.IsPresent())
                {
                    model.HtmlContent = htmlPart;
                }
                //获取txt内容，用于解析
                string messageText = string.Empty;
                string plainTextPart = mimeMessage.GetTextBody(MimeKit.Text.TextFormat.Plain);
                if (plainTextPart.IsPresent())
                {
                    // The message had a text/plain version - show that one

                }
                return resume;
            }
            return null;
        }
    }
}
