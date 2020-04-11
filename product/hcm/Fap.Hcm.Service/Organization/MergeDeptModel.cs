using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Hcm.Service.Organization
{
    public class MergeDeptModel
    {
        /// <summary>
        /// 新部门Fid
        /// </summary>
        public string DeptFid { get; set; }
        /// <summary>
        /// 新部门Deptcode
        /// </summary>
        public string DeptCode { get; set; }
        /// <summary>
        /// 合并部门列表
        /// </summary>
        public List<string> MergeFids { get;  } = new List<string>();
    }
}
