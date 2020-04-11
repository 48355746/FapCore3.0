using Dapper.Contrib.Extensions;
using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Hcm.Service.Insurance
{
    /// <summary>
    /// 参保记录
    /// </summary>
    public class InsRecord :BaseModel
    {
        /// <summary>
        /// 保险组
        /// </summary>
        public string CaseUid { get; set; }
        /// <summary>
        /// 保险组 的显性字段MC
        /// </summary>
        [Computed]
        public string CaseUidMC { get; set; }        
        /// <summary>
        /// 保险年月
        /// </summary>
        public string InsYM { get; set; }
        /// <summary>
        /// 参保次数
        /// </summary>
        public int InsCount { get; set; }
        /// <summary>
        /// 参保标识
        /// </summary>
        public int InsFlag { get; set; }
        /// <summary>
        /// 操作人
        /// </summary>
        public string InsEmpUid { get; set; }
        /// <summary>
        /// 操作人 的显性字段MC
        /// </summary>
        [Computed]
        public string InsEmpUidMC { get; set; }
        /// <summary>
        /// 参保时间
        /// </summary>
        public string InsDate { get; set; }

    }
}
