using Dapper.Contrib.Extensions;
using Fap.Core.Infrastructure.Metadata;
using System;

namespace Fap.Workflow.Model
{
    /// <summary>
    /// 流程模板
    /// </summary>
    public class WfProcess :BaseModel
    {
        /// <summary>
        /// 模板名称
        /// </summary>
        public string ProcessName { get; set; }
        /// <summary>
        /// 版本号
        /// </summary>
        public int Version { get; set; }
        /// <summary>
        /// 流程分类
        /// </summary>
        public string ProcessGroupUid { get; set; }
        /// <summary>
        /// 流程分类 的显性字段MC
        /// </summary>
        [Computed]
        public string ProcessGroupUidMC { get; set; }
        /// <summary>
        /// 表单类型
        /// </summary>
        public string FormType { get; set; }
        /// <summary>
        /// 表单类型 的显性字段MC
        /// </summary>
        [Computed]
        public string FormTypeMC { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 状态 的显性字段MC
        /// </summary>
        [Computed]
        public string StatusMC { get; set; }
        /// <summary>
        /// 流程图
        /// </summary>
        public string DiagramId { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 自由表单模板
        /// </summary>
        public string FormTemplateUid { get; set; }
        /// <summary>
        /// 流程模板集
        /// </summary>
        public string CollectionId { get; set; }
        /// <summary>
        /// 是否带有表单
        /// </summary>
        public int IsHasForm { get; set; }
        /// <summary>
        /// 单据表
        /// </summary>
        public string BillTable { get; set; }
        /// <summary>
        /// 消息设置
        /// </summary>
        public string MessageSetting { get; set; }

    }


}
