using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Hcm.Service.Time
{

	/// <summary>
	/// 请假统计
	/// </summary>
	public class TmLeaveStat : Fap.Core.Infrastructure.Metadata.BaseModel
	{
		/// <summary>
		/// 关联请假单
		/// </summary>
		public string BillUid { get; set; }
		/// <summary>
		/// 关联请假单 的显性字段MC
		/// </summary>
		[Computed]
		public string BillUidMC { get; set; }
		/// <summary>
		/// 员工
		/// </summary>
		public string EmpUid { get; set; }
		/// <summary>
		/// 员工 的显性字段MC
		/// </summary>
		[Computed]
		public string EmpUidMC { get; set; }
		/// <summary>
		/// 工作日期
		/// </summary>
		public string WorkDate { get; set; }
		/// <summary>
		/// 请假类型
		/// </summary>
		public string LeaveTypeUid { get; set; }
		/// <summary>
		/// 请假类型 的显性字段MC
		/// </summary>
		[Computed]
		public string LeaveTypeUidMC { get; set; }
		/// <summary>
		/// 请假天数
		/// </summary>
		public double LeaveDays { get; set; }
		/// <summary>
		/// 请假时长
		/// </summary>
		public double LeaveHours { get; set; }

	}
}
