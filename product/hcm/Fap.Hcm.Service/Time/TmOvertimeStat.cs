using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Hcm.Service.Time
{

	/// <summary>
	/// 加班统计
	/// </summary>
	public class TmOvertimeStat : Fap.Core.Infrastructure.Metadata.BaseModel
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
		/// 关联加班单
		/// </summary>
		public string BillUid { get; set; }
		/// <summary>
		/// 关联加班单 的显性字段MC
		/// </summary>
		[Computed]
		public string BillUidMC { get; set; }
		/// <summary>
		/// 工作日期
		/// </summary>
		public string WorkDate { get; set; }
		/// <summary>
		/// 加班类别
		/// </summary>
		public string OvertimeType { get; set; }
		/// <summary>
		/// 加班类别 的显性字段MC
		/// </summary>
		[Computed]
		public string OvertimeTypeMC { get; set; }
		/// <summary>
		/// 时长
		/// </summary>
		public double HoursLength { get; set; }
		/// <summary>
		/// 失效
		/// </summary>
		public int Invalid { get; set; }

	}
}
