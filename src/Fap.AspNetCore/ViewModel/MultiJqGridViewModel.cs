using System.Collections.Generic;

namespace Fap.AspNetCore.ViewModel
{
    /// <summary>
    /// 多个JqGridViewModel
    /// </summary>
    public class MultiJqGridViewModel:IViewModel
    {
        /// <summary>
        /// 临时数据存放
        /// </summary>
        public Dictionary<string, string> TempData { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, JqGridViewModel> JqGridViewModels { get; set; } = new Dictionary<string, JqGridViewModel>();        
        /// <summary>
        /// 临时存放对象
        /// </summary>
        public object ObjectModel { get; set; }
    }
}
