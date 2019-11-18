using Dapper.Contrib.Extensions;
using Fap.Core.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fap.Core.Rbac.Model
{
    /// <summary>
    /// 员工主要信息
    /// </summary>
    [Serializable]
    public class Employee : BaseModel
    {
        /// <summary>
        /// 员工编码
        /// </summary>
        public string EmpCode { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string EmpName { get; set; }
        /// <summary>
        /// 姓名拼音
        /// </summary>
        public string EmpPinYin { get; set; }
        /// <summary>
        /// 部门
        /// </summary>
        public string DeptUid { get; set; }
        /// <summary>
        /// 部门编码
        /// </summary>
        public string DeptCode { get; set; }
        /// <summary>
        /// 部门 的显性字段MC
        /// </summary>
        [Computed]
        public string DeptUidMC { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public string Gender { get; set; }
        /// <summary>
        /// 性别 的显性字段MC
        /// </summary>
        [Computed]
        public string GenderMC { get; set; }
        /// <summary>
        /// 身份证号
        /// </summary>
        public string IDCard { get; set; }
        /// <summary>
        /// 年龄
        /// </summary>
        public int Age { get; set; }
        /// <summary>
        /// 出生日期
        /// </summary>
        public string DateOfBirth { get; set; }
        /// <summary>
        /// 员工类别
        /// </summary>
        public string EmpCategory { get; set; }
        /// <summary>
        /// 员工类别 的显性字段MC
        /// </summary>
        [Computed]
        public string EmpCategoryMC { get; set; }
        /// <summary>
        /// 入职日期
        /// </summary>
        public string EntryDate { get; set; }
        /// <summary>
        /// 离职时间
        /// </summary>
        public string LeaveDate { get; set; }
        /// <summary>
        /// 手机
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Mailbox { get; set; }

        /// <summary>
        /// 职务
        /// </summary>
        public string EmpJob { get; set; }
        /// <summary>
        /// 职务 的显性字段MC
        /// </summary>
        [Computed]
        public string EmpJobMC { get; set; }
        /// <summary>
        /// 职务级别
        /// </summary>
        public string JobGrade { get; set; }
        /// <summary>
        /// 员工状态
        /// </summary>
        public string EmpStatus { get; set; }
        /// <summary>
        /// 员工状态 的显性字段MC
        /// </summary>
        [Computed]
        public string EmpStatusMC { get; set; }
        /// <summary>
        /// 直属领导
        /// </summary>
        public string Leadership { get; set; }
        /// <summary>
        /// 照片
        /// </summary>
        public string EmpPhoto { get; set; }
        /// <summary>
        /// 职位
        /// </summary>
        public string EmpPosition { get; set; }
        /// <summary>
        /// 职位 的显性字段MC
        /// </summary>
        [Computed]
        public string EmpPositionMC { get; set; }

        /// <summary>
        /// 主职
        /// </summary>
        public int IsMainJob { get; set; }
        /// <summary>
        /// 成本中心
        /// </summary>
        public string CostCenter { get; set; }
        [Computed]
        public string CostCenterMC { get; set; }
        /// <summary>
        /// 登录名
        /// </summary>
        public string LoginName { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int SortBy { get; set; }

    }
}
