using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.AspNetCore.Model
{
    public class FilterDescModel
    {
        /// <summary>
        /// 分组
        /// </summary>
        public string Group { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string FilterDesc { get; set; }
        /// <summary>
        /// 操作符
        /// </summary>
        public string FilterOper { get; set; }
        /// <summary>
        /// 结果
        /// </summary>

        public string FilterResult { get; set; }
    }
}
