/* ==============================================================================
 * 功能描述：  
 * 创 建 者：wyf
 * 创建日期：2017-02-21 18:57:41
 * ==============================================================================*/

namespace Fap.AspNetCore.ViewModel
{
    /// <summary>
    /// grid统计Model
    /// </summary>
    public class GridWidgetViewModel : IViewModel
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 显示列头名
        /// </summary>
        public string[] DisplayCols { get; set; }
        /// <summary>
        /// 绑定列名
        /// </summary>
        public string[] ColNames { get; set; }

    }
}
