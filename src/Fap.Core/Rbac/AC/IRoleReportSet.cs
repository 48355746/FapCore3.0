using Fap.Core.Rbac.Model;
/* ==============================================================================
 * 功能描述：  
 * 创 建 者：wyf
 * 创建日期：2016/9/19 20:01:01
 * ==============================================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fap.Core.Rbac.AC
{
    public interface IRoleReportSet : IEnumerable<FapRoleReport>
    {
        void Refresh();
        bool TryGetValue(string fid, out FapRoleReport roleRpt);

        bool TryGetValueByRole(string roleUid, out IEnumerable<FapRoleReport> roleRpts);
    }
}
