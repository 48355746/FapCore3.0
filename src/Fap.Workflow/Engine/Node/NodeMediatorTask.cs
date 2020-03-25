using Fap.Core.DataAccess;
using Fap.Workflow.Engine.Common;
using Fap.Workflow.Engine.Core;
using Fap.Workflow.Engine.Enums;
using Fap.Workflow.Engine.Xpdl;
using Fap.Workflow.Model;
using Microsoft.Extensions.Logging;
using System;

namespace Fap.Workflow.Engine.Node
{
    /// <summary>
    /// 任务节点执行器
    /// </summary>
    internal class NodeMediatorTask : NodeMediator
    {
        internal NodeMediatorTask(ActivityForwardContext forwardContext, WfAppRunner runner, IServiceProvider serviceProvider)
            : base(forwardContext, runner,serviceProvider)
        {

        }

        internal NodeMediatorTask(WfAppRunner runner, IDbContext dbContext, ILoggerFactory loggerFactory)
            : base(runner,dbContext,loggerFactory)
        {

        }

        /// <summary>
        /// 执行普通任务节点
        /// 1. 当设置任务完成时，同时设置活动完成
        /// 2. 当实例化活动数据时，产生新的任务数据
        /// </summary>
        internal override void ExecuteWorkItem()
        {
            try
            {
                //执行Action列表
                //ExecteActionList(ActivityForwardContext.Activity.ActionList,
                //    ActivityForwardContext.ActivityResource.AppRunner.ActionMethodParameters);

                //完成当前的任务节点
                bool canContinueForwardCurrentNode = CompleteWorkItem(ActivityForwardContext.TaskView,
                   this.AppRunner);

                if (canContinueForwardCurrentNode)
                {
                    bool isJumpforward = ActivityForwardContext.TaskView == null ? true : false;
                    //获取下一步节点列表：并继续执行
                    ContinueForwardCurrentNode(isJumpforward);
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 完成任务实例
        /// </summary>
        /// <param name="taskView">任务视图</param>
        /// <param name="activityResource">活动资源</param>
        /// <param name="session">会话</param>        
        internal bool CompleteWorkItem(WfTask taskView,
           WfAppRunner runner)
        {
            bool canContinueForwardCurrentNode = true;



            //流程强制拉取向前跳转时，没有运行人的任务实例
            if (taskView != null)
            {
                //完成本任务，返回任务已经转移到下一个会签任务，不继续执行其它节点
                base.TaskManager.Complete(taskView.Fid, runner);
            }
            //检查活动实例是否可以完成
            if (base.ActivityInstanceManager.IsComplete(base.Linker.FromActivityInstance.Fid))
            {
                //设置活动节点的状态为完成状态
                base.ActivityInstanceManager.Complete(base.Linker.FromActivityInstance.Fid, runner);

                base.Linker.FromActivityInstance.ActivityState = ActivityStateEnum.Completed.ToString();

            }
            else
            {
                canContinueForwardCurrentNode = false;
                //设置同一任务实例的下一个wftask
                if (base.Linker.FromActivityInstance.ActivityType != WfActivityType.SignNode && base.Linker.FromActivityInstance.ApproverMethod == ApproverMethodEnum.Queue.ToString())
                {
                    base.TaskManager.SetNextTaskHandling(taskView.ActivityInsUid, taskView.ApproverSort + 1);
                }
                //设置活动节点实例状态为运行态
                base.Linker.FromActivityInstance.ActivityState= ActivityStateEnum.Running.ToString();
                base.ActivityInstanceManager.Update(base.Linker.FromActivityInstance);
            }

            return canContinueForwardCurrentNode;
        }

        /// <summary>
        /// 创建活动任务转移实例数据
        /// </summary>
        /// <param name="toActivity">活动</param>
        /// <param name="processInstance">流程实例</param>
        /// <param name="fromActivityInstance">开始活动实例</param>
        /// <param name="transitionGUID">转移GUID</param>
        /// <param name="transitionType">转移类型</param>
        /// <param name="flyingType">跳跃类型</param>
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
            var toActivityInstance = base.CreateActivityInstanceObject(toActivity, processInstance, AppRunner);

            //进入运行状态
            toActivityInstance.ActivityState = ActivityStateEnum.Ready.ToString();
            toActivityInstance.AssignedToUserIds = GenerateActivityAssignedUserIDs(toActivity.Performers);
            toActivityInstance.AssignedToUserNames = GenerateActivityAssignedUserNames(toActivity.Performers);

            //插入活动实例数据
            base.ActivityInstanceManager.Insert(toActivityInstance);

            //插入任务数据
            base.CreateNewTask(toActivityInstance, toActivity);

            //插入转移数据
            InsertTransitionInstance(processInstance,
                transitionGUID,
                fromActivityInstance,
                toActivityInstance,
                transitionType,
                flyingType,
                AppRunner);
        }
    }
}
