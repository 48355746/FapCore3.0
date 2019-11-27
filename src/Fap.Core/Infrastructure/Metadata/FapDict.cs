using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Infrastructure.Metadata
{
    /// <summary>
    /// 字典表
    /// </summary>
    [Serializable]
    public class FapDict : BaseModel
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 父级编码
        /// </summary>
        public string Pid { get; set; }
        /// <summary>
        /// 是否末级
        /// </summary>
        public int IsEndLevel { get; set; }
        /// <summary>
        /// 分类
        /// </summary>
        public string Category { get; set; }
        /// <summary>
        /// 分类名称
        /// </summary>
        public string CategoryName { get; set; }
        /// <summary>
        /// 编码路径
        /// </summary>
        public string CPath { get; set; }
        /// <summary>
        /// 是否系统编码
        /// </summary>
        public int IsSystem { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int SortBy { get; set; }

    }
}
