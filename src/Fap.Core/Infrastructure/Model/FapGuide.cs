using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Infrastructure.Model
{

	/// <summary>
	/// 使用指南
	/// </summary>
	public class FapGuide : Fap.Core.Infrastructure.Metadata.BaseModel
	{
		/// <summary>
		/// 地址
		/// </summary>
		public string Navigation { get; set; }
		/// <summary>
		/// 使用指南
		/// </summary>
		public string Guide { get; set; }

	}
}
