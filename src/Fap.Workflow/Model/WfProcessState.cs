using System;
namespace Fap.Workflow.Model
{
    /// <summary>
    /// 字典表的常量 - 流程模板状态(WfProcessStatus)
	/// 注意：最好由FAP工具生成，不需要手动修改
	/// 只针对属于系统编码的字典
    /// </summary>
    public class WfProcessState
    {
		/// <summary>
		/// 启用
		/// </summary>
		public static string Using = "Using"; 
		/// <summary>
		/// 禁用
		/// </summary>
		public static string Forbidden = "Forbidden"; 
		/// <summary>
		/// 历史
		/// </summary>
		public static string Historical = "Historical"; 

    }
}
