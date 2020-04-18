using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Hcm.Service.Assess
{
    public class ExaminerViewModel
    {
        /// <summary>
        /// 考核方案
        /// </summary>
        public string SchemeUid { get; set; }
      
        /// <summary>
        /// 考核对象
        /// </summary>
        public List<string> Objectives { get; } = new List<string>();
        /// <summary>
        /// 是否部门内考核
        /// </summary>
        public bool IsOrgDept { get; set; }
        /// <summary>
        /// 部门考核方式名称
        /// </summary>
        public string DeptModelName { get; set; }
        /// <summary>
        /// 部门内考核权重
        /// </summary>
        public double DeptWeights { get; set; }
        /// <summary>
        /// 是否直属领导考核
        /// </summary>
        public bool IsLeaderShip { get; set; }
        /// <summary>
        /// 部门考核方式名称
        /// </summary>
        public string LeadershipModelName { get; set; }
        /// <summary>
        /// 直属领导考核权重
        /// </summary>
        public double LeaderShipWeights { get; set; }
        /// <summary>
        /// 是否自定义考核
        /// </summary>
        public bool IsCustom { get; set; }
        /// <summary>
        /// 部门考核方式名称
        /// </summary>
        public string CustomModelName { get; set; }
        /// <summary>
        /// 自定义考核权重
        /// </summary>
        public double CustomWeights { get; set; }
        /// <summary>
        /// 自定义考核人
        /// </summary>
        public List<string> CustomExaminers { get; } = new List<string>();
    }
}
