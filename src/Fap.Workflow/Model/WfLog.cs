using Fap.Core.Infrastructure.Metadata;
using System;

namespace Fap.Workflow.Model
{
    /// <summary>
    /// 流程日志
    /// </summary>
    [Serializable]
    public class WfLog :BaseModel
    {
        /// <summary>
        /// 事件类型
        /// </summary>
        public int EventTypeID { get; set; }
        /// <summary>
        /// 优先级
        /// </summary>
        public int Priority { get; set; }
        /// <summary>
        /// 严重性
        /// </summary>
        public string Severity { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 消息栈
        /// </summary>
        public string StackTrace { get; set; }
        /// <summary>
        /// 异常堆栈
        /// </summary>
        public string InnerStackTrace { get; set; }
        /// <summary>
        /// 请求数据
        /// </summary>
        public string RequestData { get; set; }
        /// <summary>
        /// 日志时间
        /// </summary>
        public string LogTime { get; set; }

        public string aa { get; set; }
    }

}
