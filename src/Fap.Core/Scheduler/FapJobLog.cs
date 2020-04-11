using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Scheduler
{
    /// <summary>
    /// 调度任务执行日志
    /// </summary>
    public class FapJobLog : BaseModel
    {
        /// <summary>
        /// 所属任务
        /// </summary>
        public string JobId { get; set; }
        /// <summary>
        /// 任务名称
        /// </summary>
        public string JobName { get; set; }
        /// <summary>
        /// 执行时间
        /// </summary>
        public string ExecuteTime { get; set; }
        /// <summary>
        /// 执行结果
        /// </summary>
        public string ExecuteResult { get; set; }
        /// <summary>
        /// 任务执行信息
        /// </summary>
        public string Message { get; set; }

    }
}
