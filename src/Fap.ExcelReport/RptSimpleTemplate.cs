using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.ExcelReport
{

	/// <summary>
	/// 简单报表模板
	/// </summary>
	public class RptSimpleTemplate : Fap.Core.Infrastructure.Metadata.BaseModel
	{
		/// <summary>
		/// 父报表
		/// </summary>
		public string Pid { get; set; }
		/// <summary>
		/// 父报表 的显性字段MC
		/// </summary>
		[Computed]
		public string PidMC { get; set; }
		/// <summary>
		/// 报表名称
		/// </summary>
		public string ReportName { get; set; }
		/// <summary>
		/// Excel文件
		/// </summary>
		public string XlsFile { get; set; }
		/// <summary>
		/// 是否目录
		/// </summary>
		public int IsDir { get; set; }
		/// <summary>
		/// 层级
		/// </summary>
		public int TreeLevel { get; set; }
		/// <summary>
		/// 是否末级
		/// </summary>
		public int IsFinal { get; set; }

	}
}
