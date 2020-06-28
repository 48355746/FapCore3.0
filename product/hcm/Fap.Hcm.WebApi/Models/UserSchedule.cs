using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Fap.Hcm.WebApi.Models
{
    /// <summary>
    /// 用户日程
    /// </summary>
    public class UserSchedule
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [Required]
        public string UserId { get; set; }
        /// <summary>
        /// 用户日程
        /// </summary>
        [Required]
        public IEnumerable<Schedule> Schedules { get; set; }
    }
    /// <summary>
    /// 日程
    /// </summary>
    public class Schedule
    {
        /// <summary>
        /// 开始时间
        /// </summary>
        [Required]
        public DateTime StartDateTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        [Required]
        public DateTime EndDateTime { get; set; }
    }
}
