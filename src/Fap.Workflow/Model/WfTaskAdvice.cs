using Dapper.Contrib.Extensions;
using Fap.Core.Infrastructure.Metadata;
using System;

namespace Fap.Workflow.Model
{
    /// <summary>
    /// 任务意见
    /// </summary>
    public class WfTaskAdvice : BaseModel
    {
        /// <summary>
        /// 所属任务
        /// </summary>
        public string TaskUid { get; set; }
        /// <summary>
        /// 所属任务 的显性字段MC
        /// </summary>
        [ComputedAttribute]
        public string TaskUidMC { get; set; }
        /// <summary>
        /// 处理事件
        /// </summary>
        public string HandleEvent { get; set; }
        /// <summary>
        /// 处理意见
        /// </summary>
        public string Suggestion { get; set; }
        /// <summary>
        /// 处理时间
        /// </summary>
        public string HandleTime { get; set; }
        /// <summary>
        /// 处理结果
        /// </summary>
        public string HandleResult { get; set; }
        /// <summary>
        /// 处理人
        /// </summary>
        public string HandleByUser { get; set; }
        /// <summary>
        /// 处理人 的显性字段MC
        /// </summary>
        [ComputedAttribute]
        public string HandleByUserMC { get; set; }
        /// <summary>
        /// 处理人名称
        /// </summary>
        public string HandleByUserName { get; set; }

        /// <summary>
        /// 流程
        /// </summary>
        public string ProcessId { get; set; }
        /// <summary>
        /// 流程 的显性字段MC
        /// </summary>
        [ComputedAttribute]
        public string ProcessIdMC { get; set; }
        /// <summary>
        /// 附件
        /// </summary>
        public string AppendFile { get; set; }
        /// <summary>
        /// 审批结果
        /// </summary>
        public string ApproveState { get; set; }

        /// <summary>
        /// 协助人
        /// </summary>
        public string AssistUser { get; set; }
        /// <summary>
        /// 协助人 的显性字段MC
        /// </summary>
        [ComputedAttribute]
        public string AssistUserMC { get; set; }
        /// <summary>
        /// 协助人名称
        /// </summary>
        public string AssistUserName { get; set; }


    }

}
