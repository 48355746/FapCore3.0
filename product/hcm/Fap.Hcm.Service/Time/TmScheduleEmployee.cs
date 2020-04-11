using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Hcm.Service.Time
{

	/// <summary>
	/// 排班人员
	/// </summary>
	public class TmScheduleEmployee : Core.Infrastructure.Metadata.BaseModel
	{
		/// <summary>
		/// 排班UID
		/// </summary>
		public string ScheduleUid { get; set; }
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
		/// 部门
		/// </summary>
		public string DeptUid { get; set; }
		/// <summary>
		/// 部门 的显性字段MC
		/// </summary>
		[Computed]
		public string DeptUidMC { get; set; }
		/// <summary>
		/// 开始时间
		/// </summary>
		public string StartDate { get; set; }
		/// <summary>
		/// 结束日期
		/// </summary>
		public string EndDate { get; set; }

	}
}
