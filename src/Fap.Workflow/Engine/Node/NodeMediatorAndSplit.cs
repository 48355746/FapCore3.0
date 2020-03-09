using Fap.Core.DataAccess;
using Fap.Workflow.Engine.Common;
using Fap.Workflow.Engine.Enums;
using Fap.Workflow.Engine.Node;
using Fap.Workflow.Engine.Xpdl;
using Fap.Workflow.Model;
using Microsoft.Extensions.Logging;

namespace Fap.Wrokflow.Engine.Node
{
    /// <summary>
    /// AndSplite 节点处理类
    /// </summary>
    internal class NodeMediatorAndSplit : NodeMediatorGateway, ICompleteAutomaticlly
    {
        internal NodeMediatorAndSplit(ActivityEntity activity, IProcessModel processModel, WfAppRunner runner,IDbContext dbContext,ILoggerFactory loggerFactory)
            : base(activity, processModel, runner,dbContext,loggerFactory)
        {

        }
        
        #region ICompleteAutomaticlly 成员
        public GatewayExecutedResult CompleteAutomaticlly(WfProcessInstance processInstance,
            string transitionGUID,
            WfActivityInstance fromActivityInstance)
        {

            //插入实例数据
            var gatewayActivityInstance = base.CreateActivityInstanceObject(base.GatewayActivity,
                processInstance);
            gatewayActivityInstance.DirectionType = GatewayDirectionEnum.AndSplit.ToString();

            base.InsertActivityInstance(gatewayActivityInstance);

            base.CompleteActivityInstance(gatewayActivityInstance.Fid);

            gatewayActivityInstance.ActivityState = ActivityStateEnum.Completed.ToString();
            base.GatewayActivityInstance = gatewayActivityInstance;
            
            //写节点转移实例数据
            base.InsertTransitionInstance(processInstance,
                transitionGUID,
                fromActivityInstance,
                gatewayActivityInstance,
                TransitionTypeEnum.Forward,
                TransitionFlyingTypeEnum.NotFlying);

            GatewayExecutedResult result = GatewayExecutedResult.CreateGatewayExecutedResult(GatewayExecutedStatus.Successed);
            return result;
        }

        #endregion
    }
}
