using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Hcm.Service.Time
{

	/// <summary>
	/// 年假规则
	/// </summary>
	public class TmAnnualLeaveRule : Fap.Core.Infrastructure.Metadata.BaseModel
	{
		/// <summary>
		/// 天数
		/// </summary>
		public int Days { get; set; }
		/// <summary>
		/// 员工条件描述
		/// </summary>
		public string EmpConditionDesc { get; set; }
		/// <summary>
		/// 员工条件
		/// </summary>
		public string EmpCondition { get; set; }

	}
}
