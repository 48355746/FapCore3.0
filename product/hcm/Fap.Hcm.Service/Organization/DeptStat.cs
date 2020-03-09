using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Hcm.Service.Organization
{
    /// <summary>
    /// 部门人数统计
    /// </summary>
    public class DeptStat
    {
        /// <summary>
        /// 部门
        /// </summary>
        public string DeptUid { get; set; }
        /// <summary>
        /// 人数
        /// </summary>
        public int Num { get; set; }
    }

}
