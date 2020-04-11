
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
    /// AndJoin 节点处理类
    /// </summary>
    internal class NodeMediatorAndJoin : NodeMediatorGateway, ICompleteAutomaticlly
    {
        internal NodeMediatorAndJoin(ActivityEntity activity, IProcessModel processModel,WfAppRunner runner, IServiceProvider serviceProvider)
            : base(activity, processModel,runner, serviceProvider)
        {

        }

        internal int GetTokensRequired()
        {
            int tokensRequired = this.ProcessModel.GetBackwardTransitionListCount(GatewayActivity.ActivityID);
            return tokensRequired;
        }

        #region ICompleteAutomaticlly 成员

        public GatewayExecutedResult CompleteAutomaticlly(WfProcessInstance processInstance,
            string transitionGUID,
            WfActivityInstance fromActivityInstance)
        {
            //检查是否有运行中的合并节点实例
            WfActivityInstance joinNode = base.ActivityInstanceManager.GetActivityInstanceRunning(
                processInstance.Fid,
                base.GatewayActivity.ActivityID);

            if (joinNode == null)
            {
                var joinActivityInstance = base.CreateActivityInstanceObject(base.GatewayActivity,processInstance);

                //计算总需要的Token数目
                joinActivityInstance.TokensRequired = GetTokensRequired();
                joinActivityInstance.TokensHad = 1;

                //进入运行状态
                joinActivityInstance.ActivityState = ActivityStateEnum.Running.ToString();
                joinActivityInstance.DirectionType = GatewayDirectionEnum.AndJoin.ToString();

                base.InsertActivityInstance(joinActivityInstance);
				base.InsertTransitionInstance(processInstance,
                    transitionGUID,
                    fromActivityInstance,
                    joinActivityInstance,
                    TransitionTypeEnum.Forward,
                    TransitionFlyingTypeEnum.NotFlying);
            }
            else
            {
                //更新节点的活动实例属性
                base.GatewayActivityInstance = joinNode;
                int tokensRequired = base.GatewayActivityInstance.TokensRequired;
                int tokensHad = base.GatewayActivityInstance.TokensHad;

                //更新Token数目
                base.ActivityInstanceManager.IncreaseTokensHad(base.GatewayActivityInstance.Fid,this._runner);
                base.InsertTransitionInstance(processInstance,
                    transitionGUID,
                    fromActivityInstance,
                    joinNode,
                    TransitionTypeEnum.Forward,
                    TransitionFlyingTypeEnum.NotFlying);
                if ((tokensHad + 1) == tokensRequired)
                {
                    //如果达到完成节点的Token数，则设置该节点状态为完成
                    base.CompleteActivityInstance(base.GatewayActivityInstance.Fid);
                    base.GatewayActivityInstance.ActivityState = ActivityStateEnum.Completed.ToString();
                }
            }

            GatewayExecutedResult result = GatewayExecutedResult.CreateGatewayExecutedResult(
                GatewayExecutedStatus.Successed);
            return result;
        }

        #endregion
    }
}
