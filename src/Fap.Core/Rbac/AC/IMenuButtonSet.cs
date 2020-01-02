using Fap.Core.Rbac.Model;
using System.Collections.Generic;

namespace Fap.Core.Rbac.AC
{
    /// <summary>
    /// 按钮集
    /// </summary>
    public interface IMenuButtonSet:IEnumerable<FapMenuButton>
    {
        void Refresh();
        bool TryGetValue(string menuUid, out IEnumerable<FapMenuButton> fapButtonList);
    }
}
