using Dapper.Contrib.Extensions;
using Fap.Core.Infrastructure.Metadata;
using System;

namespace Fap.Workflow.Model
{
    /// <summary>
    /// 流程转移实例
    /// </summary>
    public class WfTransitionInstance : BaseModel
    {
        /// <summary>
        /// 流程
        /// </summary>
        public string ProcessUid { get; set; }
        /// <summary>
        /// 流程实例
        /// </summary>
        public string ProcessInsUid { get; set; }
        /// <summary>
        /// 流程实例 的显性字段MC
        /// </summary>
        [ComputedAttribute]
        public string ProcessInsUidMC { get; set; }
        /// <summary>
        /// 转移类型
        /// </summary>
        public string TransitionType { get; set; }
        /// <summary>
        /// 转移类型 的显性字段MC
        /// </summary>
        [ComputedAttribute]
        public string TransitionTypeMC { get; set; }
        /// <summary>
        /// 源节点类型
        /// </summary>
        public string SourceActivityNodeType { get; set; }
        /// <summary>
        /// 源节点
        /// </summary>
        public string SourceActivityNodeId { get; set; }
        /// <summary>
        /// 源活动实例
        /// </summary>
        public string SourceActivityInsUid { get; set; }
        /// <summary>
        /// 源节点名称
        /// </summary>
        public string SourceActivityNodeName { get; set; }
        /// <summary>
        /// 目标节点类型
        /// </summary>
        public string TargetActivityNodeType { get; set; }
        /// <summary>
        /// 目标活动实例
        /// </summary>
        public string TargetActivityInsUid { get; set; }
        /// <summary>
        /// 目标节点
        /// </summary>
        public string TargetActivityNodeId { get; set; }
        /// <summary>
        /// 目标节点名称
        /// </summary>
        public string TargetActivityNodeName { get; set; }
        /// <summary>
        /// 转移状态
        /// </summary>
        public int TransitionState { get; set; }
        /// <summary>
        /// 流转节点
        /// </summary>
        public string TransitionNodeId { get; set; }
        /// <summary>
        /// 通过状态
        /// </summary>
        public int WalkState { get; set; }
        /// <summary>
        /// 跳转类型
        /// </summary>
        public int JumpType { get; set; }

    }


}
