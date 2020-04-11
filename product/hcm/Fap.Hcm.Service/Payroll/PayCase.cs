using Dapper.Contrib.Extensions;
using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Hcm.Service.Payroll
{
    /// <summary>
    /// 工资套
    /// </summary>
    public class PayCase : BaseModel
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string CaseCode { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string CaseName { get; set; }
        /// <summary>
        /// 套对应表
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }
        /// <summary>
        /// 币种 的显性字段MC
        /// </summary>
        [Computed]
        public string CurrencyMC { get; set; }
        /// <summary>
        /// 父节点
        /// </summary>
        public string Pid { get; set; }
        /// <summary>
        /// 套对应人员条件
        /// </summary>
        public string EmpCondition { get; set; }
        /// <summary>
        /// 对应公式套
        /// </summary>
        public string FormulaUid { get; set; }
        /// <summary>
        /// 初始薪资年月
        /// </summary>
        public string InitYM { get; set; }
        /// <summary>
        /// 发薪年月
        /// </summary>
        public string PayYM { get; set; }
        /// <summary>
        /// 发放次数
        /// </summary>
        public int PayCount { get; set; }
        /// <summary>
        /// 发放标识
        /// </summary>
        public int PayFlag { get; set; }
        /// <summary>
        /// 不可变
        /// </summary>
        public int Unchanged { get; set; }

    }
}
