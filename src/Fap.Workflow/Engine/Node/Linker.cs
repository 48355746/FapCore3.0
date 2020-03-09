using Fap.Workflow.Engine.Xpdl;
using Fap.Workflow.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Workflow.Engine.Node
{
    /// <summary>
    /// 节点连接线
    /// </summary>
    public class Linker
    {
        /// <summary>
        /// 起始节点定义
        /// </summary>
        public ActivityEntity FromActivity { get; set; }

        /// <summary>
        /// 起始节点实例
        /// </summary>
        public WfActivityInstance FromActivityInstance { get; set; }

        /// <summary>
        /// 到达节点定义
        /// </summary>
        public ActivityEntity ToActivity { get; set; }

        /// <summary>
        /// 到达节点实例
        /// </summary>
        public WfActivityInstance ToActivityInstance { get; set; }
    }
}
