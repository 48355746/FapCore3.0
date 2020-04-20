using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Hcm.Service.Assess
{
	/// <summary>
	/// 考核打分
	/// </summary>
	public class PerfScore : Fap.Core.Infrastructure.Metadata.BaseModel
	{
		/// <summary>
		/// 考核人
		/// </summary>
		public string ExaminerUid { get; set; }
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
		/// 打分人
		/// </summary>
		public string EmpUid { get; set; }
		/// <summary>
		/// 打分人 的显性字段MC
		/// </summary>
		[Computed]
		public string EmpUidMC { get; set; }
		/// <summary>
		/// 考核指标
		/// </summary>
		public string KPIUid { get; set; }
		/// <summary>
		/// 考核指标 的显性字段MC
		/// </summary>
		[Computed]
		public string KPIUidMC { get; set; }
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
		/// 评分等级
		/// </summary>
		public string Rating { get; set; }
		/// <summary>
		/// 得分
		/// </summary>
		public double Score { get; set; }
		/// <summary>
		/// 评分说明
		/// </summary>
		public string ScoreNote { get; set; }
		/// <summary>
		/// 权重
		/// </summary>
		public double Weights { get; set; }
		/// <summary>
		/// 最终分数
		/// </summary>
		public double ScoreResult { get; set; }
		/// <summary>
		/// 完成情况
		/// </summary>
		public string Completeness { get; set; }

	}
}
