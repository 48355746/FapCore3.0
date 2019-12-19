using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.AspNetCore.Model
{
    public class JqGridData
    {
        public int Total { get; set; }
        public int Page { get; set; }
        public int Records { get; set; }
        public IEnumerable<dynamic> Rows { get; set; }
        public dynamic Userdata { get; set; }
    }
}
