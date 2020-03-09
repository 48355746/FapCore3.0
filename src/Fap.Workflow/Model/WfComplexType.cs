using System;
namespace Fap.Workflow.Model
{
    /// <summary>
    /// 字典表的常量 - 流程节点附属类型(WfComplexType)
	/// 注意：最好由FAP工具生成，不需要手动修改
	/// 只针对属于系统编码的字典
    /// </summary>
    public class WfComplexType
    {
		/// <summary>
		/// 会签多实例
		/// </summary>
		public static string SignTogether = "SignTogether"; 
		/// <summary>
		/// 加签多实例
		/// </summary>
		public static string SignForward = "SignForward"; 

    }
}
