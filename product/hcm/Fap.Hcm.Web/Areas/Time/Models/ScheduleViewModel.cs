using Fap.AspNetCore.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Fap.Hcm.Web.Areas.Time.Models
{
    /// <summary>
    /// 排班
    /// </summary>
    public class ScheduleViewModel
    {
        /// <summary>
        /// 班次
        /// </summary>
        [Required]
        public string ShiftUid { get; set; }
        /// <summary>
        /// 假日套
        /// </summary>
        [Required]
        public string HolidayUid { get; set; }
        /// <summary>
        /// 开始日期
        /// </summary>
        [Required]
        public string StartDate { get; set; }
        /// <summary>
        /// 结束日期
        /// </summary>
        [Required]
        public string EndDate { get; set; }
        /// <summary>
        /// 员工条件
        /// </summary>
        [Required]
        public JqGridPostData PostData { get; set; }
    }
}
