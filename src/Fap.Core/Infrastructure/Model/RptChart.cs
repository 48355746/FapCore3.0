using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Infrastructure.Model
{

	/// <summary>
	/// 图表
	/// </summary>
	public class RptChart : Fap.Core.Infrastructure.Metadata.BaseModel
	{
		/// <summary>
		/// 标题
		/// </summary>
		public string Subject { get; set; }
		/// <summary>
		/// 图表模型
		/// </summary>
		public string ChartModel { get; set; }
		/// <summary>
		/// 实体模型
		/// </summary>
		public string EntityModel { get; set; }
		/// <summary>
		/// 实体自定义 条件
		/// </summary>
		public string EntityCondition { get; set; }
		/// <summary>
		/// 个人
		/// </summary>
		public int Personal { get; set; }
		/// <summary>
		/// 总裁
		/// </summary>
		public int CEO { get; set; }
		/// <summary>
		/// 部门
		/// </summary>
		public int Department { get; set; }

	}
}
