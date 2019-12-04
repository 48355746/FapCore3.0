using Fap.Core.Infrastructure.Metadata;
using System;

namespace Fap.Workflow.Model
{
    /// <summary>
    /// 流程图
    /// </summary>
    public class WfDiagram : BaseModel
    {
        /// <summary>
        /// 流程模板
        /// </summary>
        public string ProcessUid { get; set; }
        /// <summary>
        /// 流程名称
        /// </summary>
        public string ProcessName { get; set; }
        /// <summary>
        /// 流程内容
        /// </summary>
        public string XmlContent { get; set; }
        /// <summary>
        /// 版本号
        /// </summary>
        public int Version { get; set; }

    }

}
