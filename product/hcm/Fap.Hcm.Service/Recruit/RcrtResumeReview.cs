using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Hcm.Service.Recruit
{

	/// <summary>
	/// 简历评价
	/// </summary>
	public class RcrtResumeReview : Fap.Core.Infrastructure.Metadata.BaseModel
	{
		/// <summary>
		/// 评价人
		/// </summary>
		public string EmpUid { get; set; }
		/// <summary>
		/// 评价人 的显性字段MC
		/// </summary>
		[Computed]
		public string EmpUidMC { get; set; }
		
		/// <summary>
		/// 评价
		/// </summary>
		public string Review { get; set; }
		/// <summary>
		/// 面试建议
		/// </summary>
		public string InterviewAdvice { get; set; }
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
		/// 推送人
		/// </summary>
		public string SEmpUid { get; set; }
		/// <summary>
		/// 推送人 的显性字段MC
		/// </summary>
		[Computed]
		public string SEmpUidMC { get; set; }

	}
}
