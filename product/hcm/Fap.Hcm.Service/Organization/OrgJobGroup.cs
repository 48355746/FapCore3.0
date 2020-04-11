using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Hcm.Service.Organization
{
	/// <summary>
	/// 职位族群
	/// </summary>
	public class OrgJobGroup : Fap.Core.Infrastructure.Metadata.BaseModel
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
		/// 描述
		/// </summary>
		public string Description { get; set; }

	}
}
