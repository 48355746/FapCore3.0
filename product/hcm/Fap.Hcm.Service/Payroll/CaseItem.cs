using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Hcm.Service.Payroll
{
    /// <summary>
    /// 组项
    /// </summary>
    public class CaseItem
    {
        public string Fid { get; set; }
        public string ColName { get; set; }
        public string ColComment { get; set; }
        public bool IsSelected { get; set; }
    }
}
