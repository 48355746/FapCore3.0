using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Infrastructure.Model
{
    /// <summary>
    /// 邮件
    /// </summary>
    public class FapMail :BaseModel
    {
        /// <summary>
        /// 发送人
        /// </summary>
        public string Sender { get; set; }
        /// <summary>
        /// 发送人邮箱地址
        /// </summary>
        public string SenderEmailAddress { get; set; }
        /// <summary>
        /// 收件人
        /// </summary>
        public string Recipient { get; set; }
        /// <summary>
        /// 收件人邮箱地址
        /// </summary>
        public string RecipientEmailAddress { get; set; }
        /// <summary>
        /// 抄送人邮件地址
        /// </summary>
        public string CCEmailAddress { get; set; }
        /// <summary>
        /// 密送人邮件地址
        /// </summary>
        public string BCCEmailAddress { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// 邮件内容
        /// </summary>
        public string MailContent { get; set; }
        /// <summary>
        /// 附件
        /// </summary>
        public string Attachment { get; set; }
        /// <summary>
        /// 是否分别发送
        /// </summary>
        public int IsSeparate { get; set; }
        /// <summary>
        /// 发送状态
        /// </summary>
        public int SendStatus { get; set; }
        /// <summary>
        /// 发送次数
        /// </summary>
        public int SendCount { get; set; }
        /// <summary>
        /// 失败原因
        /// </summary>
        public string Failures { get; set; }
        /// <summary>
        /// 实际发送时间
        /// </summary>
        public string SendTime { get; set; }
        /// <summary>
        /// 预计发送时间
        /// </summary>
        public string PreSendTime { get; set; }
        /// <summary>
        /// 邮件分类
        /// </summary>
        public string MailCategory { get; set; }

    }
}
