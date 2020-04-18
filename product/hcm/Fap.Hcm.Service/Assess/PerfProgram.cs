using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Hcm.Service.Assess
{

	/// <summary>
	/// 考核方案
	/// </summary>
	public class PerfProgram : Fap.Core.Infrastructure.Metadata.BaseModel
	{
		/// <summary>
		/// 编码
		/// </summary>
		public string PrmCode { get; set; }
		/// <summary>
		/// 名称
		/// </summary>
		public string PrmName { get; set; }
		/// <summary>
		/// 考核年月
		/// </summary>
		public string PrmYm { get; set; }
		/// <summary>
		/// 考核对象
		/// </summary>
		public string ObjectType { get; set; }
		/// <summary>
		/// 考核对象 的显性字段MC
		/// </summary>
		[Computed]
		public string ObjectTypeMC { get; set; }
		/// <summary>
		/// 方案分类
		/// </summary>
		public string PrmCategory { get; set; }
		/// <summary>
		/// 方案分类 的显性字段MC
		/// </summary>
		[Computed]
		public string PrmCategoryMC { get; set; }
		/// <summary>
		/// 允许自评
		/// </summary>
		public int AllowSelfScore { get; set; }
		/// <summary>
		/// 备注
		/// </summary>
		public string PrmNote { get; set; }
		/// <summary>
		/// 最高分
		/// </summary>
		public double MaxScore { get; set; }
		/// <summary>
		/// 最低分
		/// </summary>
		public double MinScore { get; set; }
		/// <summary>
		/// 是否360度考核
		/// </summary>
		public int Is360 { get; set; }
		/// <summary>
		/// 自助查看
		/// </summary>
		public int EssViewAble { get; set; }
		/// <summary>
		/// 状态
		/// </summary>
		public string PrmStatus { get; set; }
		/// <summary>
		/// 状态 的显性字段MC
		/// </summary>
		[Computed]
		public string PrmStatusMC { get; set; }

	}
}
