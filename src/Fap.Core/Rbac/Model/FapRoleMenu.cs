using System;
using Dapper.Contrib.Extensions;
using Fap.Core.Metadata;

namespace Fap.Core.Rbac.Model
{
    /// <summary>
    /// 角色菜单表
    /// </summary>
    [Serializable]
    public class FapRoleMenu : BaseModel
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
        /// 菜单
        /// </summary>
        public string MenuUid { get; set; }
        /// <summary>
        /// 菜单 的显性字段MC
        /// </summary>
        [ComputedAttribute]
        public string MenuUidMC { get; set; }

    }
}
