using Dapper.Contrib.Extensions;
using Fap.Core.Infrastructure.Metadata;
/* ==============================================================================
* 功能描述：  
* 创 建 者：wyf
* 创建日期：2016/11/30 18:57:52
* ==============================================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fap.Core.Rbac.Model
{
    /// <summary>
    /// 角色数据
    /// </summary>
    [Serializable]
    public class FapRoleData : BaseModel
    {
        /// <summary>
        /// 角色
        /// </summary>
        public string RoleUid { get; set; }
        /// <summary>
        /// 角色 的显性字段MC
        /// </summary>
        [ComputedAttribute]
        public string RoleUidMC { get; set; }
        /// <summary>
        /// 实体
        /// </summary>
        public string TableUid { get; set; }
        /// <summary>
        /// 实体 的显性字段MC
        /// </summary>
        [ComputedAttribute]
        public string TableUidMC { get; set; }
        /// <summary>
        /// 条件
        /// </summary>
        public string SqlCondition { get; set; }

    }

}
