using Fap.Core.Rbac.Model;
/* ==============================================================================
 * 功能描述：全局角色菜单  
 * 创 建 者：wyf
 * 创建日期：2016/7/13 16:35:43
 * ==============================================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fap.Core.Rbac.AC
{
    public interface IRoleMenuSet:IEnumerable<FapRoleMenu>
    {
        void Refresh();
        bool TryGetValue(string fid, out FapRoleMenu roleMenu);

        bool TryGetValueByRole(string roleUid, out IEnumerable<FapRoleMenu> roleMenus);
    }
}
