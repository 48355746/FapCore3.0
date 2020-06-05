using Fap.Core.Infrastructure.Model;
using Fap.Model.Infrastructure;
using System.Collections.Generic;

namespace Fap.Core.Message
{
    public interface IMessageService
    {
        /// <summary>
        /// 日历
        /// </summary>
        /// <param name="calendar"></param>
        void SendEssCalendar(EssCalendar calendar);
        /// <summary>
        /// 邮件
        /// </summary>
        /// <param name="mail"></param>
        void SendMail(FapMail mail);
        void SendMailList(IEnumerable<FapMail> mailList);
        void AutoSendMail();
        /// <summary>
        /// 站内信
        /// </summary>
        /// <param name="message"></param>
        void SendMessage(FapMessage message);
        void SendMessageList(IEnumerable<FapMessage> messageList);
        /// <summary>
        /// 短信
        /// </summary>
        /// <param name="sms"></param>
        void SendSMS(FapSMS sms);
    }
}