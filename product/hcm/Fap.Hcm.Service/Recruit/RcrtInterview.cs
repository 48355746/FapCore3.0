using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Hcm.Service.Recruit
{

	/// <summary>
	/// 面试记录
	/// </summary>
	public class RcrtInterview : Fap.Core.Infrastructure.Metadata.BaseModel
	{
		/// <summary>
		/// 简历
		/// </summary>
		public string ResumeUid { get; set; }
		/// <summary>
		/// 简历 的显性字段MC
		/// </summary>
		[Computed]
		public string ResumeUidMC { get; set; }
		/// <summary>
		/// 应聘人
		/// </summary>
		public string Contenders { get; set; }
		/// <summary>
		/// 部门
		/// </summary>
		public string DeptUid { get; set; }
		/// <summary>
		/// 部门 的显性字段MC
		/// </summary>
		[Computed]
		public string DeptUidMC { get; set; }
		/// <summary>
		/// 面试官
		/// </summary>
		public string EmpUid { get; set; }
		/// <summary>
		/// 面试官 的显性字段MC
		/// </summary>
		[Computed]
		public string EmpUidMC { get; set; }
		/// <summary>
		/// 轮次
		/// </summary>
		public string Rounds { get; set; }
		/// <summary>
		/// 轮次 的显性字段MC
		/// </summary>
		[Computed]
		public string RoundsMC { get; set; }
		/// <summary>
		/// 面试地点
		/// </summary>
		public string Locations { get; set; }
		/// <summary>
		/// 面试地点 的显性字段MC
		/// </summary>
		[Computed]
		public string LocationsMC { get; set; }
		/// <summary>
		/// 面试时间
		/// </summary>
		public string IvTime { get; set; }
		/// <summary>
		/// 面试结果
		/// </summary>
		public string IvResult { get; set; }
		/// <summary>
		/// 面试结果 的显性字段MC
		/// </summary>
		[Computed]
		public string IvResultMC { get; set; }
		/// <summary>
		/// 能力评分
		/// </summary>
		public string Ability { get; set; }
		/// <summary>
		/// 面试评价
		/// </summary>
		public string IvReview { get; set; }
		/// <summary>
		/// 面试状态
		/// </summary>
		public int IvStatus { get; set; }

	}
}
