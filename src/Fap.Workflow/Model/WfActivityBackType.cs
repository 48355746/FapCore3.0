using System;
namespace Fap.Workflow.Model
{
    /// <summary>
    /// 字典表的常量 - 流程活动回退类型(WfActivityBackType)
	/// 注意：最好由FAP工具生成，不需要手动修改
	/// 只针对属于系统编码的字典
    /// </summary>
    public class WfActivityBackType
    {
		/// <summary>
		/// 撤销
		/// </summary>
		public static string Withdrawed = "Withdrawed"; 
		/// <summary>
		/// 退回
		/// </summary>
		public static string Sendback = "Sendback";
        /// <summary>
        /// 驳回
        /// </summary>
        public static string Revoked = "Revoked"; 
		/// <summary>
		/// 返签
		/// </summary>
		public static string Reversed = "Reversed"; 
		/// <summary>
		/// 多实例撤销
		/// </summary>
		public static string WithdrawedOfMI = "WithdrawedOfMI"; 
		/// <summary>
		/// 多实例退回
		/// </summary>
		public static string SendbackOfMI = "SendbackOfMI"; 

    }
}
