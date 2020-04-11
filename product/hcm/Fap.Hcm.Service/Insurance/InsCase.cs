using Dapper.Contrib.Extensions;
using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Hcm.Service.Insurance
{
    /// <summary>
    /// 保险组
    /// </summary>
    public class InsCase : BaseModel
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
        /// 初始保险年月
        /// </summary>
        public string InitYM { get; set; }
        /// <summary>
        /// 参保险年月
        /// </summary>
        public string InsYM { get; set; }
        /// <summary>
        /// 参保次数
        /// </summary>
        public int InsCount { get; set; }
        /// <summary>
        /// 参保状态
        /// </summary>
        public int InsFlag { get; set; }
        /// <summary>
        /// 不能再改变item
        /// </summary>
        public int Unchanged { get; set; }

    }
}
