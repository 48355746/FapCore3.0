using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Hcm.Service.Assess
{
	/// <summary>
	/// 考核对象
	/// </summary>
	public class PerfObject : Fap.Core.Infrastructure.Metadata.BaseModel
	{
		/// <summary>
		/// 对象Uid
		/// </summary>
		public string ObjUid { get; set; }
		/// <summary>
		/// 对象编码
		/// </summary>
		public string ObjCode { get; set; }
		/// <summary>
		/// 对象名称
		/// </summary>
		public string ObjName { get; set; }
		/// <summary>
		/// 考核方案
		/// </summary>
		public string ProgramUid { get; set; }
		/// <summary>
		/// 考核方案 的显性字段MC
		/// </summary>
		[Computed]
		public string ProgramUidMC { get; set; }
		/// <summary>
		/// 完成情况
		/// </summary>
		public string Completeness { get; set; }
		/// <summary>
		/// 最终得分
		/// </summary>
		public double Score { get; set; }

	}
}
