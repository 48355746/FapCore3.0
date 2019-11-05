using Fap.Core.Rbac.Model;
using System.Collections.Generic;

namespace Fap.Core.Rbac.AC
{
    /// <summary>
    /// 菜单集合
    /// </summary>
    public interface IMenuSet:IEnumerable<FapMenu>
    {
        void Refresh();
        bool TryGetValue(string fid, out FapMenu fapMenu);
        bool TryGetValueByPath(string path, out FapMenu fapMenu);
    }
}
