using Fap.Core.Rbac.Model;
/* ==============================================================================
 * 功能描述：  
 * 创 建 者：wyf
 * 创建日期：2016/7/13 15:26:11
 * ==============================================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fap.Core.Rbac.AC
{
    /// <summary>
    /// 角色字段权限
    /// </summary>
    public interface IRoleColumnSet : IEnumerable<FapRoleColumn>
    {
        void Refresh();
        bool TryGetValue(string fid, out FapRoleColumn roleColumn);

        bool TryGetValueByRole(string roleUid, out IEnumerable<FapRoleColumn> roleColumns);
    }
}
