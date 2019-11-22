using Fap.Core.Infrastructure.Query;
using System.Collections.Generic;

namespace Fap.AspNetCore.ViewModel
{
    /// <summary>
    /// 树表viewmodel
    /// </summary>
    public class TreeGridViewModel:IViewModel
    {
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
        ///控件ID
        /// </summary>
        public string TreeGridId { get; set; }
        /// <summary>
        /// 树标题
        /// </summary>
        public string TreeTitle { get; set; }
        /// <summary>
        /// 表格标题
        /// </summary>
        public string GridTitle { get; set; }

        /// <summary>
        /// 是否多选
        /// </summary>
        public bool IsMulti { get; set; }
        /// <summary>
        /// 树数据源，json格式
        /// </summary>
        public string JsonData { get; set; }
        /// <summary>
        /// 树过滤条件
        /// </summary>
        public string TreeFilterCondition { get; set; }
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

        public QuerySet SimpleQueryOption
        {
            get { return _simpleQueryOption; }
            set
            {
                _simpleQueryOption = value;
                //TableName = value.TableName;
            }
        }
        /// <summary>
        /// 关联查询设置
        /// </summary>
        //public QueryOption QueryOption { get; set; }
        
    }
}
