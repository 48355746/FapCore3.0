using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Infrastructure.Model
{

	/// <summary>
	/// 实体映射
	/// </summary>
	public class CfgEntityMapping : Metadata.BaseModel
	{
		/// <summary>
		/// 源实体
		/// </summary>
		public string OriEntity { get; set; }
		/// <summary>
		/// 源实体 的显性字段MC
		/// </summary>
		[Computed]
		public string OriEntityMC { get; set; }
		/// <summary>
		/// 目标实体
		/// </summary>
		public string AimsEntity { get; set; }
		/// <summary>
		/// 目标实体 的显性字段MC
		/// </summary>
		[Computed]
		public string AimsEntityMC { get; set; }
		/// <summary>
		/// 关联
		/// </summary>
		public string Associate { get; set; }

	}
}
