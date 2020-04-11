using Fap.Core.Rbac.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Rbac.AC
{
    public interface IRoleButtonSet:IEnumerable<FapRoleButton>
    {
        void Refresh();
        bool TryGetValue(string roleUid, out IEnumerable<FapRoleButton> roleButtons);        
    }
}
