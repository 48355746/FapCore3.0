using Dapper.Contrib.Extensions;
using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Infrastructure.Model
{
    /// <summary>
    /// 常用日历事件
    /// </summary>
    public class CfgCalendarEvent :BaseModel
    {
        /// <summary>
        /// 人员
        /// </summary>
        public string EmpUid { get; set; }
        /// <summary>
        /// 人员 的显性字段MC
        /// </summary>
        [Computed]
        public string EmpUidMC { get; set; }
        /// <summary>
        /// 事件编码
        /// </summary>
        public string EventCode { get; set; }
        /// <summary>
        /// 事件名称
        /// </summary>
        public string EventName { get; set; }
        /// <summary>
        /// 颜色
        /// </summary>
        public string EventColor { get; set; }
        /// <summary>
        /// 是否全局
        /// </summary>
        public int IsGlobal { get; set; }

    }
}
