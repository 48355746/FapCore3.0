using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.AspNetCore.Controls.JqGrid
{
    /// <summary>
    /// 列头分组
    /// </summary>
    public class GroupHeader
    {
        /// <summary>
        /// 分组列数，合并几个列到一个分组
        /// </summary>
        public int NumberOfColumns { get; set; }
        /// <summary>
        /// 列标题
        /// </summary>
        public string TitleText { get; set; }
        /// <summary>
        /// 起始分组列名称 
        /// </summary>
        public string StartColumnName { get; set; }
    }
}
