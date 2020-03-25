using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Hcm.Service.Time
{
    /// <summary>
    /// 申请单对应每一天的结果
    /// </summary>
    public class ApplyInfo
    {
        public string ApplyDate { get; set; }
        public double Hours { get; set; }
        public double Days { get; set; }
    }
}
