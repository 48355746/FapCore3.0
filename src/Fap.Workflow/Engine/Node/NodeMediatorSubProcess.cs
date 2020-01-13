using Fap.Core.DataAccess;
using Fap.Workflow.Engine.Common;
using Fap.Workflow.Engine.Core;
using Fap.Workflow.Engine.Enums;
using Fap.Workflow.Engine.Event;
using Fap.Workflow.Engine.Exceptions;
using Fap.Workflow.Engine.Manager;
using Fap.Workflow.Engine.Xpdl;
using Fap.Workflow.Engine.Xpdl.Entity;
using Fap.Workflow.Engine.Xpdl.Node;
using Fap.Workflow.Model;
using Microsoft.Extensions.Logging;

namespace Fap.Workflow.Engine.Node
{
    /// <summary>
    /// 子流程节点执行器
    /// </summary>
    internal class NodeMediatorSubProcess : NodeMediator
    {
        internal NodeMediatorSubProcess(ActivityForwardContext forwardContext, WfAppRunner runner,IDbContext dbContext,ILoggerFactory loggerFactory)
            : base(forwardContext, runner,dbContext,loggerFactory)
        {

        }

        internal NodeMediatorSubProcess(WfAppRunner runner, IDbContext dbContext, ILoggerFactory loggerFactory)
            : base(runner,dbContext,loggerFactory)
        {
        }

        /// <summary>
        /// 执行子流程节点
        /// </summary>
        internal override void ExecuteWorkItem()
        {
            try
            {
                if (base.Linker.FromActivity.ActivityType == ActivityTypeEnum.SubProcessNode)
                {
                    //检查子流程是否结束
                    var pim = new ProcessInstanceManager(_dataAccessor,_loggerFactory);
                    bool isCompleted = pim.CheckSubProcessInstanceCompleted(
                        base.Linker.FromActivityInstance.Fid,
                        base.Linker.FromActivityInstance.NodeId);
                    if (isCompleted == false)
                    {
                        throw new WfRuntimeException(string.Format("当前子流程:[{0}]并没有到达结束状态，主流程无法向下执行。",
                            base.Linker.FromActivity.ActivityName));
                    }
                }

                //完成当前的任务节点
                bool canContinueForwardCurrentNode = CompleteWorkItem(ActivityForwardContext.TaskView.Fid);

                if (canContinueForwardCurrentNode)
                {
                    //获取下一步节点列表：并继续执行
                    ContinueForwardCurrentNode(false);
                }
            }
            catch 
            {
                throw;
            }
        }

        /// <summary>
        /// 完成任务实例
        /// </summary>
        /// <param name="taskID">任务ID</param>
        /// <param name="activityResource">活动资源</param>
        /// <param name="session">会话</param>
        internal bool CompleteWorkItem(string  taskUid)
        {
            bool canContinueForwardCurrentNode = true;

            //完成本任务，返回任务已经转移到下一个会签任务，不继续执行其它节点
            base.TaskManager.Complete(taskUid, AppRunner);

            //设置活动节点的状态为完成状态
            base.ActivityInstanceManager.Complete(base.Linker.FromActivityInstance.Fid,
                AppRunner);
            base.Linker.FromActivityInstance.ActivityState = ActivityStateEnum.Completed.ToString();

            return canContinueForwardCurrentNode;
        }

        /// <summary>
        /// 创建活动任务转移数据
        /// </summary>
        /// <param name="toActivity">目的活动</param>
        /// <param name="processInstance">流程实例</param>
        /// <param name="fromActivityInstance">来源活动实例</param>
        /// <param name="transitionGUID">转移GUID</param>
        /// <param name="transitionType">转移类型</param>
        /// <param name="flyingType">飞跃类型</param>
        /// <param name="activityResource">活动资源</param>
        /// <param name="session">会话</param>
        internal override void CreateActivityTaskTransitionInstance(ActivityEntity toActivity,
           WfProcessInstance processInstance,
           WfActivityInstance fromActivityInstance,
           string transitionGUID,
           TransitionTypeEnum transitionType,
           TransitionFlyingTypeEnum flyingType)
        {
            //实例化Activity
            var toActivityInstance = CreateActivityInstanceObject(toActivity, processInstance, AppRunner);

            //进入运行状态
            toActivityInstance.ActivityState = ActivityStateEnum.Ready.ToString();
            toActivityInstance.AssignedToUserIds = GenerateActivityAssignedUserIDs(
                toActivity.Performers);
            toActivityInstance.AssignedToUserNames = GenerateActivityAssignedUserNames(
                toActivity.Performers);

            //插入活动实例数据
            base.ActivityInstanceManager.Insert(toActivityInstance);

            //插入任务数据
            base.CreateNewTask(toActivityInstance,toActivity);

            //插入转移数据
            InsertTransitionInstance(processInstance,
                transitionGUID,
                fromActivityInstance,
                toActivityInstance,
                transitionType,
                flyingType,AppRunner);

            //启动子流程
            WfExecutedResult startedResult = null;
            var subProcessNode = (SubProcessNode)toActivity.Node;
            subProcessNode.ActivityInstance = fromActivityInstance;
            WfAppRunner subRunner = CopyActivityForwardRunner(AppRunner,
                new Performer()
                {
                    UserId = AppRunner.UserId,
                    UserName =
                   AppRunner.UserName
                },subProcessNode );

            var runtimeInstance = WfRuntimeManagerFactory.CreateRuntimeInstanceStartup(subRunner, _dataAccessor,_loggerFactory,
                processInstance,
                subProcessNode,
                ref startedResult);

            runtimeInstance.OnWfProcessExecuted += runtimeInstance_OnWfProcessStarted;
            runtimeInstance.Execute();
        }

        private WfExecutedResult _startedResult = null;
        private void runtimeInstance_OnWfProcessStarted(object sender, WfEventArgs args)
        {
            _startedResult = args.WfExecutedResult;
        }


        /// <summary>
        /// 创建子流程时，重新生成runner信息
        /// </summary>
        /// <param name="runner">运行者</param>
        /// <param name="performer">下一步执行者</param>
        /// <param name="subProcessNode">子流程节点</param>
        /// <returns></returns>
        private WfAppRunner CopyActivityForwardRunner(WfAppRunner runner,
            Performer performer,
            SubProcessNode subProcessNode)
        {
            WfAppRunner subRunner = new WfAppRunner();
            subRunner.BizData = runner.BizData;
            subRunner.BizUid = runner.BizUid;
            subRunner.AppName = runner.AppName;
            subRunner.UserId = performer.UserId;
            subRunner.UserName = performer.UserName;
            subRunner.ProcessId = subProcessNode.SubProcessId;

            return subRunner;
        }
    }
}
