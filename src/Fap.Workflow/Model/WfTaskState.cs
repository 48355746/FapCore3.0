using System;
namespace Fap.Workflow.Model
{
    /// <summary>
    /// 字典表的常量 - 流程任务状态(WfTaskState)
	/// 注意：最好由FAP工具生成，不需要手动修改
	/// 只针对属于系统编码的字典
    /// </summary>
    public class WfTaskState
    {
        /// <summary>
        /// 等待
        /// </summary>
        public static string Waiting = "Waiting";
        /// <summary>
        /// 办理
        /// </summary>
        public static string Handling = "Handling";
        /// <summary>
        /// 完成
        /// </summary>
        public static string Completed = "Completed";
        /// <summary>
        /// 撤销
        /// </summary>
        public static string Withdrawed = "Withdrawed";
        /// <summary>
        /// 退回
        /// </summary>
        public static string Backed = "Backed";
        /// <summary>
        /// 作废
        /// </summary>
        public static string Canceled = "Canceled";
        /// <summary>
        /// 否决
        /// </summary>
        public static string Rejected = "Rejected";
        /// <summary>
        /// 终止
        /// </summary>
        public static string Ended = "Ended";
        /// <summary>
        /// 驳回
        /// </summary>
        public static string Revoked = "Revoked";
        /// <summary>
        /// 撤回
        /// </summary>
        public static string Recall = "Recall"; 


    }
}
