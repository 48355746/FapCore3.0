using Dapper.Contrib.Extensions;
using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fap.Core.Rbac.Model
{
	/// <summary>
	/// 菜单按钮
	/// </summary>
	public class FapMenuButton :BaseModel
	{
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
		/// 描述
		/// </summary>
		public string Description { get; set; }
		
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
		/// 按钮标记
		/// </summary>
		public string ButtonID { get; set; }
		/// <summary>
		/// 按钮名称
		/// </summary>
		public string ButtonName { get; set; }
		/// <summary>
		/// 启用
		/// </summary>
		public int Enabled { get; set; }

	}

}
