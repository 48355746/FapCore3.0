using Dapper.Contrib.Extensions;
using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Hcm.Service.Organization
{
    /// <summary>
    /// 职位
    /// </summary>
    public class OrgPosition : BaseModel
    {
		/// <summary>
		/// 编码
		/// </summary>
		public string PstCode { get; set; }
		/// <summary>
		/// 名称
		/// </summary>
		public string PstName { get; set; }
		/// <summary>
		/// 职务层级
		/// </summary>
		public string JobUid { get; set; }
		/// <summary>
		/// 职务层级 的显性字段MC
		/// </summary>
		[ComputedAttribute]
		public string JobUidMC { get; set; }
		/// <summary>
		/// 职位层级全称
		/// </summary>
		public string JobFullName { get; set; }
		/// <summary>
		/// 职位等级范围
		/// </summary>
		public string GradeRange { get; set; }
		/// <summary>
		/// 职位描述
		/// </summary>
		public string PstNote { get; set; }
		/// <summary>
		/// 附件
		/// </summary>
		public string Attachment { get; set; }
		/// <summary>
		/// 说明书
		/// </summary>
		public string Instructions { get; set; }


	}
}
