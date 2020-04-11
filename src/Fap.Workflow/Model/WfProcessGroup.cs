using Fap.Core.Infrastructure.Metadata;
using System;

namespace Fap.Workflow.Model
{
    /// <summary>
    /// 流程模板分类
    /// </summary>
    [Serializable]
    public class WfProcessGroup : BaseModel
    {
        /// <summary>
        /// 组名称
        /// </summary>
        public string GroupName { get; set; }
        /// <summary>
        /// 组编码
        /// </summary>
        public string GroupCode { get; set; }
        /// <summary>
        /// 上级分组
        /// </summary>
        public string Pid { get; set; }

    }

}
