using Dapper.Contrib.Extensions;
using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Hcm.Service.Organization
{
	/// <summary>
	/// 职位层级
	/// </summary>
	public class OrgJob:BaseModel
    {
		/// <summary>
		/// 编码
		/// </summary>
		public string JobCode { get; set; }
		/// <summary>
		/// 名称
		/// </summary>
		public string JobName { get; set; }
		/// <summary>
		/// 备注
		/// </summary>
		public string JobNote { get; set; }
		/// <summary>
		/// 父级职位类
		/// </summary>
		public string Pid { get; set; }
		/// <summary>
		/// 父级职位类 的显性字段MC
		/// </summary>
		[Computed]
		public string PidMC { get; set; }
		/// <summary>
		/// 是否末级
		/// </summary>
		public int IsFinal { get; set; }
		/// <summary>
		/// 排序
		/// </summary>
		public int JobOrder { get; set; }
		/// <summary>
		/// 层级
		/// </summary>
		public int TreeLevel { get; set; }
		/// <summary>
		/// 全称
		/// </summary>
		public string FullName { get; set; }

	}
}
