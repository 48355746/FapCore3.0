using Fap.Core.Rbac.Model;
using System.Collections.Generic;

namespace Fap.Core.Rbac.AC
{
    /// <summary>
    /// 按钮集
    /// </summary>
    public interface IButtonSet:IEnumerable<FapButton>
    {
        void Refresh();
        bool TryGetValue(string fid, out FapButton fapButton);
    }
}
