using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Infrastructure.Domain
{
    public class FapPlatformConstants
    {
        /// <summary>
        /// 管理员账号
        /// </summary>
        public const string Administrator = "hr";
        /// <summary>
        /// 临时文件夹，临时存放导入上传的文件等
        /// </summary>
        public const string TemporaryFolder = "Temporary";
        /// <summary>
        /// 变量替换正则表达式${Variable}
        /// </summary>
        public const string VariablePattern = @"\$\{\S+?\}";
        /// <summary>
        /// 集合替换正则表达式
        /// </summary>
        public const string CollectionPattern = @"\{\{\S+?\}\}";
        /// <summary>
        /// 记住密码Cookie
        /// </summary>
        public const string RememberPasswordCookieName = "XJpZW2hY0MFjAxNl8yMDIw";
        /// <summary>
        /// 自定义分隔符
        /// </summary>

        public const string SplitSeparator = "@@~~!!!!^^@@";
        /// <summary>
        /// 在线用户列表缓存KEY
        /// </summary>
        public const string CachedOnlineUserList = "FAP::CACHEDONLINEUSERLIST";
        /// <summary>
        /// 普通用户角色Fid值
        /// </summary>
        public const string CommonUserRoleFid = "00000000000000000000";
        /// <summary>
        /// 管理员角色Fid值
        /// </summary>
        public const string AdministratorRoleFid = "11111111111111111111";
        /// <summary>
        ///部门权限替换字符
        /// </summary>
        public const string DepartmentAuthority = "FAP::&&DEPTAUTHORITY&&";
        //邮件发送配置
        public const string MailAccount = "system.mail.account";
        public const string MailPassword = "system.mail.password";
        public const string MailPort = "system.mail.port";
        public const string MailServer = "system.mail.host";
        public const string MailAccountName = "system.mail.accountname";
    }
}
