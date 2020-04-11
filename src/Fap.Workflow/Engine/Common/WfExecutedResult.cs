
using Fap.Workflow.Engine.Node;
using System.Collections.Generic;
namespace Fap.Workflow.Engine.Common
{
    /// <summary>
    /// 流程执行结果
    /// </summary>
    public class WfExecutedResult
    {
        /// <summary>
        /// 状态
        /// </summary>
        public WfExecutedStatus Status { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 执行类型
        /// </summary>
        public string ExceptionType { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public dynamic Data { get; set; }
        /// <summary>
        /// 流程启动返回的新实例Fid
        /// </summary>
        public string ProcessInsUidStarted { get; set; }     //流程启动返回的新实例Fid
        /// <summary>
        /// 流程做回退处理时，返回的任务接收人信息
        /// </summary>
        public WfBackwardTaskReciever BackwardTaskReciever { get; set; }    //流程做回退处理时，返回的任务接收人信息
        public ReturnDataContext ReturnDataContext { get; set; }  //流程回退时返回参数
        /// <summary>
        /// 新任务IDs
        /// </summary>
        public List<string> TaskIdsCreated { get; set; } //新任务ID
        public WfExecutedResult()
        {
            Status = WfExecutedStatus.Default;
        }
    }

    /// <summary>
    /// 状态执行枚举类型
    /// </summary>
    public enum WfExecutedStatus
    {
        /// <summary>
        /// 缺省状态
        /// </summary>
        Default = 0,

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

    public class WfExceptionType
    {
        //流程启动异常信息
        public const string Started_IsRunningAlready = "Started_IsRunningAlready";

        //流程运行异常信息
        public const string RunApp_ErrorArguments = "RunApp_ErrorArguments";
        public const string RunApp_HasNoTask = "RunApp_HasNoTask";
        public const string RunApp_OverTasks = "RunApp_OverTasks";
        public const string RunApp_RuntimeError = "RunApp_RuntimeError";

        //流程跳转异常信息
        public const string Jump_ErrorArguments = "Jump_ErrorArguments";
        public const string Jump_OverOneStep = "Jump_OverOneStep";
        public const string Jump_NotActivityBackCompleted = "Jump_NotActivityBackCompleted";
        public const string Jump_OtherError = "Jump_OtherError";

        //流程撤销异常信息
        public const string Withdraw_NotInReady = "Withdraw_NotInReady";
        public const string Withdraw_NotCreatedByMine = "Withdraw_NotCreatedByMine";
        public const string Withdraw_HasTooMany = "Withdraw_HasTooMany";
        public const string Withdraw_PreviousIsEndNode = "Withdraw_PreviousIsEndNode";
        public const string Withdraw_SignTogetherNotAllowed = "Withdraw_SignTogetherNotAllowed";

        //流程退回异常信息
        public const string Sendback_NotTaskNode = "Sendback_NotTaskNode";
        public const string Sendback_IsLoopNode = "Sendback_IsLoopNode";
        public const string Sendback_NotInRunning = "Sendback_NotInRunning";
        public const string Sendback_NotMineTask = "NotMineTask";
        public const string Sendback_PreviousIsStartNode = "Sendback_PreviousIsStartNode";

        //流程返签异常信息
        public const string Reverse_NotInCompleted = "Reverse_NotInCompleted";

        //流程加签异常信息
        public const string SignForward_ErrorArguments = "SignForward_ErrorArguments";
        public const string SignForward_NoneSigners = "SignForward_NoneSigners";
        public const string SignForward_RuntimeError = "SignForward_RuntimeError";

        //流程驳回异常信息
        public const string RevokeFirst_ErrorArguments = "RevokeFirst_ErrorArguments";

    }
}
