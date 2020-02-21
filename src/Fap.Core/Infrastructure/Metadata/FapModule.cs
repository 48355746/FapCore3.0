using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Infrastructure.Metadata
{
    /// <summary>
    /// 系统模块表
    /// </summary>
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
