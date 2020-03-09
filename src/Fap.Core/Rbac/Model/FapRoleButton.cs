using Dapper.Contrib.Extensions;
using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Rbac.Model
{
	/// <summary>
	/// 角色按钮
	/// </summary>
	public class FapRoleButton : BaseModel 
	{
		/// <summary>
		/// 角色
		/// </summary>
		public string RoleUid { get; set; }
		/// <summary>
		/// 角色 的显性字段MC
		/// </summary>
		[Computed]
		public string RoleUidMC { get; set; }
		/// <summary>
		/// 菜单
		/// </summary>
		public string MenuUid { get; set; }
		/// <summary>
		/// 菜单 的显性字段MC
		/// </summary>
		[Computed]
		public string MenuUidMC { get; set; }
		/// <summary>
		/// 按钮类型
		/// </summary>
		public string ButtonType { get; set; }
		/// <summary>
		/// 按钮类型 的显性字段MC
		/// </summary>
		[Computed]
		public string ButtonTypeMC { get; set; }
		/// <summary>
		/// 按钮Id
		/// </summary>
		public string ButtonId { get; set; }
		/// <summary>
		/// 按钮值
		/// </summary>
		public string ButtonValue { get; set; }

	}

}
