using System;
using Dapper.Contrib.Extensions;
using Fap.Core.Infrastructure.Metadata;

namespace Fap.Core.Rbac.Model
{
    /// <summary>
    /// 角色实体属性
    /// </summary>
    [Serializable]
    public class FapRoleColumn : BaseModel
    {
        /// <summary>
        /// 角色
        /// </summary>
        public string RoleUid { get; set; }
        /// <summary>
        /// 实体
        /// </summary>
        public string TableUid { get; set; }
        /// <summary>
        /// 属性
        /// </summary>
        public string ColumnUid { get; set; }
        /// <summary>
        /// 可编辑
        /// </summary>
        public int EditAble { get; set; }
        /// <summary>
        /// 可查看
        /// </summary>
        public int ViewAble { get; set; }

    }
}
