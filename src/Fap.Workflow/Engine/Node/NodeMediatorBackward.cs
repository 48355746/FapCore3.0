
using Fap.Core.DataAccess;
using Fap.Workflow.Engine.Common;
using Fap.Workflow.Engine.Core;
using Fap.Workflow.Engine.Enums;
using Fap.Workflow.Engine.Node;
using Fap.Workflow.Model;
using Microsoft.Extensions.Logging;
using System;

namespace Fap.Wrokflow.Engine.Node
{
    /// <summary>
    /// 退回处理时的节点调节器
    /// </summary>
    internal class NodeMediatorBackward : NodeMediator
    {
        internal NodeMediatorBackward(BackwardContext backwardContext, WfAppRunner runner, IDbContext dbContext, ILoggerFactory loggerFactory)
            : base(backwardContext, runner,dbContext,loggerFactory)
        {
            
        }

        internal override void ExecuteWorkItem()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 创建退回时的流转节点对象、任务和转移数据
        /// </summary>
        /// <param name="processInstance">流程实例</param>
        /// <param name="fromActivityInstance">运行节点实例</param>
        /// <param name="backwardType">退回类型</param>
        /// <param name="backMostPreviouslyActivityInstanceID">退回节点实例ID</param>
        /// <param name="transitionGUID">转移GUID</param>
        /// <param name="transitionType">转移类型</param>
        /// <param name="flyingType">跳跃类型</param>
        /// <param name="activityResource">资源</param>
        /// <param name="session">会话</param>
        internal void CreateBackwardActivityTaskTransitionInstance(WfProcessInstance processInstance,
            WfActivityInstance fromActivityInstance,
            BackwardTypeEnum backwardType,
            string backMostPreviouslyActivityInstanceID,
            string transitionGUID,
            TransitionTypeEnum transitionType,
            TransitionFlyingTypeEnum flyingType)
        {
            //实例化Activity
            var toActivityInstance = base.CreateBackwardToActivityInstanceObject(processInstance,fromActivityInstance,
                backwardType,
                backMostPreviouslyActivityInstanceID,
                this.AppRunner);

            //进入准备运行状态
            toActivityInstance.ActivityState = ActivityStateEnum.Ready.ToString();
            toActivityInstance.AssignedToUserIds = base.GenerateActivityAssignedUserIDs(this.BackwardContext.BackwardToTaskActivity.Performers);
            toActivityInstance.AssignedToUserNames = base.GenerateActivityAssignedUserNames(this.BackwardContext.BackwardToTaskActivity.Performers);

            //插入活动实例数据
            base.ActivityInstanceManager.Insert(toActivityInstance);

            base.ReturnDataContext.ActivityInstanceUid = toActivityInstance.Fid;
            base.ReturnDataContext.ProcessInstanceUid= toActivityInstance.ProcessInsUid;

            //插入任务数据
            base.CreateNewTask(toActivityInstance, this.BackwardContext.BackwardToTaskActivity);

            //插入转移数据
            base.InsertTransitionInstance(processInstance,
                transitionGUID,
                fromActivityInstance,
                toActivityInstance,
                transitionType,
                flyingType,AppRunner);
        }
    }
}
