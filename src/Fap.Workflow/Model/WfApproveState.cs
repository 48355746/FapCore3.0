using System;
namespace Fap.Workflow.Model
{
    /// <summary>
    /// 字典表的常量 - 审批状态(WfApproveState)
    /// 注意：最好由FAP工具生成，不需要手动修改
    /// 只针对属于系统编码的字典
    /// </summary>
    public class WfApproveState
    {
        /// <summary>
        /// 暂无结论
        /// </summary>
        public static string None = "None";
        /// <summary>
        /// 同意
        /// </summary>
        public static string Agree = "Agree";
        /// <summary>
        /// 不同意
        /// </summary>
        public static string Disagree = "Disagree"; 

    }
}
