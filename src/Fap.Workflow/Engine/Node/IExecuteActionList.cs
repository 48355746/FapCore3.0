using System.Collections.Generic;
using Fap.Workflow.Engine.Xpdl;

namespace Fap.Wrokflow.Engine.Node
{
    /// <summary>
    /// 调用外部程序或服务的接口
    /// </summary>
    internal interface IExecuteActionList
    {
        void ExecteActionList(IList<ActionEntity> actionList, IDictionary<string, ActionParameterInternal> actionMethodParameters);
    }
}
