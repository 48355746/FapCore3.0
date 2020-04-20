using Dapper.Contrib.Extensions;
using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Hcm.Service.Assess
{
    /// <summary>
    /// 考核指标
    /// </summary>
    public class PerfKPIs : BaseModel
    {
        /// <summary>
        /// 考核方案
        /// </summary>
        public string ProgramUid { get; set; }
        /// <summary>
        /// 考核方案 的显性字段MC
        /// </summary>
        [Computed]
        public string ProgramUidMC { get; set; }
        /// <summary>
        /// 指标类型
        /// </summary>
        public string KpiType { get; set; }
        /// <summary>
        /// 指标类型 的显性字段MC
        /// </summary>
        [Computed]
        public string KpiTypeMC { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string KpiName { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string KpiNote { get; set; }
        /// <summary>
        /// 权重
        /// </summary>
        public double Weights { get; set; }
        /// <summary>
        /// 指标性质
        /// </summary>
        public string KpiProperty { get; set; }
        /// <summary>
        /// 指标性质 的显性字段MC
        /// </summary>
        [ComputedAttribute]
        public string KpiPropertyMC { get; set; }
        /// <summary>
        /// 评分方式
        /// </summary>
        public string ScoreModel { get; set; }
        /// <summary>
        /// 评分方式实体
        /// </summary>
        [Computed]
        public PerfScoreModel ObjScoreModel { get; set; }
        /// <summary>
        /// 评分方式 的显性字段MC
        /// </summary>
        [Computed]
        public string ScoreModelMC { get; set; }
        /// <summary>
        /// 工作目标
        /// </summary>
        public string Target { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int SortBy { get; set; }
        /// <summary>
        /// 完成情况
        /// </summary>
        public string Completeness { get; set; }

    }

}
