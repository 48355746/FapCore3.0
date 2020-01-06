using System.Collections.Generic;
using Fap.Core.Rbac.Model;

namespace Fap.Core.Rbac
{
    public interface IRbacService
    {
        IEnumerable<FapUserGroup> GetUserGroups();
        string UserGroupOperation(string operation, string id, string parent, string text);
        FapRole GetCurrentRole();
        string RoleGroupOperation(string operation, string id, string parent, string text);
        string BusinessRoleOperation(string operation, string id, string parent, string text);
        bool AddRoleMenu(string roleUid, IEnumerable<FapRoleMenu> menus);
        bool AddRoleDept(string roleUid, IEnumerable<FapRoleDept> depts);
        bool AddRoleColumn(string roleUid, IEnumerable<FapRoleColumn> columns, int editType);
        void AddRoleUser(IEnumerable<FapRoleUser> users);
        void AddRoleReport(string roleUid, IEnumerable<FapRoleReport> rpts);
        void AddRoleButton(string roleUid, IEnumerable<FapRoleButton> roleButtons);
        void AddRoleRole(string roleUid, IEnumerable<FapRoleRole> roleRoles);
        bool IsInRole(string roleFid);
        string GetRoleDataWhere(string tableName);
        IEnumerable<FapRoleColumn> GetUserColumnList();
        string GetUserDeptAuthorityWhere(bool hasPartPower = false);
        IEnumerable<OrgDept> GetUserDeptList(string historyDate = "");
        IEnumerable<FapRoleMenu> GetUserMenuList();
        IEnumerable<FapRoleReport> GetUserReportList();
        IEnumerable<FapRole> GetUserRoleList();
        /// <summary>
        /// 获取按钮授权
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        string GetButtonAuthorized(FapMenuButton button);
    }
}