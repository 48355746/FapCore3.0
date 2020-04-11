using System;
namespace Fap.Workflow.Model
{
    /// <summary>
    /// 字典表的常量 - 流程活动类型(WfActivityType)
	/// 注意：最好由FAP工具生成，不需要手动修改
	/// 只针对属于系统编码的字典
    /// </summary>
    public class WfActivityType
    {
		/// <summary>
		/// 开始节点
		/// </summary>
		public static string StartNode = "StartNode"; 
		/// <summary>
		/// 结束节点
		/// </summary>
		public static string EndNode = "EndNode"; 
		/// <summary>
		/// 任务节点
		/// </summary>
		public static string TaskNode = "TaskNode";
        /// <summary>
        /// 会签节点
        /// </summary>
        public static string SignNode = "SignNode"; 
		/// <summary>
		/// 子流程节点
		/// </summary>
		public static string SubProcessNode = "SubProcessNode"; 
		/// <summary>
		/// 网关节点
		/// </summary>
		public static string GatewayNode = "GatewayNode"; 
		/// <summary>
		/// 插件节点
		/// </summary>
		public static string PluginNode = "PluginNode"; 
		/// <summary>
		/// 脚本节点
		/// </summary>
		public static string ScriptNode = "ScriptNode"; 
		/// <summary>
		/// 可执行节点
		/// </summary>
		public static string ExecuteNode = "ExecuteNode";
        /// <summary>
        /// 定时节点
        /// </summary>
        public static string TimerNode = "TimerNode";

    }
}
