using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fap.Workflow.Engine.Common;

namespace Fap.Workflow.Engine.Xpdl
{
    /// <summary>
    /// 合并类型，获取下一步节点列表
    /// </summary>
    internal class NextActivityScheduleJoin : NextActivityScheduleBase
    {

        internal NextActivityScheduleJoin(IProcessModel processModel) :
            base(processModel)
        {
            
        }

                /// <summary>
        /// 获取下一步节点列表
        /// </summary>
        /// <param name="processInstanceID"></param>
        /// <param name="fromTransition"></param>
        /// <param name="currentGatewayActivity"></param>
        /// <param name="conditionKeyValuePair"></param>
        /// <returns></returns>
        internal override NextActivityComponent GetNextActivityListFromGateway(TransitionEntity fromTransition,
            ActivityEntity currentGatewayActivity,            
            out NextActivityMatchedType resultType)
        {
            NextActivityComponent child = null;
            NextActivityComponent gatewayComponent = null;
            resultType = NextActivityMatchedType.Unknown;

            //直接取出下步列表，运行时再根据条件执行
            List<TransitionEntity> transitionList = base.ProcessModel.GetForwardTransitionList(currentGatewayActivity.ActivityID).ToList();
            foreach (TransitionEntity transition in transitionList)
            {
                child = GetNextActivityListFromGatewayCore(transition,                   
                    out resultType);

                gatewayComponent = AddChildToGatewayComponent(fromTransition, currentGatewayActivity, gatewayComponent, child);
            }

            return gatewayComponent;
        }
    }
}
