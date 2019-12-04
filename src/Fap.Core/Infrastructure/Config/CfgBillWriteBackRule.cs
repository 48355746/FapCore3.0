using Dapper.Contrib.Extensions;
using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Infrastructure.Config
{
    /// <summary>
    /// 单据回写规则
    /// </summary>
    [Table("CfgBillWriteBackRule")]
    public class CfgBillWriteBackRule :BaseModel
    {
        /// <summary>
        /// 单据实体
        /// </summary>
        public string DocEntityUid { get; set; }
        /// <summary>
        /// 单据实体 的显性字段MC
        /// </summary>
        [ComputedAttribute]
        public string DocEntityUidMC { get; set; }
        /// <summary>
        /// 业务实体
        /// </summary>
        public string BizEntityUid { get; set; }
        /// <summary>
        /// 业务实体 的显性字段MC
        /// </summary>
        [ComputedAttribute]
        public string BizEntityUidMC { get; set; }
        /// <summary>
        /// 关联关系
        /// </summary>
        public string Association { get; set; }
        /// <summary>
        /// 字段映射
        /// </summary>
        public string FieldMapping { get; set; }
        /// <summary>
        /// 自定义Sql
        /// </summary>
        public string CustomSql { get; set; }

        /// <summary>
        /// 回调配置
        /// </summary>
        public string CallBackClass { get; set; }
        /// <summary>
        /// 提醒到工资
        /// </summary>
        public int IsNotifyPayroll { get; set; }
        /// <summary>
        /// 人员类别过滤
        /// </summary>
        public string EmpCategoryFilter { get; set; }


    }
}
