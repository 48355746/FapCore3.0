using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Hcm.Service.Time
{

	/// <summary>
	/// 出差统计
	/// </summary>
	public class TmTravelStat : Fap.Core.Infrastructure.Metadata.BaseModel
	{
		/// <summary>
		/// 关联单据
		/// </summary>
		public string BillUid { get; set; }
		/// <summary>
		/// 关联单据 的显性字段MC
		/// </summary>
		[Computed]
		public string BillUidMC { get; set; }
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
		/// 提交时间
		/// </summary>
		public string WorkDate { get; set; }
		/// <summary>
		/// 出差类型
		/// </summary>
		public string TravelTypeUid { get; set; }
		/// <summary>
		/// 出差类型 的显性字段MC
		/// </summary>
		[Computed]
		public string TravelTypeUidMC { get; set; }
		/// <summary>
		/// 出差天数
		/// </summary>
		public double TravelDays { get; set; }
		/// <summary>
		/// 时长
		/// </summary>
		public double TravelHours { get; set; }

	}
}
