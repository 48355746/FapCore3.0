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
    /// OrJoin 节点处理类
    /// </summary>
    internal class NodeMediatorOrJoin : NodeMediatorGateway, ICompleteAutomaticlly
    {
        internal NodeMediatorOrJoin(ActivityEntity activity, IProcessModel processModel, WfAppRunner runner, IDbContext dbContext,  ILoggerFactory loggerFactory)
            : base(activity, processModel, runner,dbContext,loggerFactory)
        {

        }
		internal int GetTokensRequired(string processInstanceUid)
        {
            int splitCount = 0;
            int joinCount = 0;
            ActivityEntity splitActivity = this.ProcessModel.GetBackwardGatewayActivity(GatewayActivity,ref joinCount,ref splitCount);

            return base.ActivityInstanceManager.GetInstanceGatewayCount(splitActivity.ActivityID, processInstanceUid);
        }

        #region ICompleteAutomaticlly 成员

        /// <summary>
        /// OrJoin合并时的节点完成方法
        /// 1. 如果有满足条件的OrJoin前驱转移，则会重新生成新的OrJoin节点实例
        /// </summary>
        /// <param name="processInstance">流程实例</param>
        /// <param name="transitionGUID">转移GUID</param>
        /// <param name="fromActivityInstance">起始活动实例</param>
        /// <param name="activityResource">活动资源</param>
        /// <param name="session">会话</param>
        public GatewayExecutedResult CompleteAutomaticlly(WfProcessInstance processInstance,
            string transitionGUID,
            WfActivityInstance fromActivityInstance)
        {
			//检查是否有运行中的合并节点实例
            WfActivityInstance joinNode = base.ActivityInstanceManager.GetActivityInstanceRunning(
                processInstance.Fid,
                base.GatewayActivity.ActivityID);

            int tokensRequired = 0;
            int tokensHad = 0;
            if (joinNode == null)
            {
                var joinActivityInstance = base.CreateActivityInstanceObject(base.GatewayActivity,processInstance);

                //计算总需要的Token数目
                joinActivityInstance.TokensRequired = GetTokensRequired(processInstance.Fid);
                joinActivityInstance.TokensHad = 1;
                tokensRequired = joinActivityInstance.TokensRequired;

                //进入运行状态
                joinActivityInstance.ActivityState =ActivityStateEnum.Running.ToString();
                //joinActivityInstance.DirectionType =GatewayDirectionEnum.AndJoin.ToString();

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
                tokensRequired = base.GatewayActivityInstance.TokensRequired;
                tokensHad = base.GatewayActivityInstance.TokensHad;
                //更新Token数目
                base.ActivityInstanceManager.IncreaseTokensHad(base.GatewayActivityInstance.Fid,base._runner);
                base.InsertTransitionInstance(processInstance,
                    transitionGUID,
                    fromActivityInstance,
                    joinNode,
                    TransitionTypeEnum.Forward,
                    TransitionFlyingTypeEnum.NotFlying);
            }
            if ((tokensHad + 1) == tokensRequired)
            {
                //如果达到完成节点的Token数，则设置该节点状态为完成
                base.CompleteActivityInstance(base.GatewayActivityInstance.Fid);
                base.GatewayActivityInstance.ActivityState = ActivityStateEnum.Completed.ToString();
            }
            
            GatewayExecutedResult result = GatewayExecutedResult.CreateGatewayExecutedResult(GatewayExecutedStatus.Successed);
            return result;
        }

        #endregion
    }
}
