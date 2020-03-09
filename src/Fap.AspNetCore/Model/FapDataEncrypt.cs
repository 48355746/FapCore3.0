using Dapper.Contrib.Extensions;
using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.AspNetCore.Model
{
    /// <summary>
    /// 数据加密
    /// </summary>
    public class FapDataEncrypt : BaseModel
    {
        /// <summary>
        /// 实体
        /// </summary>
        public string TableUid { get; set; }
        /// <summary>
        /// 实体 的显性字段MC
        /// </summary>
        [ComputedAttribute]
        public string TableUidMC { get; set; }
        /// <summary>
        /// 属性列
        /// </summary>
        public string ColumnUid { get; set; }
        /// <summary>
        /// 属性列 的显性字段MC
        /// </summary>
        [ComputedAttribute]
        public string ColumnUidMC { get; set; }
        /// <summary>
        /// 属性
        /// </summary>
        public string ColumnName { get; set; }
        /// <summary>
        /// 数据UID
        /// </summary>
        public string FidValue { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
        /// <summary>
        /// 替换字符
        /// </summary>
        public string ReplaceChart { get; set; }

    }
}
