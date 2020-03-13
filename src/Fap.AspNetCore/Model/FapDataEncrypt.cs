using Dapper.Contrib.Extensions;
using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.AspNetCore.Model
{
    /// <summary>
    /// 数据加密
    /// </summary>
    public class FapDataEncrypt : BaseModel
    {
		/// <summary>
		/// 实体
		/// </summary>
		public string RefTable { get; set; }
		/// <summary>
		/// 实体 的显性字段MC
		/// </summary>
		[Computed]
		public string RefTableMC { get; set; }
		/// <summary>
		/// 属性
		/// </summary>
		public string ColumnName { get; set; }
		/// <summary>
		/// 属性 的显性字段MC
		/// </summary>
		[Computed]
		public string ColumnNameMC { get; set; }
		/// <summary>
		/// 条件
		/// </summary>
		public string Condition { get; set; }
		/// <summary>
		/// 备注
		/// </summary>
		public string Remarks { get; set; }
		/// <summary>
		/// 替换字符
		/// </summary>
		public string ReplaceChart { get; set; }

	}
}
