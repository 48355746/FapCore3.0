using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Fap.Hcm.WebApi.Models
{
    /// <summary>
    /// 活动
    /// </summary>
    public class Activity
    {
        /// <summary>
        /// 活动名称
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// 开始日期（日期范围yyyy-MM-dd）
        /// </summary>
        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        /// <summary>
        /// 结束日期（日期范围yyyy-MM-dd）
        /// </summary>
        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
        /// <summary>
        /// 开始时间（时间范围HH:mm）
        /// </summary>
        [Required]
        public string StartTime { get; set; }
        /// <summary>
        /// 结束时间（时间范围HH:mm）
        /// </summary>
        [Required]
        public string EndTime { get; set; }
        /// <summary>
        /// 活动时长(小时)
        /// </summary>
        [Required]
        public double Duration { get; set; }
        /// <summary>
        /// 用户日程
        /// </summary>
        public IEnumerable<UserSchedule> UserSchedules { get; set; }
    }
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
        /// 开始时间(yyyy-MM-dd HH:mm)
        /// </summary>
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime StartDateTime { get; set; }
        /// <summary>
        /// 结束时间(yyyy-MM-dd HH:mm)
        /// </summary>
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime EndDateTime { get; set; }
        /// <summary>
        ///推荐级别
        /// </summary>        
        public bool Priority { get; set; }
    }
    /// <summary>
    ///空闲日程
    /// </summary>
    public class IdleSchedule
    {
        /// <summary>
        /// 开始时间(yyyy-MM-dd HH:mm)
        /// </summary>
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime StartDateTime { get; set; }
        /// <summary>
        /// 结束时间(yyyy-MM-dd HH:mm)
        /// </summary>
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime EndDateTime { get; set; }        
    }
}
