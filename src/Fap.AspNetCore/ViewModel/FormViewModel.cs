using Fap.AspNetCore.Controls.DataForm;
using Fap.Core.Infrastructure.Query;
using System.Collections.Generic;

namespace Fap.AspNetCore.ViewModel
{
    public class FormViewModel : IViewModel
    {
        /// <summary>
        /// 表单ID
        /// </summary>
        public string FormId { get; set; }
        /// <summary>
        /// 表单状态
        /// </summary>
        public FormStatus FormStatus { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 主键
        /// </summary>
        public string PK { get; set; }
        /// <summary>
        /// 简单查询设置
        /// </summary>
        //public SimpleQueryOption SimpleQueryOption { get; set; }
        /// <summary>
        /// 关联查询设置
        /// </summary>
        public QuerySet QueryOption { get; set; }
        /// <summary>
        /// 表单初始化默认值
        /// </summary>
        public Dictionary<string, string> DefaultData { get; set; } = new Dictionary<string, string>();
        /// <summary>
        /// 子表默认值列表
        /// </summary>
        public IEnumerable<SubTableDefaultValue> SubDefaultDataList { get; set; } = new List<SubTableDefaultValue>();
        /// <summary>
        /// 临时数据存储
        /// </summary>
        public Dictionary<string, object> TempData { get; set; } = new Dictionary<string, object>();
        
    }
    public class SubTableDefaultValue
    {
        /// <summary>
        /// 子表名称
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 默认值
        /// </summary>
        public Dictionary<string,object> Data { get; set; }
    }
}