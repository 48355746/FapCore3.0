using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Hcm.Service.Organization
{
    /// <summary>
    /// 应用于GoJs
    /// </summary>
    public class OrgChartNode2
    {
        public string Key { get; set; }
        public string Parent { get; set; }
        /// <summary>
        /// 扩展 部门编码
        /// </summary>
        public string DeptCode { get; set; }
        /// <summary>
        /// 部门名称
        /// </summary>
        public string DeptName { get; set; }
        /// <summary>
        /// 扩展，部门经理
        /// </summary>
        public string ManagerUid { get; set; }
        public string ManagerName { get; set; }
        /// <summary>
        /// 扩展，部门负责人
        /// </summary>
        public string DirectorUid { get; set; }
        public string DirectorName { get; set; }
        /// <summary>
        /// 扩展，部门实际人数
        /// </summary>
        public string DeptNum { get; set; }
        /// <summary>
        /// 扩展，部门计划人数
        /// </summary>
        public string DeptPlanNum { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Comments { get; set; }
        /// <summary>
        /// 机构类型，可能是岗位
        /// dept，position
        /// </summary>
        public string OrgType { get; set; }
    }
}
