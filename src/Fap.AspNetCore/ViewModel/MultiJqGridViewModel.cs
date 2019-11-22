using System.Collections.Generic;

namespace Fap.AspNetCore.ViewModel
{
    /// <summary>
    /// 多个JqGridViewModel
    /// </summary>
    public class MultiJqGridViewModel:IViewModel
    {
        private Dictionary<string, string> _tempData = new Dictionary<string, string>();
        private Dictionary<string, JqGridViewModel> _dicJqGridViewModels = new Dictionary<string, JqGridViewModel>();
        /// <summary>
        /// 临时数据存放
        /// </summary>
        public Dictionary<string, string> TempData
        {
            get { return _tempData; }
            set { _tempData = value; }
        }
        public Dictionary<string, JqGridViewModel> JqGridViewModels
        {
            get { return _dicJqGridViewModels; }
            set { _dicJqGridViewModels = value; }
        }
        /// <summary>
        /// 临时存放对象
        /// </summary>
        public object ObjectModel { get; set; }
    }
}
