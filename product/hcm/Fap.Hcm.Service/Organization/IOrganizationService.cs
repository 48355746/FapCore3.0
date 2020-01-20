using Fap.AspNetCore.Controls;
using Fap.AspNetCore.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Hcm.Service.Organization
{
    public interface IOrganizationService
    {
        ResponseViewModel MoveDepartment(TreePostData postData);
        ResponseViewModel MergeDepartment(MergeDeptModel mergeDept);
        List<TreeDataView> GetOrgJobTree();
        ResponseViewModel OperOrgJob(TreePostData postData);
    }
}
