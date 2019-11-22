using Fap.Core.Infrastructure.Query;
using System.Collections.Generic;

namespace Fap.AspNetCore.ViewModel
{
    public class JqGridPostData
    {
        //int page, int rows, string sidx, string sord,string tablename="",string pkey="", string postdata="", string filters = ""
        /// <summary>
        /// 当前页数
        /// </summary>
        public int Page { get; set; }
        /// <summary>
        /// 每页行数
        /// </summary>
        public int Rows { get; set; }
        /// <summary>
        /// 排序列名称
        /// </summary>
        public string Sidx { get; set; }
        /// <summary>
        /// 排序方向  asc desc
        /// </summary>
        public string Sord { get; set; }
        /// <summary>
        /// 查询设置序列化
        /// </summary>
        public QuerySet QuerySet { get; set; }
        /// <summary>
        /// 过滤字段
        /// </summary>
        public string Filters { get; set; }
        /// <summary>
        /// 是否包含操作列，默认为第一列
        /// </summary>
        public bool HasOperCol { get; set; }
       
        /// <summary>
        /// 初始值，用于form增加的时候的默认值
        /// </summary>
        public List<InitValue> InitValues { get; set; }
        /// <summary>
        /// 时间点， 只用于查询历史信息
        /// </summary>
        public string TimePoint { get; set; }
        /// <summary>
        /// 页面级条件，适用于页面上的条件过滤，jqgird的查询取页面级条件的子集
        /// </summary>
        public string PageCondition { get; set; }
    }
   
    /// <summary>
    /// 初始值
    /// </summary>
    public class InitValue
    {
        /// <summary>
        /// 字段名称
        /// </summary>
        public string FieldName { get; set; }
        /// <summary>
        /// 字段值
        /// </summary>
        public string FieldValue { get; set; }
    }
}