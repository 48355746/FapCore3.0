using Dapper.Contrib.Extensions;
using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fap.Hcm.Service.Time
{
    /// <summary>
    /// 排班表
    /// </summary>
    public class TmSchedule : BaseModel
    {
        /// <summary>
        /// 排班UID
        /// </summary>
        public string ScheduleUid { get; set; }       
        /// <summary>
        /// 班次
        /// </summary>
        public string ShiftUid { get; set; }
        /// <summary>
        /// 班次 的显性字段MC
        /// </summary>
        [Computed]
        public string ShiftUidMC { get; set; }        
        /// <summary>
        /// 工作日期
        /// </summary>
        public string WorkDay { get; set; }
        /// <summary>
        /// 上班时间
        /// </summary>
        public string StartTime { get; set; }
        /// <summary>
        /// 下班时间
        /// </summary>
        public string EndTime { get; set; }
        /// <summary>
        /// 迟到时间
        /// </summary>
        public string LateTime { get; set; }
        /// <summary>
        /// 早退时间
        /// </summary>
        public string LeaveTime { get; set; }
        /// <summary>
        /// 计算打卡开始时间
        /// </summary>
        public string StartCardTime { get; set; }
        /// <summary>
        /// 计算打卡结束时间
        /// </summary>
        public string EndCardTime { get; set; }
        /// <summary>
        /// 排班工作时长
        /// </summary>
        public double WorkHoursLength { get; set; }
        /// <summary>
        /// 排班休息时长
        /// </summary>
        public double RestMinutesLength { get; set; }
        /// <summary>
        /// 休息开始时间
        /// </summary>
        public string RestStartTime { get; set; }
        /// <summary>
        /// 休息结束时间
        /// </summary>
        public string RestEndTime { get; set; }

    }
}
