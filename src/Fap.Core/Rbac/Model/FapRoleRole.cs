/* ==============================================================================
 * 功能描述：角色角色  
 * 创 建 者：wyf
 * 创建日期：2017-03-24 15:01:40
 * ==============================================================================*/
using Fap.Core.MetaData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fap.Core.Rbac.Model
{
    /// <summary>
    /// 角色角色
    /// </summary>
    [Serializable]
    public class FapRoleRole : BaseModel
    {
        /// <summary>
        /// 角色
        /// </summary>
        public string RoleUid { get; set; }
        /// <summary>
        /// 权限角色
        /// </summary>
        public string PRoleUid { get; set; }

    }

}
