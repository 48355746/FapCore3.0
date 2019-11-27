using System;
using Dapper.Contrib.Extensions;
using Fap.Core.Infrastructure.Metadata;

namespace Fap.Core.Rbac.Model
{
    /// <summary>
    /// 角色用户
    /// </summary>
    [Serializable]
    public class FapRoleUser : BaseModel
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        public string RoleUid { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserUid { get; set; }
        /// <summary>
        /// 用户ID 的显性字段MC
        /// </summary>
        [ComputedAttribute]
        public string UserUidMC { get; set; }

    }
}
