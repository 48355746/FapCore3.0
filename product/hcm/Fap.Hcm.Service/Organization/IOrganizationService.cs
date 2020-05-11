using Fap.AspNetCore.Controls;
using Fap.AspNetCore.ViewModel;
using Fap.Core.Rbac.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Hcm.Service.Organization
{
    public interface IOrganizationService
    {
        ResponseViewModel MoveDepartment(TreePostData postData);
        ResponseViewModel MergeDepartment(MergeDeptModel mergeDept);
        List<TreeDataView> GetOrgJobGroupTree();
        IEnumerable<OrgDept> GetDominationDepartment();
        IEnumerable<TreeDataView> GetDominationDepartmentTree();
        //ResponseViewModel OperOrgJob(TreePostData postData);
    }
}
