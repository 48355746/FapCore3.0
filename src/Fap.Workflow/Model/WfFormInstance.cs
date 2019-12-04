using Dapper.Contrib.Extensions;
using Fap.Core.Infrastructure.Metadata;
using System;

namespace Fap.Workflow.Model
{
    /// <summary>
    /// 流程表单实例
    /// </summary>
    [Serializable]
    public class WfFormInstance :BaseModel
    {
        /// <summary>
        /// 所属流程
        /// </summary>
        public string ProcessId { get; set; }
        /// <summary>
        /// 所属流程 的显性字段MC
        /// </summary>
        [Computed]
        public string ProcessIdMC { get; set; }
        /// <summary>
        /// 表单名称
        /// </summary>
        public string FormName { get; set; }
        /// <summary>
        /// 表单内容
        /// </summary>
        public string FormHtml { get; set; }
        /// <summary>
        /// 表单数据
        /// </summary>
        public string FormData { get; set; }
        /// <summary>
        /// 表单事件脚本
        /// </summary>
        public string FormEvents { get; set; }
        /// <summary>
        /// 版本号
        /// </summary>
        public int Version { get; set; }
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
        /// 应用实例
        /// </summary>
        public string AppInstanceId { get; set; }
        /// <summary>
        /// 应用名称
        /// </summary>
        public string AppName { get; set; }
        /// <summary>
        /// 应用实例表名
        /// </summary>
        public string AppInstanceTableName { get; set; }
        /// <summary>
        /// 外挂类型
        /// </summary>
        public string AddonType { get; set; }
        /// <summary>
        /// 外挂类型 的显性字段MC
        /// </summary>
        [ComputedAttribute]
        public string AddonTypeMC { get; set; }



    }

}
