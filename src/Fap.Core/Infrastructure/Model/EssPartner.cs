using Dapper.Contrib.Extensions;
using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Infrastructure.Model
{
	/// <summary>
	/// 业务伙伴
	/// </summary>
	public class EssPartner :BaseModel
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
		/// 伙伴
		/// </summary>
		public string PartnerUid { get; set; }
		/// <summary>
		/// 伙伴 的显性字段MC
		/// </summary>
		[Computed]
		public string PartnerUidMC { get; set; }
		/// <summary>
		/// 同意
		/// </summary>
		public int Agree { get; set; }
	}
}
