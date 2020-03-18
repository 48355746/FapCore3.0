using Dapper.Contrib.Extensions;
using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Hcm.Service.Time
{
	/// <summary>
	/// 打卡记录
	/// </summary>
	public class TmCardRecord :  BaseModel
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
		/// 员工编码
		/// </summary>
		public string EmpCode { get; set; }
		/// <summary>
		/// 打卡时间
		/// </summary>
		public string CardTime { get; set; }
		/// <summary>
		/// 设备号
		/// </summary>
		public string DeviceNumber { get; set; }
		/// <summary>
		/// 设备名称
		/// </summary>
		public string DeviceName { get; set; }
		/// <summary>
		/// 设备IP
		/// </summary>
		public string IpAddress { get; set; }

	}

}
