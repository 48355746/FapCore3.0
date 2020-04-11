using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fap.Workflow.Engine.Entity
{
    /// <summary>
    /// 任务查询实体
    /// </summary>
    public class TaskQueryEntity
    {
        /// <summary>
        /// 任务状态
        /// </summary>
        public string TaskState { get; set; }
        /// <summary>
        /// 业务流程发起人
        /// </summary>
        public string StarterId { get; set; }
        /// <summary>
        /// 任务执行人
        /// </summary>
        public string ExecutorId { get; set; }

    }
}
