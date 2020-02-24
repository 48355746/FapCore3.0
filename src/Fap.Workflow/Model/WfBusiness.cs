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
	/// 流程业务表
	/// </summary>
	public class WfBusiness :BaseModel
	{
		/// <summary>
		/// 单据实体
		/// </summary>
		public string BillEntity { get; set; }
		/// <summary>
		/// 单据实体 的显性字段MC
		/// </summary>
		[Computed]
		public string BillEntityMC { get; set; }
		/// <summary>
		/// 业务编码
		/// </summary>
		public string BizCode { get; set; }
		/// <summary>
		/// 表单类型
		/// </summary>
		public string BizFormType { get; set; }
		/// <summary>
		/// 表单类型 的显性字段MC
		/// </summary>
		[Computed]
		public string BizFormTypeMC { get; set; }
		/// <summary>
		/// 业务表单URL
		/// </summary>
		public string BizFormURL { get; set; }
		/// <summary>
		/// 业务名称
		/// </summary>
		public string BizName { get; set; }
		/// <summary>
		/// 状态
		/// </summary>
		public int BizStatus { get; set; }
		/// <summary>
		/// 业务分类
		/// </summary>
		public string BizTypeUid { get; set; }
		/// <summary>
		/// 业务分类 的显性字段MC
		/// </summary>
		[Computed]
		public string BizTypeUidMC { get; set; }
		/// <summary>
		/// 员工类别
		/// </summary>
		public string EmpCategory { get; set; }
		/// <summary>
		/// 员工类别 的显性字段MC
		/// </summary>
		[Computed]
		public string EmpCategoryMC { get; set; }
		/// <summary>
		/// 主业务实体
		/// </summary>
		public string MainEntity { get; set; }
		/// <summary>
		/// 主业务实体 的显性字段MC
		/// </summary>
		[Computed]
		public string MainEntityMC { get; set; }
		/// <summary>
		/// 所属模块
		/// </summary>
		public string MouduleUid { get; set; }
		/// <summary>
		/// 所属模块 的显性字段MC
		/// </summary>
		[Computed]
		public string MouduleUidMC { get; set; }
		/// <summary>
		/// 关联流程
		/// </summary>
		public string WfProcessUid { get; set; }
		/// <summary>
		/// 关联流程 的显性字段MC
		/// </summary>
		[Computed]
		public string WfProcessUidMC { get; set; }

	}
}
