using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Workflow.Engine.Enums
{
    /// <summary>
    /// 节点审批方式
    /// </summary>
    public enum ApproverMethodEnum
    {
        /// <summary>
        /// 顺序审批
        /// </summary>
        Queue = 0,
        /// <summary>
        /// 任意审批
        /// </summary>
        Anyone = 1
    }
}
