using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Infrastructure.Metadata
{
    /// <summary>
    /// 表特性
    /// </summary>
    public class FapTableFeature : BaseModel
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 存放字段列表的JSON字符
        /// </summary>
        public string Data { get; set; }
    }
}
