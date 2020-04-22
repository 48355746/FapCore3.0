using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Hcm.Service.Payroll
{
    public class PayrollItem
    {
        public string ItemName { get; set; }
        public object ItemValue { get; set; }
    }
    public class PayrollRow
    {
        public IEnumerable<PayrollItem> PayrollItems { get; set; }
    }
    public class MyPayroll
    {
        /// <summary>
        /// 薪资套
        /// </summary>
        public string CaseUid { get; set; }
        /// <summary>
        /// 薪资项
        /// </summary>
        public IEnumerable<PayrollRow> PayrollRows { get; set; } 
    }
}
