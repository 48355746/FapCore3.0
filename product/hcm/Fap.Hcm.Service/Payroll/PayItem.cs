using Dapper.Contrib.Extensions;
using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Hcm.Service.Payroll
{
    /// <summary>
    /// 薪资项
    /// </summary>
    public class PayItem : BaseModel
    {
        /// <summary>
        /// 工资套
        /// </summary>
        public string CaseUid { get; set; }
        /// <summary>
        /// 工资套 的显性字段MC
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
        /// 类型
        /// </summary>
        public string ItemType { get; set; }
        /// <summary>
        /// 类型 的显性字段MC
        /// </summary>
        [Computed]
        public string ItemTypeMC { get; set; }
        /// <summary>
        /// 累加方式
        /// </summary>
        public string AddUpWay { get; set; }
        /// <summary>
        /// 累加方式 的显性字段MC
        /// </summary>
        [Computed]
        public string AddUpWayMC { get; set; }
        /// <summary>
        /// 引入表
        /// </summary>
        public string ImportTable { get; set; }
        /// <summary>
        /// 引入列
        /// </summary>
        public string ImportColumn { get; set; }
        /// <summary>
        /// 引入条件
        /// </summary>
        public string ImportWhere { get; set; }
        /// <summary>
        /// 变动影响字段
        /// </summary>
        public int TransEnable { get; set; }
        /// <summary>
        /// 工资条展示项
        /// </summary>
        public int ShowCard { get; set; }

    }
    public class PayItemAddData
    {
        public IEnumerable<FapColumn> Items { get; set; }
        public object JsonData { get; set; }
    }
}
