using Fap.Core.DataAccess;
using Fap.Workflow.Engine.Common;
using Fap.Workflow.Engine.Enums;
using Fap.Workflow.Engine.Manager;
using Fap.Workflow.Engine.Node;
using Fap.Wrokflow.Engine.Node;
using Microsoft.Extensions.Logging;
using System;

namespace Fap.Workflow.Engine.Core
{
    /// <summary>
    /// 跳转方式的处理
    /// </summary>
    internal class WfRuntimeManagerJump : WfRuntimeManager
    {
        internal WfRuntimeManagerJump(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        /// <summary>
        /// 跳转执行方法
        /// </summary>
        /// <param name="session">会话</param>
        internal override void ExecuteInstanceImp()
        {
            WfExecutedResult result = base.WfExecutedResult;

            //回跳类型的处理
            if (base.IsBackward == true)
            {
                //创建新任务节点
                var backMostPreviouslyActivityInstanceID = GetBackwardMostPreviouslyActivityInstanceID();
                var nodeMediatorBackward = new NodeMediatorBackward(base.BackwardContext, AppRunner,_dataAccessor,_loggerFactory);

                nodeMediatorBackward.CreateBackwardActivityTaskTransitionInstance(base.BackwardContext.ProcessInstance,
                    base.BackwardContext.BackwardFromActivityInstance,
                    BackwardTypeEnum.Sendback,
                    backMostPreviouslyActivityInstanceID,
                    base.BackwardContext.BackwardToTargetTransitionGUID,
                    TransitionTypeEnum.Sendback,
                    TransitionFlyingTypeEnum.NotFlying);

                //更新当前办理节点的状态（从准备或运行状态更新为退回状态）
                var aim = new ActivityInstanceManager(_serviceProvider);
                aim.SendBack(base.BackwardContext.BackwardFromActivityInstance.Fid,
                   AppRunner);

                //构造回调函数需要的数据
                result.BackwardTaskReciever = base.BackwardContext.BackwardTaskReciever;
                result.Status = WfExecutedStatus.Success;
            }
            else
            {
                var jumpActivityGUID = base.AppRunner.NextActivity.ActivityID;
                var jumpforwardActivity = base.ProcessModel.GetActivity(jumpActivityGUID);
                var proecessInstance = (new ProcessInstanceManager(_serviceProvider)).GetByFid(base.RunningActivityInstance.ProcessInsUid);
                var jumpforwardExecutionContext = ActivityForwardContext.CreateJumpforwardContext(jumpforwardActivity,
                    base.ProcessModel, proecessInstance);

                NodeMediator mediator = NodeMediatorFactory.CreateNodeMediator(jumpforwardExecutionContext, AppRunner,_serviceProvider);
                mediator.Linker.FromActivityInstance = base.RunningActivityInstance;
                mediator.Linker.ToActivity = jumpforwardActivity;
                mediator.ExecuteWorkItem();

                result.Status = WfExecutedStatus.Success;
                result.Message = mediator.GetNodeMediatedMessage();
            }
        }
    }
}
