using System.Collections.Generic;
using Fap.Core.Infrastructure.Enums;
using Fap.Core.Rbac.Model;

namespace Fap.Core.Infrastructure.Domain
{
    public interface IFapApplicationContext
    {
        string DeptCode { get; }
        string DeptName { get; }
        string DeptUid { get; }
        string EmpName { get; }
        string EmpUid { get; }
        string GroupUid { get; }
        string OnlineUserUid { get; }
        string OrgUid { get; }
        IEnumerable<string> Roles { get; }
        string CurrentRoleUid { get; set; }
        string UserName { get; }
        string UserType { get; }
        string UserUid { get; }
        /// <summary>
        /// 用户会话标识。一个用户（Account）可以对应有多个会话，约定会话标识与账户标识相等的那个会话为这个账户的主会话。
        /// <remarks>
        /// 主会话在用户第一次登录使用系统的时候创建并持久跟踪，非主会话由安全管理员按需创建。用户登录成功时该用户的会话列表会加载进应用系统内存，然后由会话激活策略基于会话的属性取其中之一激活。
        /// </remarks>
        /// </summary>
        FapRole Role { get; set; }
        /// <summary>
        /// 当前会话所属的 账户 = 用户。
        /// </summary>
        /// <returns></returns>
        FapUser Account { get; }
        /// <summary>
        /// 在线用户
        /// </summary>
        FapOnlineUser OnlineUser { get; }
        /// <summary>
        /// 员工信息
        /// </summary>
        Employee Employee { get; }


        /// <summary>
        /// 多语语种
        /// </summary>
        MultiLanguageEnum Language { get; }
    }
}