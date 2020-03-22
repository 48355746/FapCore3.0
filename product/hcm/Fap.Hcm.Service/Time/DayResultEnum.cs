using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Fap.Hcm.Service.Time
{
    /// <summary>
    /// 日结果
    /// </summary>
    public enum DayResultEnum
    {
        /// <summary>
        /// 正常
        /// </summary>
        [Description("正常")]
        Normal,
        /// <summary>
        /// 迟到
        /// </summary>
        [Description("迟到")]
        ComeLate,
        /// <summary>
        /// 早退
        /// </summary>
        [Description("早退")]
        LeaveEarly,
        /// <summary>
        /// 缺勤
        /// </summary>
        [Description("缺勤")]
        Absence,
        /// <summary>
        /// 请假
        /// </summary>
        [Description("请假")]
        TakeLeave,
        /// <summary>
        /// 出差
        /// </summary>
        [Description("出差")]
        Travel
    }
}
