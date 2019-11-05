using Dapper.Contrib.Extensions;
using Fap.Core.MetaData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fap.Core.Rbac.Model
{
    /// <summary>
    /// 角色表
    /// </summary>
    [Serializable]
    public class FapRole : BaseModel
    {
        /// <summary>
        /// 角色编码
        /// </summary>
        public string RoleCode { get; set; }
        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; }
        /// <summary>
        /// 角色备注
        /// </summary>
        public string RoleNote { get; set; }
        /// <summary>
        /// 所属角色组
        /// </summary>
        public string RoleGroupUid { get; set; }
        /// <summary>
        /// 所属角色组 的显性字段MC
        /// </summary>
        [ComputedAttribute]
        public string RoleGroupUidMC { get; set; }
        /// <summary>
        /// 角色类型
        /// </summary>
        public string RoleType { get; set; }
        /// <summary>
        /// 角色类型 的显性字段MC
        /// </summary>
        [ComputedAttribute]
        public string RoleTypeMC { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }

    }

}
