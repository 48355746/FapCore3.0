
namespace Fap.Workflow.Engine.Common
{
    public class WorkflowResult
    {
        public WorkflowStatus Status { get; set; }
        public string Message { get; set; }
        public string ExceptionType { get; set; }
        public dynamic Data { get; set; }

        public WorkflowResult()
        {
            Status = WorkflowStatus.Success;
        }
    }

    /// <summary>
    /// 状态执行枚举类型
    /// </summary>
    public enum WorkflowStatus
    {
        /// <summary>
        /// 成功状态
        /// </summary>
        Success = 1,

        /// <summary>
        /// 执行失败状态
        /// </summary>
        Failed = 2,

        /// <summary>
        /// 异常状态
        /// </summary>
        Exception = 3
    }

}
