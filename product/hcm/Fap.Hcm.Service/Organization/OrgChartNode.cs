using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Hcm.Service.Organization
{
    /// <summary>
    /// OrgChart节点
    /// </summary>
    public class OrgChartNode
    {
        /// <summary>
        /// ID
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 父ID
        /// </summary>
        public string Parent { get; set; }
        /// <summary>
        /// 布局类型：l左，r右，u默认 下面
        /// 'r'   'l'  'u'
        /// </summary>
        public string Ctype { get; set; }
        /// <summary>
        /// 显示文本，/n 回车
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// 边框 0 无，1有
        /// </summary>
        public int Bold { get; set; }
        /// <summary>
        /// 钻取 Url链接
        /// </summary>
        public string NavigationUrl { get; set; }
        public string Cline { get; set; }
        public string Cfill { get; set; }
        public string Ctext { get; set; }
        public string Image { get; set; }
        /// <summary>
        /// 图片对齐 rt 右上，lt左上 默认，ct中上
        /// </summary>
        public string Imgalign { get; set; }
        /// <summary>
        /// 扩展数据，只作为作图判断，不是node的属性
        /// </summary>
        public string ExtendData { get; set; }

        public int Order { get; set; }


    }
}
