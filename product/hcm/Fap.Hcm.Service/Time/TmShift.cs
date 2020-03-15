using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fap.Hcm.Service.Time
{
    /// <summary>
    /// 排班班次
    /// </summary>
    [Serializable]
    public class TmShift : BaseModel
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string ShiftCode { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string ShiftName { get; set; }
        /// <summary>
        /// 上班时间
        /// </summary>
        public string StartTime { get; set; }
        /// <summary>
        /// 工作时长(小时)
        /// </summary>
        public double WorkHoursLength { get; set; }
        /// <summary>
        /// 休息时长(分钟)
        /// </summary>
        public double RestMinutesLength { get; set; }
        /// <summary>
        /// 下班时间
        /// </summary>
        public string EndTime { get; set; }
        /// <summary>
        /// 迟到浮动(分钟)
        /// </summary>
        public int ComeLate { get; set; }
        /// <summary>
        /// 早退浮动(分钟)
        /// </summary>
        public int LeftEarly { get; set; }
        /// <summary>
        /// 早打卡浮动(小时)
        /// </summary>
        public double EarlyCard { get; set; }
        /// <summary>
        /// 晚打卡浮动(小时)
        /// </summary>
        public double LateCard { get; set; }
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
