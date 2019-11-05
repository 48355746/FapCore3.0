using Fap.Core.Rbac.Model;
using System.Collections.Generic;

namespace Fap.Core.Rbac.AC
{
    /// <summary>
    /// 用户集合
    /// </summary>
    public interface ISysUserSet : IEnumerable<FapUser>
    {
        void Refresh();
        bool TryGetValue(string fid, out FapUser fapUser);

        bool TryGetValueByUserName(string userName, out FapUser fapUser);
    }
}
