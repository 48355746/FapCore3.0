using Fap.Core.Rbac.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Rbac.AC
{
    public interface IRoleUserSet:IEnumerable<FapRoleUser>
    {
        void Refresh();
        bool TryGetUserValue(string roleUid, out IEnumerable<string> userUids);
        bool TryGetRoleValue(string userUid, out IEnumerable<string> roleUids);
    }
}
