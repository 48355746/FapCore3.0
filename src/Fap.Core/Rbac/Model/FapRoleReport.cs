/* ==============================================================================
 * 功能描述：角色报表  
 * 创 建 者：wyf
 * 创建日期：2016/9/19 19:35:30
 * ==============================================================================*/
using Fap.Core.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fap.Core.Rbac.Model
{
    /// <summary>
    /// 角色报表
    /// </summary>
    [Serializable]
    public class FapRoleReport : BaseModel
    {
        /// <summary>
        /// 角色
        /// </summary>
        public string RoleUid { get; set; }
        /// <summary>
        /// 报表
        /// </summary>
        public string RptUid { get; set; }

    }

}
