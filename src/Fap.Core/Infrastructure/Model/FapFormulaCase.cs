using Dapper.Contrib.Extensions;
using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Infrastructure.Model
{
    /// <summary>
    /// 公式套
    /// </summary>
    public class FapFormulaCase : BaseModel
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string FcName { get; set; }
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 表名 的显性字段MC
        /// </summary>
        [Computed]
        public string TableNameMC { get; set; }

    }
}
