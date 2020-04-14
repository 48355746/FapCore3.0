using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fap.Hcm.Web.Areas.Time.Models
{
    public class HolidayViewModel
    {
        public string CaseUid { get; set; }
        public List<string> Holidays { get; } = new List<string>();
    }
}
