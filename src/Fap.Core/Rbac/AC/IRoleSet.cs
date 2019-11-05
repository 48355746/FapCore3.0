using Fap.Core.Rbac.Model;
using System.Collections.Generic;

namespace Fap.Core.Rbac.AC
{
    /// <summary>
    /// 角色集合
    /// </summary>
    public interface IRoleSet:IEnumerable<FapRole>
    {
        void Refresh();
        bool TryGetValue(string fid, out FapRole fapRole);
    }
}
