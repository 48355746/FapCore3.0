using Fap.Core.Rbac.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Rbac.AC
{
    public interface IMenuColumnSet : IEnumerable<FapMenuColumn>
    {
        void Refresh();
        bool TryGetValue(string menuUid, out IEnumerable<FapMenuColumn> fapColumnList);
        bool NotRegistryAuthority(string menuUid, string gridId);
    }
}
