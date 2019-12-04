using Dapper.Contrib.Extensions;
using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Infrastructure.Config
{
    /// <summary>
    /// 邮件模板
    /// </summary>
    [Table("CfgEmailTemplate")]
    public class CfgEmailTemplate : BaseModel
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 模块
        /// </summary>
        public string ModuleUid { get; set; }
        /// <summary>
        /// 模块 的显性字段MC
        /// </summary>
        [ComputedAttribute]
        public string ModuleUidMC { get; set; }
        /// <summary>
        /// 实体
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 实体 的显性字段MC
        /// </summary>
        [ComputedAttribute]
        public string TableNameMC { get; set; }
        /// <summary>
        /// 模板内容
        /// </summary>
        public string TemplateContent { get; set; }
        /// <summary>
        /// 启用
        /// </summary>
        public int Enabled { get; set; }

    }
}
