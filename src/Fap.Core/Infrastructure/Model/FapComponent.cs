using Dapper.Contrib.Extensions;
using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Infrastructure.Model
{
    /// <summary>
    /// 组件库
    /// </summary>
    public class FapComponent : BaseModel
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 组件类型
        /// </summary>
        public string ComponentType { get; set; }
        /// <summary>
        /// 组件类型 的显性字段MC
        /// </summary>
        [ComputedAttribute]
        public string ComponentTypeMC { get; set; }
        /// <summary>
        /// 表格表名
        /// </summary>
        public string GridTableName { get; set; }
        /// <summary>
        /// 表格表名 的显性字段MC
        /// </summary>
        [ComputedAttribute]
        public string GridTableNameMC { get; set; }
        /// <summary>
        /// 返回的字段
        /// </summary>
        public string ReturnFields { get; set; }
        /// <summary>
        /// 显示字段
        /// </summary>
        public string GridDisplayFields { get; set; }
        /// <summary>
        /// 是否预置
        /// </summary>
        public int IsPreset { get; set; }
        /// <summary>
        /// 树表名
        /// </summary>
        public string TreeTableName { get; set; }
        /// <summary>
        /// 树过滤条件
        /// </summary>
        public string TreeFilterCondition { get; set; }
        /// <summary>
        /// 树显示字段
        /// </summary>
        public string TreeDisplayField { get; set; }
        /// <summary>
        /// 树节点图标
        /// </summary>
        public string TreeNodeIcon { get; set; }
        /// <summary>
        /// 表条件
        /// </summary>
        public string TableCondition { get; set; }
        /// <summary>
        /// 树标题
        /// </summary>
        public string TreeTitle { get; set; }
        /// <summary>
        /// 表格标题
        /// </summary>
        public string GridTitle { get; set; }
        /// <summary>
        /// 树排序
        /// </summary>
        public string TreeOrder { get; set; }
        /// <summary>
        /// 表格排序
        /// </summary>
        public string GridOrder { get; set; }
        /// <summary>
        /// 树条件
        /// </summary>
        public string TreeCondition { get; set; }


    }
}
