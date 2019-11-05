using Fap.Core.Infrastructure.Config;
using System.Collections.Generic;

namespace Fap.Core.Rbac.AC
{
    /// <summary>
    /// 系统参数集合
    /// </summary>
    public interface ISysParamSet:IEnumerable<FapConfig>
    {
        void Refresh();
        bool TryGetValue(string fid, out FapConfig fapParam);

        bool TryGetValueByKey(string key, out FapConfig fapParam);
    }
}
