using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fap.Hcm.Web.Areas.Time.Models
{
    public class BatchCardViewModel
    {
        public IList<string> DeptUidList { get; } = new List<string>();
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}
