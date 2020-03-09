using System.Collections.Generic;

namespace Fap.AspNetCore.ViewModel
{
    public class JsTreeViewModel : IViewModel
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
        /// tree   id
        /// </summary>
        public string JsTreeId { get; set; }
        /// <summary>
        /// 是否多选
        /// </summary>
        public bool IsMulti { get; set; }
        /// <summary>
        /// 树标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 树数据源，json格式
        /// </summary>
        public string JsonData { get; set; }
    }
}