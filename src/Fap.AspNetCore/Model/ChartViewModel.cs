using Fap.Core.Infrastructure.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fap.AspNetCore.Model
{
    public class ChartViewModel
    {
        public IEnumerable<Group> Groups { get; set; }
        public IEnumerable<Aggregate> Aggregates { get; set; }
    }
    public class Group
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
        public StatSymbolEnum AggType { get; set; }
    }
}
