using System;
using System.Collections.Generic;

namespace Fap.Workflow.Engine.Entity
{
    /// <summary>
    /// 流程文件实体对象
    /// </summary>
    public class ProcessFileEntity
    {
        /// <summary>
        /// 流程图ID
        /// </summary>
        public string ProcessId { get; set; }
        /// <summary>
        /// 流程模板Fid
        /// </summary>
        public string TemplateFid { get; set; }
        /// <summary>
        /// 流程模板类型
        /// </summary>
        public string TemplateType { get; set; }
        /// <summary>
        /// 表单类型
        /// </summary>
        public string FormType { get; set; }
        /// <summary>
        /// 表单外挂链接
        /// </summary>
        public string FormUrl { get; set; }
        /// <summary>
        /// 流程图名称
        /// </summary>
        public string ProcessName { get; set; }
        /// <summary>
        /// 流程图描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 消息设置
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 流程图XML
        /// </summary>
        public string XmlContent { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<ProcessNodeState> ProcessNodes { get; set; }
    }

    /// <summary>
    /// 流程图节点的状态
    /// </summary>
    public class ProcessNodeState
    {
        public string NodeId { get; set; }

        /// <summary>
        /// 节点状态：同意，不同意，退回、驳回、撤销等
        /// </summary>
        public string NodeState { get; set; }
        public string ApproveState { get; set; }
        /// <summary>
        /// 节点类型
        /// </summary>
        public string NodeType { get; set; }
        /// <summary>
        /// 实际执行人
        /// </summary>
        public string RealExecutor { get;set;}
        /// <summary>
        /// 执行人
        /// </summary>
        public string Executor { get; set; }
        /// <summary>
        /// 执行时间
        /// </summary>
        public string ExecuteTime { get; set; }
    }
}
