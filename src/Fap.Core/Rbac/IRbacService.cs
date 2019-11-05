using System.Collections.Generic;
using Fap.Core.Rbac.Model;

namespace Fap.Core.Rbac
{
    public interface IRbacService
    {
        bool IsInRole(string roleFid);
        string GetRoleDataWhere(string tableName);
        IEnumerable<FapRoleColumn> GetUserColumnList();
        string GetUserDeptAuthorityWhere(bool hasPartPower = false);
        IEnumerable<OrgDept> GetUserDeptList(string historyDate = "");
        IEnumerable<FapRoleMenu> GetUserMenuList();
        IEnumerable<FapRoleReport> GetUserReportList();
        IEnumerable<FapRole> GetUserRoleList();
    }
}