using Dapper.Contrib.Extensions;
using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Hcm.Service.Payroll
{
    /// <summary>
    /// 薪资待处理
    /// </summary>
    public class PayToDo : BaseModel
    {
        /// <summary>
        /// 员工
        /// </summary>
        public string EmpUid { get; set; }
        /// <summary>
        /// 员工 的显性字段MC
        /// </summary>
        [Computed]
        public string EmpUidMC { get; set; }
        /// <summary>
        /// 员工编码
        /// </summary>
        public string EmpCode { get; set; }
        /// <summary>
        /// 部门
        /// </summary>
        public string DeptUid { get; set; }
        /// <summary>
        /// 部门 的显性字段MC
        /// </summary>
        [Computed]
        public string DeptUidMC { get; set; }
        /// <summary>
        /// 部门编码
        /// </summary>
        public string DeptCode { get; set; }
        /// <summary>
        /// 工资套
        /// </summary>
        public string CaseUid { get; set; }
        /// <summary>
        /// 工资套 的显性字段MC
        /// </summary>
        [Computed]
        public string CaseUidMC { get; set; }
        /// <summary>
        /// 业务单据
        /// </summary>
        public string TableUid { get; set; }
        /// <summary>
        /// 业务单据 的显性字段MC
        /// </summary>
        [Computed]
        public string TableUidMC { get; set; }
        /// <summary>
        /// 业务日期
        /// </summary>
        public string BizDate { get; set; }
        /// <summary>
        /// 变动处理人
        /// </summary>
        public string OperEmpUid { get; set; }
        /// <summary>
        /// 变动处理人 的显性字段MC
        /// </summary>
        [Computed]
        public string OperEmpUidMC { get; set; }
        /// <summary>
        /// 应用日期
        /// </summary>
        public string OperDate { get; set; }
        /// <summary>
        /// 应用类型
        /// </summary>
        public string OperFlag { get; set; }
        /// <summary>
        /// 应用类型 的显性字段MC
        /// </summary>
        [Computed]
        public string OperFlagMC { get; set; }
        /// <summary>
        /// 业务ID
        /// </summary>
        public string TransID { get; set; }

    }

}
