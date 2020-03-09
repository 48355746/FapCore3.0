namespace Fap.AspNetCore.ViewModel
{
    /// <summary>
    /// 图表Model
    /// </summary>
    public class ChartWidgetViewModel:IViewModel
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 饼图URL
        /// </summary>
        public string PieChartURL { get; set; }
        /// <summary>
        /// 柱图URL
        /// </summary>
        public string BarChartURL { get; set; }
        /// <summary>
        /// 线图URL
        /// </summary>
        public string LineChartURL { get; set; }
        /// <summary>
        /// 标记符号
        /// </summary>
        public string Symbol { get; set; }
        /// <summary>
        /// 默认类型
        /// </summary>
        public string DefaultType { get; set; }
    }
}
