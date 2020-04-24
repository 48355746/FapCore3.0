using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Hcm.Service.Organization
{

	/// <summary>
	/// 职衔
	/// </summary>
	public class OrgJobTitle : Fap.Core.Infrastructure.Metadata.BaseModel
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
		/// 职族
		/// </summary>
		public string JobGroup { get; set; }
		/// <summary>
		/// 职族 的显性字段MC
		/// </summary>
		[Computed]
		public string JobGroupMC { get; set; }
		/// <summary>
		/// 职族等级
		/// </summary>
		public string JobGroupGrade { get; set; }
		/// <summary>
		/// 职族等级 的显性字段MC
		/// </summary>
		[Computed]
		public string JobGroupGradeMC { get; set; }
		/// <summary>
		/// 组织分类
		/// </summary>
		public string OrgCategory { get; set; }
		/// <summary>
		/// 组织分类 的显性字段MC
		/// </summary>
		[Computed]
		public string OrgCategoryMC { get; set; }
		/// <summary>
		/// 职等
		/// </summary>
		public int JobGrade { get; set; }
		/// <summary>
		/// 职位说明书
		/// </summary>
		public string Outline { get; set; }
		/// <summary>
		/// 职等范围
		/// </summary>
		public string JobGradeRange { get; set; }

	}
}
