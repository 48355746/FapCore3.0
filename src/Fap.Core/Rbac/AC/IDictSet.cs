using Fap.Model.MetaData;
using System.Collections.Generic;

namespace Fap.Core.Rbac.AC
{
    public interface IDictSet:IEnumerable<FapDict>
    {
        void Refresh();
        bool TryGetValue(string fid, out FapDict fapDict);
        bool TryGetValueByCategory(string category, out IEnumerable<FapDict> fapDicts);
        bool TryGetValueByCodeAndCategory(string code, string category, out FapDict fapDict);
    }
}
