using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Hcm.Service.Time
{

	/// <summary>
	/// 考勤期间
	/// </summary>
	public class TmPeriod : Fap.Core.Infrastructure.Metadata.BaseModel
	{
		/// <summary>
		/// 月份
		/// </summary>
		public string CurrMonth { get; set; }
		/// <summary>
		/// 开始日期
		/// </summary>
		public string StartDate { get; set; }
		/// <summary>
		/// 结束日期
		/// </summary>
		public string EndDate { get; set; }
		/// <summary>
		/// 当前期间
		/// </summary>
		public int IsPeriod { get; set; }

	}
}
