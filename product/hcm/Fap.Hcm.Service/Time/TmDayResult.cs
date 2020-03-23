using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Hcm.Service.Time
{

	/// <summary>
	/// 日结果
	/// </summary>
	public class TmDayResult : Fap.Core.Infrastructure.Metadata.BaseModel
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
		/// 班次
		/// </summary>
		public string ShiftUid { get; set; }
		/// <summary>
		/// 班次 的显性字段MC
		/// </summary>
		[Computed]
		public string ShiftUidMC { get; set; }
		/// <summary>
		/// 日期
		/// </summary>
		public string CurrDate { get; set; }
		/// <summary>
		/// 早打卡时间
		/// </summary>
		public string CardStartTime { get; set; }
		/// <summary>
		/// 晚打卡时间
		/// </summary>
		public string CardEndTime { get; set; }
		/// <summary>
		/// 请假类型
		/// </summary>
		public string LeavelType { get; set; }
		/// <summary>
		/// 请假类型 的显性字段MC
		/// </summary>
		[Computed]
		public string LeavelTypeMC { get; set; }
		/// <summary>
		/// 出差类型
		/// </summary>
		public string TravelType { get; set; }
		/// <summary>
		/// 出差类型 的显性字段MC
		/// </summary>
		[Computed]
		public string TravelTypeMC { get; set; }
		/// <summary>
		/// 日结果
		/// </summary>
		public string CalResult { get; set; }
		/// <summary>
		/// 上班时间
		/// </summary>
		public string StartTime { get; set; }
		/// <summary>
		/// 下班时间
		/// </summary>
		public string EndTime { get; set; }
		/// <summary>
		/// 迟到时间
		/// </summary>
		public string LateTime { get; set; }
		/// <summary>
		/// 早退时间
		/// </summary>
		public string LeaveTime { get; set; }
		/// <summary>
		/// 考勤开始时间
		/// </summary>
		public string StartCardTime { get; set; }
		/// <summary>
		/// 考勤结束时间
		/// </summary>
		public string EndCardTime { get; set; }
		/// <summary>
		/// 排班工作时长
		/// </summary>
		public double WorkHoursLength { get; set; }
		/// <summary>
		/// 排班休息时长
		/// </summary>
		public double RestMinutesLength { get; set; }
		/// <summary>
		/// 实际工作时长
		/// </summary>
		public double StWorkHourLength { get; set; }
		/// <summary>
		/// 请假时长
		/// </summary>
		public double LeavelHours { get; set; }
		/// <summary>
		/// 出差时长
		/// </summary>
		public double TravelHours { get; set; }
		/// <summary>
		/// 休息开始时间
		/// </summary>
		public string RestStartTime { get; set; }
		/// <summary>
		/// 休息结束时间
		/// </summary>
		public string RestEndTime { get; set; }
		/// <summary>
		/// 请假天数
		/// </summary>
		public double LeaveDays { get; set; }
		/// <summary>
		/// 出差天数
		/// </summary>
		public double TravelDays { get; set; }

	}
}
