using Fap.Core.DataAccess;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.Utility;
using Fap.Workflow.Engine.Common;
using Fap.Workflow.Model;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Fap.Workflow.Engine.Manager
{
    /// <summary>
    /// 任务和流程处理的记录
    /// </summary>
    internal class TaskAdviceManager: ManagerBase
	{
        private readonly IFapApplicationContext _applicationContext;
        public TaskAdviceManager(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _applicationContext =_serviceProvider.GetService<IFapApplicationContext>();
        }

        public void RecordWhenStartupProcess(string processId, string taskId, string suggestion)
        {
            string userId = _applicationContext.EmpUid;
            string userName = _applicationContext.EmpName;
            this.Insert(WorkflowConstants.constant_startup, userId, userName,
                processId, taskId, "", suggestion);
        }

        public void RecordWhenCreateTask(string processId, string taskId, string suggestion)
        {
            string userId = _applicationContext.EmpUid;
            string userName = _applicationContext.EmpName;
            WfProcessInstance process = _dataAccessor.Get<WfProcessInstance>(processId,false);
            this.Insert(WorkflowConstants.constant_create, userId, userName, 
                processId, taskId, process.ApproveResult, suggestion);
        }

        public void RecordWhenCompleteTask(string processId, string taskId, string suggestion)
        {
            WfProcessInstance process = _dataAccessor.Get<WfProcessInstance>(processId,false);
            this.Insert(WorkflowConstants.constant_approve,
                processId, taskId, process.ApproveResult, suggestion);
        }

        public void RecordWhenCompleteProcess(string processId, string taskId, string suggestion)
        {
            WfProcessInstance process = _dataAccessor.Get<WfProcessInstance>(processId,false);
            this.Insert(WorkflowConstants.constant_complete,
                processId, taskId, process.ApproveResult, suggestion);
        }

        public void RecordWhenBack(string processId, string taskId, string suggestion)
        {
            this.Insert(WorkflowConstants.constant_back,
                processId, taskId, WfApproveState.Disagree, suggestion);
        }

        public void RecordWhenRecall(string processId, string taskId, string suggestion)
        {
            this.Insert(WorkflowConstants.constant_recall, 
                processId, taskId, "", suggestion);
        }

        public void RecordWhenReject(string processId, string taskId, string suggestion)
        {
            this.Insert(WorkflowConstants.constant_reject, 
                processId, taskId, WfApproveState.Disagree, suggestion);
        }

        public void RecordWhenWithdraw(string processId, string taskId, string suggestion)
        {
            this.Insert(WorkflowConstants.constant_withdraw,
                processId, taskId, WfApproveState.Disagree, suggestion);
        }

        public void RecordWhenCancel(string processId, string taskId, string suggestion)
        {
            this.Insert(WorkflowConstants.constant_cancle, 
                processId, taskId, "", suggestion);
        }

        public void RecordWhenSuspend(string processId, string suggestion)
        {
            this.Insert(WorkflowConstants.constant_suspend,
                processId, "", "", suggestion);
        }

        public void RecordWhenResume(string processId, string suggestion)
        {
            this.Insert(WorkflowConstants.constant_resume,
                processId, "", "", suggestion);
        }

        public void RecordWhenDelete(string processId, string suggestion)
        {
            this.Insert(WorkflowConstants.constant_delete, 
                processId, "", "", "");
        }

        public void RecordWhenEnd(string processId, string suggestion)
        {
            this.Insert(WorkflowConstants.constant_end, 
                processId, "", "", "");
        }

        /// <summary>
        /// 转办任务
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="suggestion"></param>
        public void RecordWhenTurnTask(string processId, string taskId, string suggestion, string turner, string turnerName)
        {
            this.Insert(WorkflowConstants.constant_turntask,
                processId, taskId, "", suggestion, turner, turnerName);
        }
        /// <summary>
        /// 指派代办任务
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="suggestion"></param>
        public void RecordWhenAssignAgentTask(string processId, string taskId, string suggestion, string agent, string agentName)
        {
            this.Insert(WorkflowConstants.constant_assign_agent_task,
                processId, taskId, "", suggestion, agent, agentName);
        }


        public void Insert(string handleEvent, string processId, string taskId,
            string approveState, string suggestion, string assistUser = "", string assistUserName = "")
        {
            //填写任务意见单
            WfTaskAdvice taskAdvice = new WfTaskAdvice();
            taskAdvice.Fid = UUIDUtils.Fid;
            taskAdvice.HandleEvent = handleEvent;
            taskAdvice.ProcessId = processId;
            taskAdvice.TaskUid = taskId;
            taskAdvice.Suggestion = suggestion;
            taskAdvice.ApproveState = approveState;
            taskAdvice.HandleTime = DateTimeUtils.CurrentDateTimeStr;
            taskAdvice.HandleByUser = _applicationContext.EmpUid;
            taskAdvice.HandleByUserName = _applicationContext.EmpName;
            taskAdvice.AssistUser = assistUser;
            taskAdvice.AssistUserName = assistUserName;
            _dataAccessor.Insert<WfTaskAdvice>(taskAdvice);
        }

    }
}
