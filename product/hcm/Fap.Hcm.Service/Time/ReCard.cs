using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fap.Hcm.Service.Time
{
    /// <summary>
    /// 补签卡Model
    /// </summary>
    public class ReCard
    {
        /// <summary>
        /// 补签部门
        /// </summary>
        public List<string> DeptUids { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public string StartDate { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public string EndDate { get; set; }
        
    }
}
