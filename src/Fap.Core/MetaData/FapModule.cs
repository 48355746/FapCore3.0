using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Metadata
{
    /// <summary>
    /// 系统模块表
    /// </summary>
    [Serializable]
    public class FapModule : BaseModel
    {
        /// <summary>
        /// 模块编码
        /// </summary>
        public string ModuleCode { get; set; }
        /// <summary>
        /// 模块名称
        /// </summary>
        public string ModuleName { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int ModuleOrder { get; set; }
        /// <summary>
        /// 父级模块
        /// </summary>
        public string Pid { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public int ActiveFlag { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }
        /// <summary>
        /// 繁体中文
        /// </summary>
        public string LangZhTW { get; set; }
        /// <summary>
        /// 英文
        /// </summary>
        public string LangEn { get; set; }
        /// <summary>
        /// 日本语
        /// </summary>
        public string LangJa { get; set; }
        /// <summary>
        /// 所属产品
        /// </summary>
        public string ProductUid { get; set; }
        /// <summary>
        /// 所属产品 的显性字段MC
        /// </summary>
        [Computed]
        public string ProductUidMC { get; set; }


    }
}
