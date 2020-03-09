using Fap.Workflow.Engine.Common;

namespace Fap.Workflow.Engine.Common
{
    /// <summary>
    /// 活动节点执行结果
    /// </summary>
    internal class WorkflowActivityMediatedResult : WfExecutedResult
    {
        public WorkflowActivityMediatedFeedback Feedback { get; set; }
    }

    public enum WorkflowActivityMediatedFeedback
    {
        /// <summary>
        /// 串行会(加)签，设置下一个执行节点的任务进入运行状态
        /// </summary>
        ForwardToNextSequenceTask = 1,

        /// <summary>
        /// 并行会(加)签，等待节点到达足够多的完成比例
        /// </summary>
        WaitingForCompletedMore = 2
    }
}
