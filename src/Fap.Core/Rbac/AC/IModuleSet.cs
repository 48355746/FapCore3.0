using Fap.Core.Rbac.Model;
using System.Collections.Generic;

namespace Fap.Core.Rbac.AC
{
    /// <summary>
    /// 模块集合
    /// </summary>
    public interface IModuleSet : IEnumerable<FapModule>
    {
        void Refresh();
        bool TryGetValue(string fid, out FapModule fapModule);
    }
}
