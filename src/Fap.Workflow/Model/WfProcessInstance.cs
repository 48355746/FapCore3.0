using Dapper.Contrib.Extensions;
using Fap.Core.Infrastructure.Metadata;
using System;

namespace Fap.Workflow.Model
{

    /// <summary>
    /// 流程实例
    /// </summary>
    public class WfProcessInstance : BaseModel
    {
        /// <summary>
        /// 流程名称
        /// </summary>
        public string ProcessName { get; set; }
        /// <summary>
        /// 所属流程
        /// </summary>
        public string ProcessUid { get; set; }
        /// <summary>
        /// 所属流程 的显性字段MC
        /// </summary>
        [ComputedAttribute]
        public string ProcessUidMC { get; set; }
        /// <summary>
        /// 流程描述
        /// </summary>
        public string ProcessDesc { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public string StartTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public string EndTime { get; set; }
        /// <summary>
        /// 流程状态
        /// </summary>
        public string ProcessState { get; set; }
        /// <summary>
        /// 流程状态 的显性字段MC
        /// </summary>
        [ComputedAttribute]
        public string ProcessStateMC { get; set; }
        /// <summary>
        /// 发起人
        /// </summary>
        public string AppEmpUid { get; set; }
        /// <summary>
        /// 发起人 的显性字段MC
        /// </summary>
        [ComputedAttribute]
        public string AppEmpUidMC { get; set; }
        /// <summary>
        /// 发起人名称
        /// </summary>
        public string AppEmpName { get; set; }
        /// <summary>
        /// 流程版本
        /// </summary>
        public int Version { get; set; }
        /// <summary>
        /// 业务ID
        /// </summary>
        public string BizUid { get; set; }
        /// <summary>
        /// 业务名称
        /// </summary>
        public string BizName { get; set; }
        /// <summary>
        /// 是否带有表单
        /// </summary>
        public int IsHasForm { get; set; }
        /// <summary>
        /// 所属业务
        /// </summary>
        public string BizTypeUid { get; set; }
        /// <summary>
        /// 所属业务 的显性字段MC
        /// </summary>
        [ComputedAttribute]
        public string BizTypeUidMC { get; set; }
        /// <summary>
        /// 是否处于处理状态
        /// </summary>
        public int IsHandling { get; set; }
        /// <summary>
        /// 审批结论
        /// </summary>
        public string ApproveResult { get; set; }
        /// <summary>
        /// 审批结论 的显性字段MC
        /// </summary>
        [ComputedAttribute]
        public string ApproveResultMC { get; set; }
        /// <summary>
        /// 消息设置
        /// </summary>
        public string MessageSetting { get; set; }
        /// <summary>
        /// 回写状态
        /// </summary>
        public int WriteBackState { get; set; }
        /// <summary>
        /// 回写时间
        /// </summary>
        public string WriteBackTime { get; set; }
        /// <summary>
        /// 父流程实例
        /// </summary>
        public string ParentProcessInsUid { get; set; }
        /// <summary>
        /// 父流程实例 的显性字段MC
        /// </summary>
        [ComputedAttribute]
        public string ParentProcessInsUidMC { get; set; }
        /// <summary>
        /// 父流程ID
        /// </summary>
        public string ParentProcessUid { get; set; }
        /// <summary>
        /// 子流程节点实例ID
        /// </summary>
        public string InvokeActivityInsUid { get; set; }
        /// <summary>
        /// 子流程节点ID
        /// </summary>
        public int InvokeActivityUid { get; set; }

        /// <summary>
        /// 表单模板
        /// </summary>
        public string FormTemplateUid { get; set; }
        /// <summary>
        /// 表单模板 的显性字段MC
        /// </summary>
        [ComputedAttribute]
        public string FormTemplateUidMC { get; set; }

        /// <summary>
        /// 表单类型
        /// </summary>
        public string FormType { get; set; }
        /// <summary>
        /// 表单类型 的显性字段MC
        /// </summary>
        [ComputedAttribute]
        public string FormTypeMC { get; set; }
        /// <summary>
        /// 单据表
        /// </summary>
        public string BillTable { get; set; }

        /// <summary>
        /// 流程是否运行
        /// </summary>
        [ComputedAttribute]
        public bool IsRunning
        {
            get
            {
                return this.ProcessState == WfProcessInstanceState.Running;
            }
        }

        /// <summary>
        /// 不可运行的流程状态信息提示
        /// </summary>
        [ComputedAttribute]
        public string ProcessStateOfOutRunningMessage
        {
            get
            {
                if (this.ProcessState == WfProcessInstanceState.Completed)
                {
                    return "该业务流程已完成";
                }
                else if (this.ProcessState == WfProcessInstanceState.Canceled)
                {
                    return "该业务流程已取消";
                }
                else if (this.ProcessState == WfProcessInstanceState.Recall)
                {
                    return "该业务流程已撤回";
                }
                else if (this.ProcessState == WfProcessInstanceState.Withdrawed)
                {
                    return "该业务流程已撤销";
                }
                else if (this.ProcessState == WfProcessInstanceState.Suspended)
                {
                    return "该业务流程已挂起";
                }
                else if (this.ProcessState == WfProcessInstanceState.Ended)
                {
                    return "该业务流程已终止";
                }
                else
                {
                    return "";
                }
            }
        }
    }

}
