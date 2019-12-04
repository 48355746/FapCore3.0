using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fap.Workflow.Engine.Enums
{
    /// <summary>
    /// 工作项类型
    /// </summary>
    public enum WorkItemTypeEnum
    {
        /// <summary>
        /// 非工作项StartNode, EndNode, GatewayNode
        /// </summary>
        NonWorkItem = 0,

        /// <summary>
        /// 工作项TaskNode, MultipleInstanceNode
        /// </summary>
        IsWorkItem = 1
    }
}
