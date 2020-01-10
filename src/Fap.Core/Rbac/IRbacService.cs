using System.Collections.Generic;
using Fap.Core.Rbac.Model;
using Fap.Model.Infrastructure;

namespace Fap.Core.Rbac
{
    public interface IRbacService
    {
        #region UserGroup
        IEnumerable<FapUserGroup> GetAllUserGroup();
        long CreateUserGroup(FapUserGroup userGroup);
        bool DeleteUserGroup(string fid);
        bool EditUserGroup(FapUserGroup userGroup);
        #endregion
        #region RoleGroup
        IEnumerable<FapRoleGroup> GetAllRoleGroup();
        long CreateRoleGroup(FapRoleGroup roleGroup);
        bool DeleteRoleGroup(string fid);
        bool EditRoleGroup(FapRoleGroup roleGroup);
        #endregion
        #region BizRole
        IEnumerable<FapBizRole> GetAllBizRole();
        long CreateBizRole(FapBizRole bizRole);
        bool DeleteBizRole(string fid);
        bool EditBizRole(FapBizRole bizRole);
        #endregion

        FapMenu GetCurrentMenu();
        FapRole GetCurrentRole();
        IEnumerable<FapRole> GetAllRole();
      
       
        bool AddRoleMenu(string roleUid, IEnumerable<FapRoleMenu> menus);
        bool AddRoleDept(string roleUid, IEnumerable<FapRoleDept> depts);
        bool AddRoleColumn(string roleUid, IEnumerable<FapRoleColumn> columns, int editType);
        void AddRoleUser(IEnumerable<FapRoleUser> users);
        void AddRoleReport(string roleUid, IEnumerable<FapRoleReport> rpts);
        void AddRoleButton(string roleUid, IEnumerable<FapRoleButton> roleButtons);
        void AddRoleRole(string roleUid, IEnumerable<FapRoleRole> roleRoles);
        
        IEnumerable<FapRoleData> GetRoleDataList(string roleUid);
        IEnumerable<FapRoleColumn> GetRoleColumnList(string roleUid);
        IEnumerable<OrgDept> GetRoleDeptList(string roleUid,string historyDate = "");
        IEnumerable<FapRoleMenu> GetRoleMenuList(string roleUid);
        IEnumerable<FapRoleReport> GetRoleReportList(string roleUid);
        IEnumerable<FapRoleUser> GetRoleUserList(string roleUid);
        IEnumerable<FapRole> GetUserRoleList(string userUid);
        IEnumerable<FapRoleRole> GetRoleRoleList(string roleUid);
        IEnumerable<FapRoleButton> GetRoleButtonList(string roleUid);
        /// <summary>
        /// 获取按钮授权
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        string GetMenuButtonAuthorized(FapMenuButton button);
        string GetMenuColumnAuthorized(FapMenuColumn column);
    }
}