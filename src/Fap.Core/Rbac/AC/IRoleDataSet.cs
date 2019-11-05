using Fap.Core.Rbac.Model;
/* ==============================================================================
 * 功能描述：角色数据集  
 * 创 建 者：wyf
 * 创建日期：2016/11/30 19:02:01
 * ==============================================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fap.Core.Rbac.AC
{
    public interface IRoleDataSet : IEnumerable<FapRoleData>
    {
        void Refresh();
        bool TryGetValue(string fid, out FapRoleData roleData);

        bool TryGetValueByRole(string roleUid, out IEnumerable<FapRoleData> roleDatas);
    }
}
