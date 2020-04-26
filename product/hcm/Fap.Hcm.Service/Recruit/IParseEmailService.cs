using Fap.Core.Annex;
using Fap.Core.DataAccess;
using Fap.Core.Infrastructure.Model;
using Fap.Core.Utility;
using Microsoft.Extensions.Logging;
using MimeKit;
using Sys=System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fap.Hcm.Service.Recruit
{
    /// <summary>
    /// 解析招聘网站邮件
    /// </summary>
    public interface IParseEmailService
    {
        void Analysis(IEnumerable<MimeMessage> mimeMessages);

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
        public void Analysis(IEnumerable<MimeMessage> mimeMessages)
        {
            if (mimeMessages.Any())
            {
                //简历
                List<RcrtResume> list = new List<RcrtResume>();
                foreach (var mimeMessage in mimeMessages)
                {
                    RcrtResume resume = ParseHtmlContent(mimeMessage);
                    if (resume == null)
                    {
                        continue;
                    }
                    resume.Attachment = ParseAttachment(mimeMessage);
                    list.Add(resume);
                }            
                   
                 _dbContext.InsertBatchSql(list);
               
            }
        }
        public abstract RcrtResume ParseHtmlContent(MimeMessage mimeMessage);
        public  string ParseAttachment(MimeMessage message)
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
