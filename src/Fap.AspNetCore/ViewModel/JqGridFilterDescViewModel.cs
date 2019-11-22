/* ==============================================================================
 * 功能描述：  
 * 创 建 者：wyf
 * 创建日期：2016/10/17 17:51:43
 * ==============================================================================*/

namespace Fap.AspNetCore.ViewModel
{
    public class JqGridFilterDescViewModel : IViewModel
    {
        /// <summary>
        /// 分组
        /// </summary>
        public string Group { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string FilterDesc { get; set; }
        /// <summary>
        /// 操作符
        /// </summary>
        public string FilterOper { get; set; }
        /// <summary>
        /// 结果
        /// </summary>

        public string FilterResult { get; set; }
    }
}
