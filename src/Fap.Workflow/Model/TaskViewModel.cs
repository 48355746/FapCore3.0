using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fap.Workflow.Model
{
    public class TaskViewModel
    {
        /// <summary>
        /// 任务Id
        /// </summary>
        public string TaskId { get; set; }
        /// <summary>
        /// 任务名称
        /// </summary>
        public string TaskName { get; set; }
        /// <summary>
        /// 执行人ID
        /// </summary>
        public string ExecutorName { get; set; }
        /// <summary>
        /// 执行人名称
        /// </summary>
        public string ExecutorRealName {get;set;}
        /// <summary>
        /// 任务状态
        /// </summary>
        public string TaskState { get; set; }
        /// <summary>
        /// 任务类型
        /// </summary>
        public string TaskType { get; set; }
        /// <summary>
        /// 任务开始时间
        /// </summary>
        public string TaskStartTime { get; set; }
        /// <summary>
        /// 任务结束时间
        /// </summary>
        public string TaskEndTime { get; set; }
        /// <summary>
        /// 流程发起人ID
        /// </summary>
        public string StarterName { get; set; }
        /// <summary>
        /// 流程发起人姓名
        /// </summary>
        public string StarterRealName { get; set; }
        /// <summary>
        /// 流程ID
        /// </summary>
        public string ProcessId { get; set; }
        /// <summary>
        /// 流程名称
        /// </summary>
        public string ProcessName { get; set; }
        /// <summary>
        /// 流程状态
        /// </summary>
        public string ProcessState { get; set; }
    }
}
