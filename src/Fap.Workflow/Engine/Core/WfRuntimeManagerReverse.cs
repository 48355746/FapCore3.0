using Fap.Core.DataAccess;
using Fap.Workflow.Engine.Common;
using Fap.Workflow.Engine.Enums;
using Fap.Workflow.Engine.Manager;
using Fap.Wrokflow.Engine.Node;
using Microsoft.Extensions.Logging;

namespace Fap.Workflow.Engine.Core
{
    /// <summary>
    /// 流程返签时的运行时
    /// </summary>
    internal class WfRuntimeManagerReverse : WfRuntimeManager
    {
        internal WfRuntimeManagerReverse(IDbContext dbContext,  ILoggerFactory loggerFactory) : base(dbContext,  loggerFactory)
        {
        }

        /// <summary>
        /// 返签执行方法
        /// </summary>
        /// <param name="session">会话</param>
        internal override void ExecuteInstanceImp()
        {
            //修改流程实例为返签状态
            var pim = new ProcessInstanceManager(_dataAccessor,_loggerFactory);
            pim.Reverse(base.BackwardContext.ProcessInstance.Fid);

            //创建新任务节点
            var backMostPreviouslyActivityInstanceID = GetBackwardMostPreviouslyActivityInstanceID();
            var nodeMediatorBackward = new NodeMediatorBackward(base.BackwardContext, AppRunner, _dataAccessor,  _loggerFactory);
            nodeMediatorBackward.CreateBackwardActivityTaskTransitionInstance(base.BackwardContext.ProcessInstance,
                base.BackwardContext.BackwardFromActivityInstance,
                BackwardTypeEnum.Reversed,
                backMostPreviouslyActivityInstanceID,
                base.BackwardContext.BackwardToTargetTransitionGUID,
                TransitionTypeEnum.Backward,
                TransitionFlyingTypeEnum.NotFlying);

            //构造回调函数需要的数据
            WfExecutedResult result = base.WfExecutedResult;
            result.BackwardTaskReciever = base.BackwardContext.BackwardTaskReciever;
            result.Status = WfExecutedStatus.Success;
        }
    }
}
