
using System.Collections.Generic;
namespace Fap.Workflow.Engine.Entity
{
    /// <summary>
    /// 流程xml文件查询实体
    /// </summary>
    public class ProcessFileQuery
    {
        /// <summary>
        /// 操作类型， 新增add， 编辑edit， 查看view, 流程图实例查看viewdiagram
        /// </summary>
        public string Oper { get; set; }
        /// <summary>
        /// 流程模板类型
        /// </summary>
        public string TemplateType { get; set; }
        /// <summary>
        /// 流程模板Fid
        /// </summary>
        public string TemplateFid { get; set; }
        /// <summary>
        /// 流程实例Id
        /// </summary>
        public string ProcessId { get; set; }

    }

}
