using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Hcm.Service.Time
{

	/// <summary>
	/// 考勤补签数据
	/// </summary>
	public class TmReplenDay : Fap.Core.Infrastructure.Metadata.BaseModel
	{
		/// <summary>
		/// 人员
		/// </summary>
		public string EmpUid { get; set; }
		/// <summary>
		/// 人员 的显性字段MC
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
		/// 考勤日期
		/// </summary>
		public string WorkDate { get; set; }
		/// <summary>
		/// 日结果
		/// </summary>
		public string DayCalResult { get; set; }
		/// <summary>
		/// 考勤周期
		/// </summary>
		public string TmPeriod { get; set; }
		/// <summary>
		/// 补签状态
		/// </summary>
		public int ReplenStatus { get; set; }
		/// <summary>
		/// 上班时间
		/// </summary>
		public string StartTime { get; set; }
		/// <summary>
		/// 下班时间
		/// </summary>
		public string EndTime { get; set; }
		/// <summary>
		/// 直属上级
		/// </summary>
		public string LeaderShip { get; set; }
		/// <summary>
		/// 直属上级 的显性字段MC
		/// </summary>
		[Computed]
		public string LeaderShipMC { get; set; }
		/// <summary>
		/// 邮箱
		/// </summary>
		public string MailBox { get; set; }

	}
}
