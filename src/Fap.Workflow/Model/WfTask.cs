using Dapper.Contrib.Extensions;
using Fap.Core.Infrastructure.Metadata;
using System;

namespace Fap.Workflow.Model
{
    /// <summary>
    /// 流程任务
    /// </summary>
    public class WfTask : BaseModel
    {
        /// <summary>
        /// 活动实例
        /// </summary>
        public string ActivityInsUid { get; set; }
        /// <summary>
        /// 任务类型
        /// </summary>
        public string TaskType { get; set; }
        /// <summary>
        /// 任务类型 的显性字段MC
        /// </summary>
        [Computed]
        public string TaskTypeMC { get; set; }
        /// <summary>
        /// 活动节点名称
        /// </summary>
        public string NodeName { get; set; }
        /// <summary>
        /// 活动节点
        /// </summary>
        public string NodeId { get; set; }
        /// <summary>
        /// 任务名称
        /// </summary>
        public string TaskName { get; set; }
        /// <summary>
        /// 执行人名称
        /// </summary>
        public string ExecutorEmpName { get; set; }
        /// <summary>
        /// 执行人
        /// </summary>
        public string ExecutorEmpUid { get; set; }
        /// <summary>
        /// 执行人 的显性字段MC
        /// </summary>
        [Computed]
        public string ExecutorEmpUidMC { get; set; }
        /// <summary>
        /// 任务状态
        /// </summary>
        public string TaskState { get; set; }
        /// <summary>
        /// 任务状态 的显性字段MC
        /// </summary>
        [Computed]
        public string TaskStateMC { get; set; }
        /// <summary>
        /// 处理时间
        /// </summary>
        public string ExecuteTime { get; set; }
        /// <summary>
        /// 记录状态
        /// </summary>
        public int RecordState { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public string TaskStartTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public string TaskEndTime { get; set; }
        /// <summary>
        /// 应用名称
        /// </summary>
        public string AppName { get; set; }
        /// <summary>
        /// Bill数据ID
        /// </summary>
        public string BillUid { get; set; }
        /// <summary>
        /// 所属业务
        /// </summary>
        public string BusinessUid { get; set; }
        /// <summary>
        /// 所属业务 的显性字段MC
        /// </summary>
        [Computed]
        public string BusinessUidMC { get; set; }
        /// <summary>
        /// 流程实例
        /// </summary>
        public string ProcessInsUid { get; set; }
        /// <summary>
        /// 流程实例 的显性字段MC
        /// </summary>
        [Computed]
        public string ProcessInsUidMC { get; set; }
        /// <summary>
        /// 流程
        /// </summary>
        public string ProcessUid { get; set; }
        /// <summary>
        /// 流程 的显性字段MC
        /// </summary>
        [Computed]
        public string ProcessUidMC { get; set; }

        /// <summary>
        /// 申请人
        /// </summary>
        public string AppEmpUid { get; set; }
        /// <summary>
        /// 申请人 的显性字段MC
        /// </summary>
        [Computed]
        public string AppEmpUidMC { get; set; }
        /// <summary>
        /// 申请时间
        /// </summary>
        public string AppStartTime { get; set; }
        /// <summary>
        /// 流程状态
        /// </summary>
        public string ProcessState { get; set; }
        /// <summary>
        /// 流程状态 的显性字段MC
        /// </summary>
        [Computed]
        public string ProcessStateMC { get; set; }
        /// <summary>
        /// 审批状态
        /// </summary>
        public string ApproveState { get; set; }
        /// <summary>
        /// 审批状态 的显性字段MC
        /// </summary>
        [Computed]
        public string ApproveStateMC { get; set; }
        /// <summary>
        /// 批注意见
        /// </summary>
        public string Suggestion { get; set; }
        /// <summary>
        /// 任务活动类型
        /// </summary>
        public string TaskActivityType { get; set; }
        
        /// <summary>
        /// 代办人
        /// </summary>
        public string AgentEmpUid { get; set; }
        /// <summary>
        /// 代办人名称
        /// </summary>
        public string AgentEmpName { get; set; }
        /// <summary>
        /// 审批顺序
        /// </summary>
        public int ApproverSort { get; set; }
        /// <summary>
        /// 是否加签任务
        /// </summary>
        public int IsSignTask { get; set; }
        /// <summary>
        /// 加签类型
        /// </summary>
        public string SignTaskType { get; set; }
        /// <summary>
        /// 加签类型 的显性字段MC
        /// </summary>
        [Computed]
        public string SignTaskTypeMC { get; set; }

        /// <summary>
        /// 任务是否可以办理
        /// </summary>
        [Computed]
        public bool IsCanHandle
        {
            get
            {
                return this.TaskState == WfTaskState.Backed
                    || this.TaskState == WfTaskState.Handling;
                    //|| this.TaskState == WfTaskState.Waiting;
            }
        }

        /// <summary>
        /// 不可办理的任务的状态信息提示
        /// </summary>
        [Computed]
        public string TaskStateMessageOfOutHandle
        {
            get
            {
                if (this.TaskState == WfTaskState.Completed)
                {
                    return "该任务已完成";
                }
                else if (this.TaskState == WfTaskState.Rejected)
                {
                    return "该任务已否决";
                } 
                else if (this.TaskState == WfTaskState.Withdrawed)
                {
                    return "该任务已撤销";
                }
                else if (this.TaskState == WfTaskState.Canceled)
                {
                    return "该任务已取消";
                }
                else if (this.TaskState == WfTaskState.Ended)
                {
                    return "该任务已终止";
                }
                else if (this.TaskState == WfTaskState.Backed)
                {
                    return "该任务已退回";
                }
                else if (this.TaskState == WfTaskState.Revoked)
                {
                    return "该任务已驳回";
                }
                else if (this.TaskState == WfTaskState.Waiting)
                {
                    return "该任务正处于等待状态";
                }
                else
                {
                    return "";
                }
            }
        }
    }

}
