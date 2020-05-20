using Fap.Core.Rbac.Model;
using System.Collections.Generic;

namespace Fap.Core.Rbac
{
    /// <summary>
    /// 登录服务
    /// </summary>
    public interface ILoginService
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns></returns>
        FapUser Login(string userName);
        /// <summary>
        /// 增加尝试次数
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        FapUser AddTryTimes(FapUser user);
        /// <summary>
        /// 更新最后登录时间
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        FapUser UpdateLastLoginTime(FapUser user);
        /// <summary>
        /// 获取用户所有角色集合
        /// </summary>
        /// <param name="userUid"></param>
        /// <returns></returns>
        IEnumerable<FapRole> GetUserRoles(string userUid);
        /// <summary>
        /// 获取角色对应的菜单
        /// </summary>
        /// <param name="roleUid"></param>
        /// <returns></returns>
        IEnumerable<FapRoleMenu> GetRoleMenus(string roleUid);
        /// <summary>
        /// 获取用户默认角色
        /// </summary>
        /// <param name="userUid"></param>
        /// <returns></returns>
        string GetUserDefaultRole(string userUid);
        /// <summary>
        /// 更新用户的默认角色
        /// </summary>
        /// <param name="roleUid"></param>
        /// <param name="userUid"></param>
        void UpdateUserDefaultRole(string roleUid, string userUid);
        /// <summary>
        /// 登出
        /// </summary>
        void Logout();
    }
}
