using Fap.Workflow.Engine.Xpdl.Entity;
using Fap.Workflow.Model;
using System;
using System.Collections.Generic;

namespace Fap.Workflow.Engine.Entity
{
    /// <summary>
    /// 工作流流转节点的视图对象
    /// </summary>
    public class NodeView
    {
        /// <summary>
        /// 活动节点GUID
        /// </summary>
        public String ActivityId { get; set; }

        /// <summary>
        /// 活动节点名称
        /// </summary>
        public String ActivityName { get; set; }

        /// <summary>
        /// 活动节点编码
        /// </summary>
        public String ActivityCode { get; set; }
        /// <summary>
        /// 节点类型
        /// </summary>
        public string ActivityType { get; set; }
        //public IList<Role> Roles { get; set; }
        //public string ParticipantSelectMode { get; set; }
        public IList<Participant> Participants { get; set; }
        public Boolean IsSkipTo { get; set; }

        /// <summary>
        /// 是否开始节点
        /// </summary>
        public bool IsStartNode
        {
            get
            {
                return ActivityType == WfActivityType.StartNode;
            }
        }

        /// <summary>
        /// 是否结束节点
        /// </summary>
        public bool IsEndNode
        {
            get
            {
                return ActivityType == WfActivityType.EndNode;
            }
        }
    }
}
