using Fap.AspNetCore.Controls.JqGrid;
using Fap.Core.Infrastructure.Query;
using System.Collections.Generic;

namespace Fap.AspNetCore.ViewModel
{
    public class JqGridViewModel : IViewModel
    {
        /// <summary>
        /// 控件名称
        /// </summary>
        //public string CtrlName { get; set; }
        /// <summary>
        /// 临时数据存放
        /// </summary>
        public Dictionary<string, string> TempData { get; set; } = new Dictionary<string, string>();        
        /// <summary>
        /// jqgridid,默认等于tablename
        /// </summary>
        public string JqgridId { get; set; }
        /// <summary>
        /// 是否多选
        /// </summary>
        public bool IsMulti { get; set; }
        /// <summary>
        /// 表格标题
        /// </summary>
        public string Title { get; set; }        
        /// <summary>
        /// 查询设置
        /// </summary>
        public QuerySet QuerySet { get; set; }
        
        /// <summary>
        /// 是否包含操作列
        /// </summary>
        public bool HasOperCol { get; set; }
        /// <summary>
        /// 操作列
        /// </summary>
        public List<Column> OperColumns { get; set; }
        /// <summary>
        /// 自定义列
        /// </summary>
        public List<Column> CustomColumns { get; set; }
    }
}