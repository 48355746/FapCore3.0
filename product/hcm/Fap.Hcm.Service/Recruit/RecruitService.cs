using Dapper;
using Fap.Core.Annex;
using Fap.Core.DataAccess;
using Fap.Core.DI;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.Infrastructure.Enums;
using Fap.Core.Message.Mail;
using Fap.Core.Message.Mail.Core;
using Fap.Core.Message.Mail.Infrastructure.Internal;
using Microsoft.Extensions.Logging;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fap.Core.Extensions;
using Sys = System;
using Microsoft.Extensions.DependencyInjection;
using Fap.Core.Exceptions;
using Ardalis.GuardClauses;

namespace Fap.Hcm.Service.Recruit
{
    [Service]
    public class RecruitService : IRecruitService
    {
        private readonly IDbContext _dbContext;
        private readonly ILogger<RecruitService> _logger;
        private readonly IFapApplicationContext _applicationContext;
        private readonly IFapFileService _fapFileService;
        private readonly IServiceProvider _serviceProvider;
        public RecruitService(IDbContext dbContext, IServiceProvider serviceProvider, IFapFileService fapFileService, ILoggerFactory loggerFactory, IFapApplicationContext applicationContext)
        {
            _dbContext = dbContext;
            _logger = loggerFactory.CreateLogger<RecruitService>();
            _fapFileService = fapFileService;
            _applicationContext = applicationContext;
            _serviceProvider = serviceProvider;
        }
        /// <summary>
        /// 接收简历
        /// </summary>
        public async void ReceiveResume()
        {
            var mails = _dbContext.QueryWhere<RcrtMail>("Enabled=1");
            if (mails.Any())
            {
                foreach (var mail in mails)
                {
                    ReceiveFromMailBox(mail.Pop3Server, mail.Pop3Port, mail.Account, mail.Password, mail.UseSSL == 1 ? true : false, Core.Infrastructure.Enums.MailProtocolEnum.Pop3);
                }
            }
            else
            {
                Guard.Against.FapBusiness("请配置招聘邮箱");
            }
            async void ReceiveFromMailBox(string host, int port, string account, string pwd, bool useSSL, MailProtocolEnum protocol)
            {

                DynamicParameters param = new DynamicParameters();
                param.Add("EmpUid", _applicationContext.EmpUid);
                var seenUids = _dbContext.QueryWhere<RcrtMailReadRecord>("EmpUid=@EmpUid", param).Select(s => s.MessageUid);

                MailKitOptions moptions = new MailKitOptions() { Server = host, Port = port, Account = account, Password = pwd, Security = useSSL, SenderEmail = account, SenderName = account };
                IEmailService mailService = new EmailService(new MailKitProvider(moptions));

                if (protocol == MailProtocolEnum.Pop3)
                {
                    List<MimeMessage> mimeMsg = await mailService.RecieveEmailByPop3Async(seenUids).ConfigureAwait(false);
                    if (mimeMsg.Any())
                    {
                        var newUids = mimeMsg.Select(m => m.MessageId).ToList();
                        AddResumeByMail(mimeMsg);
                        AddReadRecord(newUids);
                    }

                }
                else if (protocol == MailProtocolEnum.Imap)
                {
                    //默认收取前一天至今
                    List<MimeMessage> mimeMsg = await mailService.RecieveEmailByImapAsync(seenUids).ConfigureAwait(false);
                    if (mimeMsg.Any())
                    {
                        var newUids = mimeMsg.Select(m => m.MessageId).ToList();
                        AddResumeByMail(mimeMsg);
                        AddReadRecord(newUids);
                    }
                }
            }
            void AddReadRecord(IEnumerable<string> newUids)
            {
                List<RcrtMailReadRecord> list = new List<RcrtMailReadRecord>();
                foreach (var uid in newUids)
                {
                    RcrtMailReadRecord model = new RcrtMailReadRecord();
                    model.EmpUid = _applicationContext.EmpUid;
                    model.MessageUid = uid;
                    list.Add(model);
                }
                if (list.Any())
                {
                    _dbContext.InsertBatchSql(list);
                }
            }

            void AddResumeByMail(IEnumerable<MimeMessage> messages)
            {
                var list = _dbContext.QueryAll<RcrtWebsite>();
                List<IParseEmailService> parseMailServiceList = new List<IParseEmailService>();
                foreach (var website in list)
                {
                    if (website.EmailAnalysisPlugin.IsPresent())
                    {
                        //解析插件
                        IParseEmailService analysis = ParseEmailService(website.EmailAnalysisPlugin);
                        if (analysis != null)
                        {
                            parseMailServiceList.Add(analysis);
                        }
                    }
                }
                if (parseMailServiceList.Any())
                {
                    foreach (var message in messages)
                    {
                        foreach (var service in parseMailServiceList)
                        {
                            var result = service.Analysis(message);
                            if (result)
                            {
                                break;
                            }
                        }
                    }
                }
            }
            IParseEmailService ParseEmailService(string parseEmailServiceClass)
            {
                IParseEmailService dataInterceptor = null;
                if (parseEmailServiceClass.IsPresent())
                {
                    //此处不能缓存，容易使session丢失，若要缓存的话需要重新赋值session
                    try
                    {
                        Type type = Sys.Type.GetType(parseEmailServiceClass);
                        if (type != null && type.GetInterface("IParseEmailService") != null)
                        {
                            //dataInterceptor = (IDataInterceptor)Activator.CreateInstance(type, new object[] { _serviceProvider, this });
                            dataInterceptor = (IParseEmailService)ActivatorUtilities.GetServiceOrCreateInstance(_serviceProvider, type);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.Message);
                        return null;
                    }
                }

                return dataInterceptor;
            }
        }
    }
}
