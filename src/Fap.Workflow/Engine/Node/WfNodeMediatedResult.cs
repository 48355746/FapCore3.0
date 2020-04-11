using Fap.Workflow.Engine.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Workflow.Engine.Node
{
    /// <summary>
    /// 活动节点执行结果
    /// </summary>
    public class WfNodeMediatedResult : WfExecutedResult
    {
        public WfNodeMediatedFeedback Feedback { get; set; }
    }

    /// <summary>
    /// 执行反馈枚举
    /// </summary>
    public enum WfNodeMediatedFeedback
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
