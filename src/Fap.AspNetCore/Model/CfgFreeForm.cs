using Dapper.Contrib.Extensions;
using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.AspNetCore.Model
{
    /// <summary>
    /// 自由表单
    /// </summary>
    [Table("CfgFreeForm")]
    public class CfgFreeForm :BaseModel
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string FFCode { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string FFName { get; set; }
        /// <summary>
        /// 单据
        /// </summary>
        public string BillTable { get; set; }
        /// <summary>
        /// 单据 的显性字段MC
        /// </summary>
        [ComputedAttribute]
        public string BillTableMC { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string FFContent { get; set; }
        /// <summary>
        /// 可用
        /// </summary>
        public int Enabled { get; set; }
        /// <summary>
        /// 打印模板
        /// </summary>
        public int IsPrint { get; set; }


    }
}
