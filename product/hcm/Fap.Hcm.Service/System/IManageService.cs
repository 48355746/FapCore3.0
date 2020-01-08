using Fap.AspNetCore.Controls;
using Fap.AspNetCore.ViewModel;
using Fap.Core.Infrastructure.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Hcm.Service.System
{
    public interface IManageService
    {
        public void SetTheme(string theme,string userUid);
        bool ResetPasswor(IList<string> Uids);
        IEnumerable<TreeDataView> GetUserGroupTree();
        ResponseViewModel OperUserGroup(TreePostData postData);

        IEnumerable<TreeDataView> GetRoleGroupTree();
        IEnumerable<TreeDataView> GetRoleAndGroupTree();
        ResponseViewModel OperRoleGroup(TreePostData postData);

        IEnumerable<TreeDataView> GetBusinessRoleTree();
        ResponseViewModel OperBusinessRole(TreePostData postData);
        IEnumerable<TreeDataView> GetConfigGroupTree();
        ResponseViewModel OperConfigGroup(TreePostData postData);

        IEnumerable<GrpConfig> GetFapConfig(string configGroup);

        IEnumerable<TreeDataView> GetModuleTree();
        IEnumerable<TreeDataView> GetModuleAndMenuTree();

        IEnumerable<TreeDataView> GetAllDeptTree();
        IEnumerable<TreeDataView> GetMenuButtonTree();

        IEnumerable<TreeDataView> GetMenuEntityTree();

    }
}
