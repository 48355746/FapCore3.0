using Fap.Core.Rbac.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Hcm.Service.Payroll
{
    public class PayGapEmployee
    {
        /// <summary>
        /// 增加的员工
        /// </summary>
        public IEnumerable<Employee> AddedList { get; set; }
        /// <summary>
        /// 移除的员工
        /// </summary>
        public IEnumerable<Employee> RemovedList { get; set; }
        /// <summary>
        /// 更新的员工
        /// </summary>
        public IEnumerable<Employee> UpdatedList { get; set; }
    }
}
