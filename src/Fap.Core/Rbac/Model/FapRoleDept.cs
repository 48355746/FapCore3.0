using System;
using Dapper.Contrib.Extensions;
using Fap.Core.Infrastructure.Metadata;

namespace Fap.Core.Rbac.Model
{
    /// <summary>
    /// 角色部门
    /// </summary>
    [Serializable]
    public class FapRoleDept : BaseModel
    {
        /// <summary>
        /// 角色
        /// </summary>
        public string RoleUid { get; set; }
        /// <summary>
        /// 部门
        /// </summary>
        public string DeptUid { get; set; }

    }
}
