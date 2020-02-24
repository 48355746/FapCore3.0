using Fap.Workflow.Engine.Xpdl.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Workflow.Model
{
    /// <summary>
    /// 用于流程提交
    /// </summary>
    public class WfViewModel
    {
        /// <summary>
        /// 业务名称
        /// </summary>
        public string AppName { get; set; }
        /// <summary>
        /// 流程
        /// </summary>
        public string ProcessUid { get; set; }
        /// <summary>
        /// 业务
        /// </summary>
        public string BusinessUid { get; set; }
        /// <summary>
        /// 单据数据Uid
        /// </summary>
        public string BillUid { get; set; }
        /// <summary>
        /// 审批节点Id
        /// </summary>
        public string AvtivityId { get; set; }
        /// <summary>
        /// 下一审批节点审批人
        /// </summary>
        public IList<Performer> NextPerformers { get; set; }
        /// <summary>
        /// 审批状态
        /// </summary>
        public string ApproveState { get; set; }
        /// <summary>
        /// 审批备注
        /// </summary>
        public string ApproveComment { get; set; }
        /// <summary>
        /// 当前节点ID
        /// </summary>
        public string CurrentNodeId { get; set; }
        /// <summary>
        /// 当前审批节点实例Uid
        /// </summary>
        public string CurrentActivityInsUid { get; set; }
        /// <summary>
        /// 当前任务
        /// </summary>
        public string CurrentWfTaskUid { get; set; }
        /// <summary>
        /// 当前流程实例Uid
        /// </summary>
        public string CurrentProcessInsUid { get; set; }
        
        /// <summary>
        /// 单据表
        /// </summary>
        public string BillTable { get; set; }
    }
}
