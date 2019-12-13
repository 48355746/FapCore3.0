using Fap.Core.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Tracker
{
    /// <summary>
    /// 触发事件的数据
    /// </summary>
    public struct EventData
    {
        /// <summary>
        /// 数据变化类型
        /// </summary>
        public string ChangeDataType { get; set; }
        /// <summary>
        /// 变化实体
        /// </summary>
        public string EntityName { get; set; }
        /// <summary>
        /// 数据对象
        /// </summary>
        public object ChangeData { get; set; }
    }
}
