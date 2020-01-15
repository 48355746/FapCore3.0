using Fap.AspNetCore.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Hcm.Service.Organization
{
    public interface IOrganizationService
    {
        ResponseViewModel MoveDepartment(TreePostData postData);
    }
}
