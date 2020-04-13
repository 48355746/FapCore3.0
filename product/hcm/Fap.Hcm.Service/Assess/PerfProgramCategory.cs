using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Hcm.Service.Assess
{
	/// <summary>
	/// 考核方案分类
	/// </summary>
	public class PerfProgramCategory : Fap.Core.Infrastructure.Metadata.BaseModel
	{
		/// <summary>
		/// 编码
		/// </summary>
		public string Code { get; set; }
		/// <summary>
		/// 名称
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// 父级
		/// </summary>
		public string Pid { get; set; }

	}
}
