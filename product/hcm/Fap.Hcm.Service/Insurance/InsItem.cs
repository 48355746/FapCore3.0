using Dapper.Contrib.Extensions;
using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Hcm.Service.Insurance
{
    /// <summary>
    /// 参保项目
    /// </summary>
    public class InsItem : BaseModel
    {
        /// <summary>
        /// 保险组
        /// </summary>
        public string CaseUid { get; set; }
        /// <summary>
        /// 保险组 的显性字段MC
        /// </summary>
        [Computed]
        public string CaseUidMC { get; set; }
        /// <summary>
        /// 项目
        /// </summary>
        public string ColumnUid { get; set; }
        /// <summary>
        /// 项目 的显性字段MC
        /// </summary>
        [Computed]
        public string ColumnUidMC { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int ItemSort { get; set; }
        /// <summary>
        /// 可见
        /// </summary>
        public int ShowAble { get; set; }        
        /// <summary>
        /// 变动影响字段
        /// </summary>
        public int TransEnable { get; set; }
        /// <summary>
        /// 展示项
        /// </summary>
        public int ShowCard { get; set; }

    }

}
