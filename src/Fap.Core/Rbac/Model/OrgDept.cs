using Dapper.Contrib.Extensions;
using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fap.Core.Rbac.Model
{
    /// <summary>
    /// 组织部门表
    /// </summary>
    [Serializable]
    public class OrgDept : BaseModel
    {
        /// <summary>
        /// 部门编码
        /// </summary>
        public string DeptCode { get; set; }
        /// <summary>
        /// 部门名称
        /// </summary>
        public string DeptName { get; set; }
        /// <summary>
        /// 部门全称
        /// </summary>
        public string FullName { get; set; }
        /// <summary>
        /// 父部门编码
        /// </summary>
        public string PCode { get; set; }
        /// <summary>
        /// 父部门
        /// </summary>
        public string Pid { get; set; }
        /// <summary>
        /// 父部门 的显性字段MC
        /// </summary>
        [Computed]
        public string PidMC { get; set; }
        /// <summary>
        /// 部门类型
        /// </summary>
        public string DeptType { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string DeptNote { get; set; }
        /// <summary>
        /// 层级
        /// </summary>
        public int TreeLevel { get; set; }
        /// <summary>
        /// 是否末级
        /// </summary>
        public int IsFinal { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int DeptOrder { get; set; }
        /// <summary>
        /// 附件
        /// </summary>
        public string Attachment { get; set; }
        /// <summary>
        /// 部门经理
        /// </summary>
        public string DeptManager { get; set; }
        /// <summary>
        /// 部门经理 的显性字段MC
        /// </summary>
        [Computed]
        public string DeptManagerMC { get; set; }
        /// <summary>
        /// 负责人
        /// </summary>
        public string Director { get; set; }
        /// <summary>
        /// 负责人 的显性字段MC
        /// </summary>
        [Computed]
        public string DirectorMC { get; set; }

        /// <summary>
        /// 子部门
        /// </summary>
        [Computed]
        public IEnumerable<OrgDept> Children { get; set; }
        /// <summary>
        /// 部门人数
        /// </summary>

        [Computed]
        public int EmpNum { get; set; }
        /// <summary>
        /// 权限【判断对这个部门拥有部分权限】
        /// </summary>
        [Computed]
        public bool HasPartPower { get; set; }
        /// <summary>
        /// 编制人数,用于在编校验
        /// </summary>
        public int PreparationNums { get; set; }
        /// <summary>
        /// 用于第三方系统的部门同步使用
        /// </summary>
        public string ThirdDeptId { get; set; }
    }


}
