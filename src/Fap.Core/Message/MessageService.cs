using Fap.Core.Annex;
using Fap.Core.DataAccess;
using Fap.Core.DI;
using Fap.Core.Extensions;
using Fap.Core.Infrastructure.Config;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.Infrastructure.Model;
using Fap.Core.Message.Mail;
using Fap.Core.Message.Mail.Core;
using Fap.Core.Message.Mail.Infrastructure.Internal;
using Microsoft.Extensions.Logging;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fap.Core.Message
{
    /// <summary>
    /// Fap信息发送
    /// </summary>
    [Service]
    public class MessageService : IMessageService
    {
        private readonly IFapFileService _fileService;
        private readonly IDbContext _dataAccessor;
        private IFapConfigService _configService;
        private readonly ILogger<MessageService> _logger;
        public MessageService(IDbContext dataAccessor, IFapConfigService configService, ILogger<MessageService> logger, IFapFileService fileService)
        {
            _dataAccessor = dataAccessor;
            _configService = configService;
            _logger = logger;
            _fileService = fileService;
        }
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailModel"></param>
        public void SendMail(FapMail mail)
        {
            _dataAccessor.Insert(mail);
        }
        /// <summary>
        /// 发送站内信
        /// </summary>
        /// <param name="message"></param>
        public void SendMessage(FapMessage message)
        {
            _dataAccessor.Insert(message);
        }


        /// <summary>
        /// 发送到员工日历事件
        /// </summary>
        /// <param name="calendar"></param>
        public void SendEssCalendar(EssCalendar calendar)
        {
            _dataAccessor.Insert(calendar);
        }

        /// <summary>
        /// 短信
        /// </summary>
        /// <param name="sms"></param>
        public void SendSMS(FapSMS sms)
        {
            _dataAccessor.Insert(sms);
        }
        /// <summary>
        /// Scheduler 发送邮件
        /// </summary>
        [Transactional]
        public void AutoSendMail()
        {
            string account = _configService.GetSysParamValue(FapPlatformConstants.MailAccount);
            string password = _configService.GetSysParamValue(FapPlatformConstants.MailPassword);
            string server = _configService.GetSysParamValue(FapPlatformConstants.MailServer);
            int? port = _configService.GetSysParamValue(FapPlatformConstants.MailPort).ToInt();
            string accountName = _configService.GetSysParamValue(FapPlatformConstants.MailAccountName);
            MailKitOptions mailOption = new MailKitOptions() { Account = account, Password = password, Port = (int)port, Server = server, SenderEmail = account, SenderName = accountName };
            IEmailService service = new EmailService(new MailKitProvider(mailOption));
            try
            {
                SendEmail();
            }
            catch (Exception ex)
            {
                _logger.LogError($"任务调度 邮件发送异常信息:{ex.Message}");
            }
            void SendEmail()
            {
                //未发送或发送次数小于5，注：可能发送失败，每次发送次数会增加
                IEnumerable<FapMail> listMails = _dataAccessor.Query<FapMail>($"select * from FapMail where SendStatus=0 and SendCount<5");
                if (listMails == null || listMails.Count() < 1)
                {
                    _logger.LogInformation("无邮件可发");
                    return;
                }
                //招聘相关的特殊处理
                var rcrtMails = listMails.Where<FapMail>(m => m.MailCategory == "招聘相关");
                IEnumerable<dynamic> rcrtEmailAddress = new List<dynamic>();
                if (rcrtMails != null && rcrtMails.Count() > 0)
                {
                    rcrtEmailAddress = _dataAccessor.Query("select * from RcrtMail where Enabled=1");
                }
                foreach (FapMail mail in listMails)
                {
                    if (mail.RecipientEmailAddress.IsMissing())
                    {
                        mail.SendCount = 5;
                        mail.Failures = "收件人为空";
                        _dataAccessor.Update<FapMail>(mail);
                        continue;
                    }
                    //预计发送时间
                    if (mail.PreSendTime.IsPresent())
                    {
                        DateTime pre = Convert.ToDateTime(mail.PreSendTime);
                        //不到预计时间不发送
                        if (pre > DateTime.Now)
                        {
                            continue;
                        }
                    }
                    List<AttachmentInfo> listAtts = new List<AttachmentInfo>();
                    var attachemts = _fileService.FileList(mail.Attachment);
                    if (attachemts != null && attachemts.Any())
                    {
                        foreach (var att in attachemts)
                        {
                            listAtts.Add(new AttachmentInfo { ContentType = att.FileType, UniqueId = att.Id.ToString(), FileName = att.FileName, Stream = _fileService.GetFileStream(att) });
                        }
                    }
                    string[] arryRec = mail.RecipientEmailAddress.Split(';');
                    string failMsg = string.Empty;
                    //是否分别发送,分别发送的情况下 就没有抄送和密送
                    if (mail.IsSeparate == 1)
                    {
                        if (arryRec != null && arryRec.Length > 0)
                        {
                            foreach (string rec in arryRec)
                            {
                                string emailAddr = rec.Trim();
                                if (emailAddr.IsPresent())
                                {
                                    try
                                    {
                                        //招聘的特殊处理
                                        if (mail.MailCategory == "招聘相关" && mail.SenderEmailAddress.IsPresent())
                                        {
                                            var rcrtMail = rcrtEmailAddress.FirstOrDefault(m => m.Account == mail.SenderEmailAddress);
                                            if (rcrtMail != null)
                                            {
                                                try
                                                {
                                                    MailKitOptions rcrtMailOption = new MailKitOptions() { Account = rcrtMail.Account, Password = rcrtMail.Password, Port = Convert.ToInt32(rcrtMail.SmtpPort), Server = rcrtMail.Smtp, SenderEmail = rcrtMail.Account, SenderName = "招聘负责人" };
                                                    IEmailService mailService = new EmailService(new MailKitProvider(rcrtMailOption));
                                                    mailService.Send(emailAddr, mail.Subject, mail.MailContent, MimeKit.Text.TextFormat.Html, listAtts);
                                                }
                                                catch (Exception ex)
                                                {
                                                    failMsg += $"{ex.Message}";
                                                    _logger.LogError($"招聘相关 邮件发送失败:{ex.Message}");
                                                }
                                                _logger.LogInformation("招聘相关 任务调度,定时执行 发送邮件 成功=====>" + mail.Fid);

                                            }

                                        }
                                        else
                                        {
                                            try
                                            {
                                                service.Send(emailAddr, mail.Subject, mail.MailContent, MimeKit.Text.TextFormat.Html, listAtts);
                                            }
                                            catch (Exception ex)
                                            {
                                                failMsg += $"{ex.Message}";
                                                _logger.LogError($"邮件发送失败:{ex.Message}");
                                            }
                                            _logger.LogInformation("任务调度,定时执行 发送邮件 成功=====>" + mail.Fid);
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.LogError($"分割发送邮件异常，邮件地址：{rec + ex.Message}");
                                        continue;
                                    }

                                }
                            }



                        }
                    }
                    else
                    {
                        if (mail.MailCategory == "招聘相关" && mail.SenderEmailAddress.IsPresent())
                        {
                            var rcrtMail = rcrtEmailAddress.FirstOrDefault(m => m.Account == mail.SenderEmailAddress);
                            if (rcrtMail != null)
                            {
                                try
                                {
                                    MailKitOptions rcrtMailOption = new MailKitOptions() { Account = rcrtMail.Account, Password = rcrtMail.Password, Port = Convert.ToInt32(rcrtMail.SmtpPort), Server = rcrtMail.Smtp, SenderEmail = rcrtMail.Account, SenderName = "招聘负责人" };
                                    IEmailService mailService = new EmailService(new MailKitProvider(rcrtMailOption));
                                    mailService.Send(mail.RecipientEmailAddress, mail.CCEmailAddress, mail.BCCEmailAddress, mail.Subject, mail.MailContent, TextFormat.Html, listAtts);
                                }
                                catch (Exception ex)
                                {
                                    failMsg += $"{ex.Message}";
                                    _logger.LogError($"招聘相关 合并发送邮件发送失败:{ex.Message}");
                                }
                                _logger.LogInformation("招聘相关 任务调度,定时执行 发送邮件 成功=====>" + mail.Fid);

                            }
                            else
                            {
                                try
                                {
                                    service.Send(mail.RecipientEmailAddress, mail.CCEmailAddress, mail.BCCEmailAddress, mail.Subject, mail.MailContent, TextFormat.Html, listAtts);
                                }
                                catch (Exception ex)
                                {
                                    failMsg += $"{ex.Message}";
                                    _logger.LogError($"招聘相关 合并发送邮件发送失败:{ex.Message}");
                                }
                                _logger.LogInformation("招聘相关 任务调度,定时执行 发送邮件 成功=====>" + mail.Fid);

                            }
                        }
                        else
                        {
                            try
                            {
                                service.Send(mail.RecipientEmailAddress, mail.CCEmailAddress, mail.BCCEmailAddress, mail.Subject, mail.MailContent, TextFormat.Html, listAtts);
                            }
                            catch (Exception ex)
                            {
                                failMsg += $"{ex.Message}";
                                _logger.LogError($" 合并发送邮件发送失败:{ex.Message}");
                            }
                            _logger.LogInformation(" 任务调度,定时执行 发送邮件 成功=====>" + mail.Fid);
                        }
                    }
                    if (failMsg.IsPresent())
                    {
                        mail.SendCount += 1;
                        mail.SendStatus = 1;
                        mail.Failures = failMsg;
                        _dataAccessor.Update<FapMail>(mail);
                    }
                    else
                    {
                        mail.SendStatus = 1;
                        mail.SendCount += 1;
                        _dataAccessor.Update<FapMail>(mail);
                    }
                   
                }
            }
        }

        public void SendMailList(IEnumerable<FapMail> mailList)
        {
            _dataAccessor.InsertBatchSql(mailList);
        }

        public void SendMessageList(IEnumerable<FapMessage> messageList)
        {
            _dataAccessor.InsertBatchSql(messageList);
        }
    }
}

