using Dapper.Contrib.Extensions;
using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Infrastructure.Config
{
    /// <summary>
    /// 单据编码规则
    /// </summary>
    [Table("CfgBillCodeRule")]
    public class CfgBillCodeRule : BaseModel
    {
        /// <summary>
        /// 单据实体
        /// </summary>
        public string BillEntity { get; set; }
        /// <summary>
        /// 单据实体 的显性字段MC
        /// </summary>
        [Computed]
        public string BillEntityMC { get; set; }
        /// <summary>
        /// 字段
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// 前缀
        /// </summary>
        public string Prefix { get; set; }
        /// <summary>
        /// 日期格式
        /// </summary>
        public string DateFormat { get; set; }
        /// <summary>
        /// 日期格式 的显性字段MC
        /// </summary>
        [ComputedAttribute]
        public string DateFormatMC { get; set; }
        /// <summary>
        /// 序列号长度
        /// </summary>
        public int SequenceLen { get; set; }
        /// <summary>
        /// 补长符号
        /// </summary>
        public string Symbol { get; set; }
        /// <summary>
        /// 重计数条件
        /// </summary>
        public string ReCountContidion { get; set; }
        /// <summary>
        /// 重计数条件 的显性字段MC
        /// </summary>
        [Computed]
        public string ReCountContidionMC { get; set; }
        /// <summary>
        /// 自定义，临时存储生成的编码
        /// </summary>
        [Computed]
        public string BillCode { get; set; }

    }
}
