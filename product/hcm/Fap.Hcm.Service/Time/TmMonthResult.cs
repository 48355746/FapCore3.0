using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Hcm.Service.Time
{
	/// <summary>
	/// 月结果
	/// </summary>
	public class TmMonthResult : Fap.Core.Infrastructure.Metadata.BaseModel
	{
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
		/// 考勤周期
		/// </summary>
		public string CurrMonth { get; set; }
		/// <summary>
		/// 缺勤天数
		/// </summary>
		public int AbsenceNum { get; set; }
		/// <summary>
		/// 年假天数
		/// </summary>
		public double AnnualNum { get; set; }
		/// <summary>
		/// 事假天数
		/// </summary>
		public double BizLeaveNum { get; set; }
		/// <summary>
		/// 病假天数
		/// </summary>
		public double SickLeaveNum { get; set; }
		/// <summary>
		/// 婚假天数
		/// </summary>
		public double WedLeaveNum { get; set; }
		/// <summary>
		/// 产假天数
		/// </summary>
		public double MaternityLeaveNum { get; set; }
		/// <summary>
		/// 丧假天数
		/// </summary>
		public double FuneralLeaveNum { get; set; }
		/// <summary>
		/// 迟到天数
		/// </summary>
		public int LateNum { get; set; }
		/// <summary>
		/// 出差天数
		/// </summary>
		public double TravelNum { get; set; }
		/// <summary>
		/// 早退天数
		/// </summary>
		public int LeftEarlyNum { get; set; }
		/// <summary>
		/// 周末加班时长
		/// </summary>
		public double WeekEndOtHour { get; set; }
		/// <summary>
		/// 节假日加班时长
		/// </summary>
		public double HolidayOtHour { get; set; }
		/// <summary>
		/// 工作日天数
		/// </summary>
		public double WorkDayNum { get; set; }
		/// <summary>
		/// 工作日加班时长
		/// </summary>
		public double WorkDayOtHour { get; set; }

	}
}
