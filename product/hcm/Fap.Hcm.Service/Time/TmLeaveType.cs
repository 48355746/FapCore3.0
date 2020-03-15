using Dapper.Contrib.Extensions;
using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Hcm.Service.Time
{
    /// <summary>
    /// 请假类型
    /// </summary>
    public class TmLeaveType :BaseModel
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string TypeCode { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string TypeName { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int SortBy { get; set; }
        /// <summary>
        /// 限制条件
        /// </summary>
        public string LimitCondition { get; set; }
        /// <summary>
        /// 提示信息
        /// </summary>
        public string Tooltip { get; set; }
        /// <summary>
        /// 每年允许天数
        /// </summary>
        public double AllowDays { get; set; }
        /// <summary>
        /// 是否工作日
        /// </summary>
        public int IsWorkDay { get; set; }
        /// <summary>
        /// 时长
        /// </summary>
        [Computed]
        public double HoursLength { get; set; }

    }

}
