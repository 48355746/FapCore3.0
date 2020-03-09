using Fap.Workflow.Engine.Common;
using Fap.Workflow.Engine.Xpdl;
using Fap.Workflow.Engine.Xpdl.Entity;
using Fap.Workflow.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Workflow.Engine.Core
{
    /// <summary>
    /// 流程回退处理时的上下文对象
    /// </summary>
    internal class BackwardContext
    {
        internal ActivityEntity BackwardToTaskActivity { get; set; }
        internal WfActivityInstance BackwardToTaskActivityInstance { get; set; }
        internal ActivityEntity BackwardFromActivity { get; set; }
        internal WfActivityInstance BackwardFromActivityInstance { get; set; }
        internal WfProcessInstance ProcessInstance { get; set; }
        internal String BackwardToTargetTransitionGUID { get; set; }
        internal WfBackwardTaskReciever BackwardTaskReciever { get; set; }
        internal ActivityEntity ActivityCurrent { get; set; }
    }
}
