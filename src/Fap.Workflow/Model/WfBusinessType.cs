using Dapper.Contrib.Extensions;
using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fap.Workflow.Model
{
	/// <summary>
	/// 业务分类
	/// </summary>
	public class WfBusinessType : BaseModel
	{
		/// <summary>
		/// 父分类
		/// </summary>
		public string Pid { get; set; }
		/// <summary>
		/// 父分类 的显性字段MC
		/// </summary>
		[Computed]
		public string PidMC { get; set; }
		/// <summary>
		/// 分类编号
		/// </summary>
		public string TypeCode { get; set; }
		/// <summary>
		/// 分类名
		/// </summary>
		public string TypeName { get; set; }

	}
}
