using Dapper.Contrib.Extensions;
using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Infrastructure.Model
{
    /// <summary>
    /// 员工日历
    /// </summary>
    [Serializable]
    public class EssCalendar : BaseModel
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string EventName { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public string StartTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public string EndTime { get; set; }
        /// <summary>
        /// 样式
        /// </summary>
        public string EventClass { get; set; }
        /// <summary>
        /// Url
        /// </summary>
        public string EventUrl { get; set; }
        /// <summary>
        /// 全天事件
        /// </summary>
        public int IsAllDay { get; set; }
        /// <summary>
        /// 员工
        /// </summary>
        public string EmpUid { get; set; }
        /// <summary>
        /// 员工 的显性字段MC
        /// </summary>
        [Computed]
        public string EmpUidMC { get; set; }
        /// <summary>
        /// 来源
        /// </summary>
        public string Origin { get; set; }

    }
}
