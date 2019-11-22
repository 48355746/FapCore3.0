using Dapper.Contrib.Extensions;
using Fap.Core.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.AspNetCore.Model
{
    [Table("CfgQueryProgram")]
    public class CfgQueryProgram :BaseModel
    {
        /// <summary>
        /// 方案名称
        /// </summary>
        public string ProgramName { get; set; }
        /// <summary>
        /// 表名称
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 查询条件
        /// </summary>
        public string QueryCondition { get; set; }
        /// <summary>
        /// 所有人
        /// </summary>
        public string Owner { get; set; }
        /// <summary>
        /// 所有人 的显性字段MC
        /// </summary>
        [ComputedAttribute]
        public string OwnerMC { get; set; }
        /// <summary>
        /// 是否全局
        /// </summary>
        public int IsGlobal { get; set; }
        /// <summary>
        /// 可使用人
        /// </summary>
        public string UseEmployee { get; set; }
        /// <summary>
        /// 可使用人 的显性字段MC
        /// </summary>
        [ComputedAttribute]
        public string UseEmployeeMC { get; set; }

    }
}
