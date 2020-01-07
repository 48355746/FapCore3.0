using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.AspNetCore.ViewModel
{
    public class TreePostData
    {
        /// <summary>
        /// 操作符
        /// </summary>
        public string Operation { get; set; }
        /// <summary>
        /// Id此处值为Fid值
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 位置
        /// </summary>
        public string Position { get; set; }
        /// <summary>
        /// 文本
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// 父节点
        /// </summary>
        public  string Parent { get; set; }
    }
}
