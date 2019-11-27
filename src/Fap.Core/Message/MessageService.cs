using Fap.Core.DataAccess;
using Fap.Core.DI;
using Fap.Core.Infrastructure.Model;

namespace Fap.Core.Message
{
    /// <summary>
    /// Fap信息发送
    /// </summary>
    [Service]
    public class MessageService : IMessageService
    {
        private readonly IDbContext _dataAccessor;
        public MessageService(IDbContext dataAccessor)
        {
            _dataAccessor = dataAccessor;
        }
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailModel"></param>
        public  void SendMail(FapMail mail)
        {
            _dataAccessor.Insert(mail);
        }
        /// <summary>
        /// 发送站内信
        /// </summary>
        /// <param name="message"></param>
        public  void SendMessage(FapMessage message)
        {
            _dataAccessor.Insert(message);
        }
       

        /// <summary>
        /// 发送到员工日历事件
        /// </summary>
        /// <param name="calendar"></param>
        public  void SendEssCalendar(EssCalendar calendar)
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
    }
}
