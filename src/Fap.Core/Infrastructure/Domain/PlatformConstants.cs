using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Infrastructure.Domain
{
    public class PlatformConstants
    {

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
    }
}
