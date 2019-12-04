using Dapper.Contrib.Extensions;
using Fap.Core.Infrastructure.Metadata;
using System;

namespace Fap.Core.Rbac.Model
{

    /// <summary>
    /// 动态角色
    /// </summary>
    public class FapBizDynRole : BaseModel
    {
        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; }
        /// <summary>
        /// 自定义脚本
        /// </summary>
        public string CustomSql { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int State { get; set; }
        /// <summary>
        /// 占位符绑定方式
        /// </summary>
        public string BindType { get; set; }
        /// <summary>
        /// 占位符绑定方式 的显性字段MC
        /// </summary>
        [ComputedAttribute]
        public string BindTypeMC { get; set; }


    }
}
