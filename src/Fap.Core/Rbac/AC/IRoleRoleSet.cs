using Fap.Core.Rbac.Model;
/* ==============================================================================
 * 功能描述：  
 * 创 建 者：wyf
 * 创建日期：2016/7/13 16:44:11
 * ==============================================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fap.Core.Rbac.AC
{
    public interface IRoleRoleSet : IEnumerable<FapRoleRole>
    {
        void Refresh();
        bool TryGetValue(string fid, out FapRoleRole roleRole);

        bool TryGetValueByRole(string roleUid, out IEnumerable<FapRoleRole> roleRoles);
    }
}
