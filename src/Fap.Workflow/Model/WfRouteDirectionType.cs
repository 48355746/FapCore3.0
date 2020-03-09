using System;
namespace Fap.Workflow.Model
{
    /// <summary>
    /// 字典表的常量 - 流程路由方向类型(WfRouteDirectionType)
	/// 注意：最好由FAP工具生成，不需要手动修改
	/// 只针对属于系统编码的字典
    /// </summary>
    public class WfRouteDirectionType
    {
		/// <summary>
		/// 未指定
		/// </summary>
		public static string Unpecified = "Unpecified"; 
		/// <summary>
		/// 或分支
		/// </summary>
		public static string OrSplit = "OrSplit"; 
		/// <summary>
		/// 异或分支
		/// </summary>
		public static string XOrSplit = "XOrSplit"; 
		/// <summary>
		/// 并行分支
		/// </summary>
		public static string AndSplit = "AndSplit"; 
		/// <summary>
		/// 复杂分支
		/// </summary>
		public static string ComplexSplit = "ComplexSplit"; 
		/// <summary>
		/// 所有分支类型
		/// </summary>
		public static string AllSplitType = "AllSplitType"; 
		/// <summary>
		/// 或合并
		/// </summary>
		public static string OrJoin = "OrJoin"; 
		/// <summary>
		/// 异或合并
		/// </summary>
		public static string XOrJoin = "XOrJoin"; 
		/// <summary>
		/// 并行合并
		/// </summary>
		public static string AndJoin = "AndJoin"; 
		/// <summary>
		/// 复杂合并
		/// </summary>
		public static string ComplexJoin = "ComplexJoin"; 
		/// <summary>
		/// 所有合并类型
		/// </summary>
		public static string AllJoinType = "AllJoinType"; 

    }
}
