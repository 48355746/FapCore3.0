using Dapper.Contrib.Extensions;
using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Hcm.Service.Payroll
{
    /// <summary>
    /// 发放记录
    /// </summary>
    public class PayRecord :BaseModel
    {
        /// <summary>
        /// 薪资套
        /// </summary>
        public string CaseUid { get; set; }
        /// <summary>
        /// 薪资套 的显性字段MC
        /// </summary>
        [Computed]
        public string CaseUidMC { get; set; }        
        /// <summary>
        /// 薪资年月
        /// </summary>
        public string PayYM { get; set; }
        /// <summary>
        /// 发放次数
        /// </summary>
        public int PayCount { get; set; }
        /// <summary>
        /// 发放标识
        /// </summary>
        public int PayFlag { get; set; }
        /// <summary>
        /// 发放人
        /// </summary>
        public string PayEmpUid { get; set; }
        /// <summary>
        /// 发放人 的显性字段MC
        /// </summary>
        [Computed]
        public string PayEmpUidMC { get; set; }
        /// <summary>
        /// 发放时间
        /// </summary>
        public string PayDate { get; set; }

    }

}
