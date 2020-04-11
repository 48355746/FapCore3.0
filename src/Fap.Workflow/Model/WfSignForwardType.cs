using System;
namespace Fap.Workflow.Model
{
    /// <summary>
    /// 字典表的常量 - 流程加签类型(WfSignForwardType)
	/// 注意：最好由FAP工具生成，不需要手动修改
	/// 只针对属于系统编码的字典
    /// </summary>
    public class WfSignForwardType
    {
		/// <summary>
		/// 前加签
		/// </summary>
		public static string SignForwardBefore = "SignForwardBefore"; 
		/// <summary>
		/// 后加签
		/// </summary>
		public static string SignForwardBehind = "SignForwardBehind"; 
		/// <summary>
		/// 并行加签
		/// </summary>
		public static string SignForwardParallel = "SignForwardParallel"; 

    }
}
