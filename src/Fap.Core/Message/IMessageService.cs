using Fap.Core.Infrastructure.Model;
using Fap.Model.Infrastructure;

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
        /// <summary>
        /// 站内信
        /// </summary>
        /// <param name="message"></param>
        void SendMessage(FapMessage message);
        /// <summary>
        /// 短信
        /// </summary>
        /// <param name="sms"></param>
        void SendSMS(FapSMS sms);
    }
}