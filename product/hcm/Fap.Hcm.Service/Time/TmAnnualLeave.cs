using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Hcm.Service.Time
{

	/// <summary>
	/// 年假
	/// </summary>
	public class TmAnnualLeave : Fap.Core.Infrastructure.Metadata.BaseModel
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
		/// 年度
		/// </summary>
		public string Annual { get; set; }
		/// <summary>
		/// 开始日期
		/// </summary>
		public string StartDate { get; set; }
		/// <summary>
		/// 结束日期
		/// </summary>
		public string EndDate { get; set; }
		/// <summary>
		/// 本年分配天数
		/// </summary>
		public double CurrYearNum { get; set; }
		/// <summary>
		/// 上年结余天数
		/// </summary>
		public double LastYearLeft { get; set; }
		/// <summary>
		/// 本年实际天数
		/// </summary>
		public double CurrRealNum { get; set; }
		/// <summary>
		/// 已休天数
		/// </summary>
		public double UsedNum { get; set; }
		/// <summary>
		/// 剩余天数
		/// </summary>
		public double RemainderNum { get; set; }
		/// <summary>
		/// 已处理结余
		/// </summary>
		public int IsHandle { get; set; }

	}
}
