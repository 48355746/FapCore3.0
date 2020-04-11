using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Fap.Hcm.Service.Payroll
{
    public class InitDataViewModel
    {
        /// <summary>
        /// 薪资套
        /// </summary>
        [Required]
        public string CaseUid { get; set; }
        /// <summary>
        /// 薪资年月
        /// </summary>
        [Required]
        public string InitYm { get; set; }
        /// <summary>
        /// 保留薪资项
        /// </summary>
        public string ReservedItems { get; set; }
        /// <summary>
        /// 记录
        /// </summary>
        [Required]
        public string RecordUid { get; set; }
    }
}
