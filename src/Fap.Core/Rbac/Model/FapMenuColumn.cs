using Dapper.Contrib.Extensions;
using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Rbac.Model
{
	/// <summary>
	/// 菜单列
	/// </summary>
	public class FapMenuColumn : BaseModel
	{
		/// <summary>
		/// 菜单
		/// </summary>
		public string MenuUid { get; set; }
		/// <summary>
		/// 菜单 的显性字段MC
		/// </summary>
		[Computed]
		public string MenuUidMC { get; set; }
		/// <summary>
		/// 描述
		/// </summary>
		public string Description { get; set; }
		/// <summary>
		/// 表格
		/// </summary>
		public string GridId { get; set; }
		/// <summary>
		/// 表名
		/// </summary>
		public string TableName { get; set; }
		/// <summary>
		/// 表格列
		/// </summary>
		public string GridColumn { get; set; }
		/// <summary>
		/// 启用
		/// </summary>
		public int Enabled { get; set; }

	}

}
