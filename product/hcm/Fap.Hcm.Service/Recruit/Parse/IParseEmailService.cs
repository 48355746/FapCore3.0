using Fap.Core.Annex;
using Fap.Core.DataAccess;
using Fap.Core.Infrastructure.Model;
using Fap.Core.Utility;
using Microsoft.Extensions.Logging;
using MimeKit;
using Sys = System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Fap.Core.Extensions;
namespace Fap.Hcm.Service.Recruit
{
    /// <summary>
    /// 解析招聘网站邮件
    /// </summary>
    public interface IParseEmailService
    {
        bool Analysis(MimeMessage mimeMessages,IEnumerable<RcrtResume> blacklist);

    }
    public abstract class ParseEmailService : IParseEmailService
    {
        private readonly IDbContext _dbContext;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IFapFileService _fapFileService;
        public ParseEmailService(IDbContext dbContext, ILoggerFactory loggerFactory, IFapFileService fapFileService)
        {
            _dbContext = dbContext;
            _loggerFactory = loggerFactory;
            _fapFileService = fapFileService;
        }
        public bool Analysis(MimeMessage mimeMessage, IEnumerable<RcrtResume> blacklist)
        {
            RcrtResume resume = ParseHtmlContent(mimeMessage);
            if (resume == null)
            {
                return false;
            }
            if (blacklist.Any() && resume.Mobile.IsPresent())
            {
                //黑名单
                if (blacklist.Select(r => r.Mobile).Contains(resume.Mobile))
                {
                    return false;
                }
            }
            resume.Attachment = ParseAttachment(mimeMessage);
            _dbContext.Insert(resume);
            return true;

        }
        public abstract RcrtResume ParseHtmlContent(MimeMessage mimeMessage);
        public string ParseAttachment(MimeMessage message)
        {
            string uuid = UUIDUtils.Fid;
            //解析附件
            // Build up the attachment list
            var attachments = message.Attachments;
            if (attachments != null && attachments.Any())
            {
                //FapFileServiceHelper fh = new FapFileServiceHelper();
                foreach (MimePart attachmentPart in attachments)
                {
                    if (attachmentPart.IsAttachment)
                    {
                        FapAttachment attachment = new FapAttachment();
                        attachment.Bid = uuid;
                        attachment.FileName = attachmentPart.ContentDisposition.FileName;
                        attachment.FileType = attachmentPart.ContentType.MimeType;
                        if (attachment != null)
                        {
                            using (var cancel = new Sys.Threading.CancellationTokenSource())
                            {
                                string filePath = Path.Combine(Sys.Environment.CurrentDirectory, attachment.FileName);
                                using (Stream stream = new MemoryStream())
                                {
                                    attachmentPart.Content.DecodeTo(stream, cancel.Token);
                                    stream.Seek(0, SeekOrigin.Begin);
                                    string attFid = _fapFileService.UploadFile(stream, attachment);
                                }
                            }
                        }
                    }
                }
            }
            return uuid;
        }
        protected string GetGender(string name)
        {
            if (name.Trim() == "男")
            {
                return "male";
            }
            else if (name.Trim() == "女")
            {
                return "female";
            }
            else
            {
                return "unknown";
            }
        }
    }
}
