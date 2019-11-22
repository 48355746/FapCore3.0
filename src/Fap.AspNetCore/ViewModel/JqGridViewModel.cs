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
        public string CtrlName { get; set; }
        private Dictionary<string, string> _tempData = new Dictionary<string, string>();
        /// <summary>
        /// 临时数据存放
        /// </summary>
        public Dictionary<string, string> TempData
        {
            get { return _tempData; }
            set { _tempData = value; }
        }
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
        /// 向后台传输的数据，加密的json
        /// </summary>
        public PostData PostData { get; set; }
        /// <summary>
        /// 更新数据的时候使用
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 简单查询设置
        /// </summary>
        private QuerySet _simpleQueryOption;

        public QuerySet QueryOption
        {
            get { return _simpleQueryOption; }
            set
            {
                _simpleQueryOption = value;
                TableName =value.TableName;
            }
        }
        //public SimpleQueryOption SimpleQueryOption { get; set; }
        /// <summary>
        /// 关联查询设置
        /// </summary>
        //public QueryOption QueryOption { get; set; }
        /// <summary>
        /// 操作列
        /// </summary>
        public List<Column> OperColumns { get; set; }
        /// <summary>
        /// 自定义列
        /// </summary>
        public List<Column> CustomColumns { get; set; }
    }
    public class PostData
    {
        /// <summary>
        /// QuerySet，查询设置
        /// </summary>
        public QuerySet QuerySet { get; set; }

        /// <summary>
        /// 是否包含操作列
        /// </summary>
        public bool HasOperCol { get; set; }
        
    }
}