using Dapper.Contrib.Extensions;
using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.AspNetCore.Model
{
    /// <summary>
    /// 表单注入
    /// </summary>
    public class FapFormInjection : BaseModel
    {
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
        /// 变化列
        /// </summary>
        public string ChangeColumn { get; set; }
        /// <summary>
        /// 变化列 的显性字段MC
        /// </summary>
        [ComputedAttribute]
        public string ChangeColumnMC { get; set; }
        /// <summary>
        /// 参数列
        /// </summary>
        public string ParamColumns { get; set; }
        /// <summary>
        /// 执行Sql
        /// </summary>
        public string ExecSql { get; set; }
        /// <summary>
        /// 生效
        /// </summary>
        public int IsEnabled { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int SortBy { get; set; }

    }

}
