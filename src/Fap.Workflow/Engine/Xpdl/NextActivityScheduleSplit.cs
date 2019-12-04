﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fap.Workflow.Engine.Common;
using Fap.Workflow.Engine.Enums;

namespace Fap.Workflow.Engine.Xpdl
{
    /// <summary>
    /// 分支类型，获取下一步节点列表
    /// </summary>
    internal class NextActivityScheduleSplit : NextActivityScheduleBase
    {
        internal NextActivityScheduleSplit(IProcessModel processModel) : base(processModel)
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
            var transitionList = this.ProcessModel.GetForwardTransitionList(currentGatewayActivity.ActivityID).ToList();

            if (currentGatewayActivity.GatewayDirectionType == GatewayDirectionEnum.AndSplit)
            {
                //判读连线上的条件是否都满足，如果都满足才可以取出后续节点列表
                //获取AndSplit的每一条后续连线上的To节点
                foreach (TransitionEntity transition in transitionList)
                {
                    child = GetNextActivityListFromGatewayCore(transition,
                        out resultType);

                    gatewayComponent = AddChildToGatewayComponent(fromTransition, currentGatewayActivity, gatewayComponent, child);
                }

                if (gatewayComponent == null)
                {
                    resultType = NextActivityMatchedType.WaitingForSplitting;
                }
            }
            else if (currentGatewayActivity.GatewayDirectionType == GatewayDirectionEnum.OrSplit)
            {
                //获取OrSplit的，满足条件的后续连线上的To节点
                foreach (TransitionEntity transition in transitionList)
                {

                    child = GetNextActivityListFromGatewayCore(transition,
                        out resultType);

                    gatewayComponent = AddChildToGatewayComponent(fromTransition, currentGatewayActivity, gatewayComponent, child);


                    if (gatewayComponent == null)
                    {
                        resultType = NextActivityMatchedType.NoneTransitionMatchedToSplit;
                    }
                }
            }
            else if (currentGatewayActivity.GatewayDirectionType == GatewayDirectionEnum.XOrSplit)
            {
                //按连线定义的优先级排序
                transitionList.Sort(new TransitionPriorityCompare());

                //获取XOrSplit的，第一条满足条件的后续连线上的To节点
                foreach (TransitionEntity transition in transitionList)
                {

                    child = GetNextActivityListFromGatewayCore(transition,
                        out resultType);

                    gatewayComponent = AddChildToGatewayComponent(fromTransition, currentGatewayActivity, gatewayComponent, child);
                    //退出循环
                    break;

                }

                if (gatewayComponent == null)
                {
                    resultType = NextActivityMatchedType.NoneTransitionMatchedToSplit;
                }
            }
            else if (currentGatewayActivity.GatewayDirectionType == GatewayDirectionEnum.ComplexSplit)
            {
                resultType = NextActivityMatchedType.Failed;
                throw new Exception("ComplexSplit 没有具体实现！");
            }
            else
            {
                resultType = NextActivityMatchedType.Failed;
                throw new Exception("Split 分支节点的类型不明确！");
            }

            return gatewayComponent;
        }
    }
}
