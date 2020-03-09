using Fap.Workflow.Engine.Xpdl.Entity;

namespace Fap.Workflow.Engine.Xpdl.Node
{
    /// <summary>
    /// 子流程节点
    /// </summary>
    internal class SubProcessNode : NodeBase
    {
        public string SubProcessId { get; set; }

        internal SubProcessNode(ActivityEntity activity) :
            base(activity)
        {

        }
    }
}
