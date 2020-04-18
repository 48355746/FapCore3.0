using Dapper.Contrib.Extensions;
using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Hcm.Service.Assess
{
    /// <summary>
    /// 考核指标类型
    /// </summary>
    public class PerfKPIType : BaseModel
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
        /// 考核方案
        /// </summary>
        public string PerfProgram { get; set; }
        /// <summary>
        /// 考核方案 的显性字段MC
        /// </summary>
        [Computed]
        public string PerfProgramMC { get; set; }
        /// <summary>
        /// 权重
        /// </summary>
        public double Weighting { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string TypeNote { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int SortBy { get; set; }
        /// <summary>
        /// 父类型
        /// </summary>
        public string Pid { get; set; }
        /// <summary>
        /// 父类型 的显性字段MC
        /// </summary>
        [Computed]
        public string PidMC { get; set; }

    }

}
