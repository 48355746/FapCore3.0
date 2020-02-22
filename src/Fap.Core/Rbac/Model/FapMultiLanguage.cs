using System;
using Dapper.Contrib.Extensions;
using Fap.Core.Infrastructure.Metadata;

namespace Fap.Core.Rbac.Model
{ 

	/// <summary>
	/// 多语言
	/// </summary>
	public class FapMultiLanguage :BaseModel
	{
		/// <summary>
		/// 语言键
		/// </summary>
		public string LangKey { get; set; }
		/// <summary>
		/// 语言值
		/// </summary>
		public string LangValue { get; set; }
		/// <summary>
		/// 英文
		/// </summary>
		public string LangValueEn { get; set; }
		/// <summary>
		/// 日本语
		/// </summary>
		public string LangValueJa { get; set; }
		/// <summary>
		/// 中文
		/// </summary>
		public string LangValueZhCn { get; set; }
		/// <summary>
		/// 繁体
		/// </summary>
		public string LangValueZhTW { get; set; }
		/// <summary>
		/// 限定 的显性字段MC
		/// </summary>
		[Computed]
		public string QualifierMC { get; set; }
		/// <summary>
		/// 限定
		/// </summary>
		public string Qualifier { get; set; }

	}
}
