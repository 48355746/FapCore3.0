using Fap.Core.Rbac.Model;
using System.Collections.Generic;

namespace Fap.Core.Rbac.AC
{
    /// <summary>
    /// 组织集合
    /// </summary>
    public interface IOrgDeptSet:IEnumerable<OrgDept>
    {
        void Refresh();
        bool TryGetValue(string fid, out OrgDept fapOrg);
        bool TryGetValueByPid(string pid, out IEnumerable<OrgDept> childDepts);
    }
}
