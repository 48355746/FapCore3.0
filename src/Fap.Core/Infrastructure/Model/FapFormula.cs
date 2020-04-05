using Dapper.Contrib.Extensions;
using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Infrastructure.Model
{
    /// <summary>
    /// 公式设置
    /// </summary>
    public class FapFormula : BaseModel
    {
        /// <summary>
        /// 表
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 列
        /// </summary>
        public string ColName { get; set; }
        /// <summary>
        /// 列描述
        /// </summary>
        public string ColComment { get; set; }
        /// <summary>
        /// 公式
        /// </summary>
        public string FmuContent { get; set; }
        /// <summary>
        /// 公式描述
        /// </summary>
        public string FmuDesc { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int OrderBy { get; set; }
        /// <summary>
        /// 启用
        /// </summary>
        public int Enabled { get; set; }
        /// <summary>
        /// 成功执行
        /// </summary>
        public int SuccessExec { get; set; }
        /// <summary>
        /// 公式套
        /// </summary>
        public string FcUid { get; set; }
        /// <summary>
        /// 公式套 的显性字段MC
        /// </summary>
        [Computed]
        public string FcUidMC { get; set; }

    }

    public class FapFormulaItems
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 公式套
        /// </summary>
        public string FcUid { get; set; }
        /// <summary>
        /// 公式项
        /// </summary>
        public IEnumerable<FapFormula> Formulas { get; set; }

    }
}
