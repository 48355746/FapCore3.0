using Fap.Workflow.Engine.Node;
using Fap.Workflow.Model;

namespace Fap.Wrokflow.Engine.Node
{
    /// <summary>
    /// 路由接口
    /// </summary>
    internal interface ICompleteAutomaticlly
    {
        GatewayExecutedResult CompleteAutomaticlly(WfProcessInstance processInstance,
            string transitionNodeId,
            WfActivityInstance fromActivityInstance);
    }
}
