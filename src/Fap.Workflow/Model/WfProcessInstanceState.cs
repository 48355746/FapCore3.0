using System;
namespace Fap.Workflow.Model
{
    /// <summary>
    /// 字典表的常量 - 流程状态(WfProcessState)
	/// 注意：最好由FAP工具生成，不需要手动修改
	/// 只针对属于系统编码的字典
    /// </summary>
    public class WfProcessInstanceState
    {
        /// <summary>
        /// 运行
        /// </summary>
        public static string Running = "Running";
        /// <summary>
        /// 完成
        /// </summary>
        public static string Completed = "Completed";
        /// <summary>
        /// 挂起
        /// </summary>
        public static string Suspended = "Suspended";
        /// <summary>
        /// 作废
        /// </summary>
        public static string Canceled = "Canceled";
        /// <summary>
        /// 撤销
        /// </summary>
        public static string Withdrawed = "Withdrawed";
        /// <summary>
        /// 终止
        /// </summary>
        public static string Ended = "Ended";
        /// <summary>
        /// 删除
        /// </summary>
        public static string Deleted = "Deleted";
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
