using Fap.Core.Infrastructure.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fap.Hcm.Web.Models
{
    public class ChartViewModel
    {
        public IEnumerable<Group> Groups { get; set; }
        public IEnumerable<Aggregate> Aggregates { get; set; }
    }
    public class Group
    {
        public string Field { get; set; }
        public string Format { get; set; }
    }
    public class Aggregate
    {
        public string Field { get; set; }
        public StatSymbolEnum AggType { get; set; }
    }
}
