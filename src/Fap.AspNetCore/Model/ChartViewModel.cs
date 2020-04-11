using Fap.Core.Infrastructure.Query;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Fap.AspNetCore.Model
{
    public class ChartViewModel
    {
        public IEnumerable<GroupBy> Groups { get; set; }
        public IEnumerable<Aggregate> Aggregates { get; set; }
    }
    public class GroupBy
    {
        /// <summary>
        /// 字段
        /// </summary>
        public string Field { get; set; }
        /// <summary>
        /// 别名
        /// </summary>
        public string Alias { get; set; }
        /// <summary>
        /// 格式（year,year-month）
        /// </summary>
        public string Format { get; set; }
    }
    public class Aggregate
    {
        /// <summary>
        /// 字段
        /// </summary>
        public string Field { get; set; }
        /// <summary>
        /// 别名
        /// </summary>
        public string Alias { get; set; }
        public AggregateEnum AggType { get; set; }
        /// <summary>
        /// 图表类型
        /// </summary>
        public string ChartType { get; set; }
    }
    public enum AggregateEnum
    {
        [Description("分类计数")]
        CCOUNT,
        [Description("计数")]
        COUNT,
        [Description("最大值")]
        MAX,
        [Description("最小值")]
        MIN,
        [Description("平均值")]
        AVG,
        [Description("合计")]
        SUM
    }

    public class ChartResult
    {
        public IEnumerable<dynamic> DataSet { get; set; }
        public IEnumerable<Aggregate> Aggregates { get; set; }
    }
}
