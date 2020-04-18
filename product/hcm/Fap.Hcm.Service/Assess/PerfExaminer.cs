using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Hcm.Service.Assess
{

	/// <summary>
	/// 考核人
	/// </summary>
	public class PerfExaminer : Fap.Core.Infrastructure.Metadata.BaseModel
	{
		/// <summary>
		/// 考核对象
		/// </summary>
		public string ObjectUid { get; set; }
		/// <summary>
		/// 考核对象 的显性字段MC
		/// </summary>
		[Computed]
		public string ObjectUidMC { get; set; }
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
		/// 考核方式
		/// </summary>
		public string AssessModel { get; set; }
		/// <summary>
		/// 考核人
		/// </summary>
		public string EmpUid { get; set; }
		/// <summary>
		/// 考核人 的显性字段MC
		/// </summary>
		[Computed]
		public string EmpUidMC { get; set; }
		/// <summary>
		/// 权重
		/// </summary>
		public double Weights { get; set; }
		/// <summary>
		/// 分数
		/// </summary>
		public double Score { get; set; }
		/// <summary>
		/// 评价
		/// </summary>
		public string Reviews { get; set; }

	}
}
