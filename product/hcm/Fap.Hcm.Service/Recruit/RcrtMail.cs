using Dapper.Contrib.Extensions;
using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Hcm.Service.Recruit
{
    /// <summary>
    /// 招聘邮箱
    /// </summary>
    public class RcrtMail : BaseModel
    {
        /// <summary>
        /// 招聘专员
        /// </summary>
        public string EmpUid { get; set; }
        /// <summary>
        /// 招聘专员 的显性字段MC
        /// </summary>
        [Computed]
        public string EmpUidMC { get; set; }
        /// <summary>
        /// 账号
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// SMTP服务器
        /// </summary>
        public string SmtpServer { get; set; }
        /// <summary>
        /// Smtp端口
        /// </summary>
        public int SmtpPort { get; set; }
        /// <summary>
        /// POP3服务器
        /// </summary>
        public string Pop3Server { get; set; }
        /// <summary>
        /// Pop3端口
        /// </summary>
        public int Pop3Port { get; set; }
        /// <summary>
        /// IMAP服务器
        /// </summary>
        public string ImapServer { get; set; }
        /// <summary>
        /// IMAP端口
        /// </summary>
        public int ImapPort { get; set; }
        /// <summary>
        /// 开启SSL
        /// </summary>
        public int UseSSL { get; set; }
        /// <summary>
        /// 启用
        /// </summary>
        public int Enabled { get; set; }
        /// <summary>
        /// 删除服务端邮件
        /// </summary>
        public int IsDelOriMail { get; set; }

    }
}
