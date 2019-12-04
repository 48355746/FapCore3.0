using Fap.Core.Infrastructure.Metadata;
using System;

namespace Fap.Workflow.Model
{
    /// <summary>
    /// 流程图实例
    /// </summary>
    public class WfDiagramInstance : BaseModel
    {
        /// <summary>
        /// 所属流程实例
        /// </summary>
        public string ProcessInsUid { get; set; }
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
