using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Hcm.Service.Payroll
{
    /// <summary>
    /// 薪资套项
    /// </summary>
    public class PayCaseItem
    {
        public string Fid { get; set; }
        public string ColName { get; set; }
        public string ColComment { get; set; }
        public bool IsSelected { get; set; }
    }
}
