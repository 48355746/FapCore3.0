using Fap.Core.DataAccess;
using Fap.Workflow.Engine.Common;
using Fap.Workflow.Engine.Manager;
using Microsoft.Extensions.Logging;
using System;

namespace Fap.Workflow.Engine.Core
{
    /// <summary>
    /// 退回流程运行时
    /// </summary>
    internal class WfRuntimeManagerSendBack : WfRuntimeManager
    {
        internal WfRuntimeManagerSendBack(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        /// <summary>
        /// 退回操作的处理逻辑
        /// </summary>
        /// <param name="session">会话</param>
        internal override void ExecuteInstanceImp()
        {
            //当前任务实例设置为退回，存在其他任务实例设置为 终止，业务数据设置为 驳回状态

            //var nodeMediatorBackward = new NodeMediatorBackward(base.BackwardContext, AppRunner);
            //var aim = new ActivityInstanceManager(AppRunner.DataAccessor,AppRunner._DbSession,AppRunner.LoggerFactory);
            //var backMostPreviouslyActivityInstanceID = GetBackwardMostPreviouslyActivityInstanceID();

            ////上一步节点是普通节点的退回处理
            //nodeMediatorBackward.CreateBackwardActivityTaskTransitionInstance(base.BackwardContext.ProcessInstance,
            //    base.BackwardContext.BackwardFromActivityInstance,
            //    BackwardTypeEnum.Sendback,
            //    backMostPreviouslyActivityInstanceID,
            //    base.BackwardContext.BackwardToTargetTransitionGUID,
            //    TransitionTypeEnum.Sendback,
            //    TransitionFlyingTypeEnum.NotFlying);

            ////更新当前办理节点的状态（从准备或运行状态更新为退回状态）
            //aim.SendBack(base.BackwardContext.BackwardFromActivityInstance.Fid,AppRunner);
            //设置流程实例为驳回状态
            var pim = new ProcessInstanceManager(_serviceProvider);
            pim.Revoke(AppRunner.CurrProcessInsUid);
            var aim = new ActivityInstanceManager(_serviceProvider);
            aim.Revoke(AppRunner.CurrActivityInsUid, AppRunner.CurrProcessInsUid);
            var tim = new TaskManager(_serviceProvider);
            tim.Revoke(AppRunner.CurrWfTaskUid, AppRunner);

            //构造回调函数需要的数据
            WfExecutedResult result = base.WfExecutedResult;
            //result.BackwardTaskReciever = base.BackwardContext.BackwardTaskReciever;
            //result.ReturnDataContext = nodeMediatorBackward.ReturnDataContext;
            result.Status = WfExecutedStatus.Success;
        }
    }
}
