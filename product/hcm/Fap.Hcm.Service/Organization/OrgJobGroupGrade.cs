using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Hcm.Service.Organization
{

	/// <summary>
	/// 职族等级
	/// </summary>
	public class OrgJobGroupGrade : Fap.Core.Infrastructure.Metadata.BaseModel
	{
		/// <summary>
		/// 职位族群
		/// </summary>
		public string JobGroup { get; set; }
		/// <summary>
		/// 职位族群 的显性字段MC
		/// </summary>
		[Computed]
		public string JobGroupMC { get; set; }
		/// <summary>
		/// 名称
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// 职等
		/// </summary>
		public string JobGrade { get; set; }
		/// <summary>
		/// 描述
		/// </summary>
		public string Description { get; set; }

	}
}
