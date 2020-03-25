using Fap.Core.DataAccess;
using Fap.Workflow.Engine.Common;
using Fap.Workflow.Engine.Enums;
using Fap.Workflow.Engine.Node;
using Fap.Workflow.Engine.Xpdl;
using Fap.Workflow.Model;
using Microsoft.Extensions.Logging;
using System;

namespace Fap.Wrokflow.Engine.Node
{
    /// <summary>
    /// XOrJoin 节点处理类
    /// </summary>
    internal class NodeMediatorXOrJoin : NodeMediatorGateway, ICompleteAutomaticlly
    {
        internal NodeMediatorXOrJoin(ActivityEntity activity, IProcessModel processModel, WfAppRunner runner, IServiceProvider serviceProvider)
            : base(activity, processModel, runner,serviceProvider)
        {

        }

        #region ICompleteAutomaticlly 成员

        public GatewayExecutedResult CompleteAutomaticlly(WfProcessInstance processInstance,
            string transitionGUID,
            WfActivityInstance fromActivityInstance)
        {
            GatewayExecutedResult result = GatewayExecutedResult.CreateGatewayExecutedResult(GatewayExecutedStatus.Unknown);

            bool canRenewInstance = false;

            //检查是否有运行中的合并节点实例
            WfActivityInstance joinNode = base.ActivityInstanceManager.GetActivityInstanceRunning(
                processInstance.Fid,
                base.GatewayActivity.ActivityID);

            if (joinNode == null)
            {
                canRenewInstance = true;
            }
            else
            {
                //判断是否可以激活下一步节点
                canRenewInstance = (joinNode.CanRenewInstance == 1);
                if (!canRenewInstance)
                {
                    result = GatewayExecutedResult.CreateGatewayExecutedResult(GatewayExecutedStatus.FallBehindOfXOrJoin);
                    return result;
                }
            }

            if (canRenewInstance)
            {
                var gatewayActivityInstance = base.CreateActivityInstanceObject(base.GatewayActivity,processInstance);

                gatewayActivityInstance.DirectionType = GatewayDirectionEnum.XOrJoin.ToString();

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

                result = GatewayExecutedResult.CreateGatewayExecutedResult(GatewayExecutedStatus.Successed);
            }
            return result;
        }
        #endregion
    }
}
