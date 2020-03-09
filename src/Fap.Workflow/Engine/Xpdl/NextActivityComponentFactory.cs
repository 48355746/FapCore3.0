﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fap.Workflow.Engine.Common;
using Fap.Workflow.Engine.Enums;

namespace Fap.Workflow.Engine.Xpdl
{
    /// <summary>
    /// 下一步节点的工厂类
    /// </summary>
    public class NextActivityComponentFactory
    {
        /// <summary>
        /// 创建下一步活动的节点
        /// </summary>
        /// <param name="activity"></param>
        /// <returns></returns>
        internal static NextActivityComponent CreateNextActivityComponent(TransitionEntity transition,
            ActivityEntity activity)
        {
            NextActivityComponent component = null;
            if (XPDLHelper.IsSimpleComponentNode(activity.ActivityType))           //可流转简单类型节点
            {
                string name = "单一节点";
                component = new NextActivityItem(name, transition, activity);
            }
            else if (activity.ActivityType == ActivityTypeEnum.SubProcessNode)
            {
                string name = "子流程节点";
                component = new NextActivityItem(name, transition, activity);
            }
            else
            {
                string name = string.Empty;
                if (activity.GatewayDirectionType == GatewayDirectionEnum.AndSplit)
                {
                    name = "必全选节点";
                }
                else
                {
                    name = "或多选节点";
                }

                component = new NextActivityGateway(name, transition, activity);
            }
            return component;
        }

        /// <summary>
        /// 创建跳转节点(强制拉取跳转方式，后续节点状态可以强制拉取前置节点到当前节点[后续节点])
        /// </summary>
        /// <param name="sourceActivity">要拉取的节点</param>
        /// <param name="targetActivity">拉取到节点</param>
        /// <returns></returns>
        internal static NextActivityComponent CreateNextActivityComponent(ActivityEntity sourceActivity,
            ActivityEntity targetActivity)
        {
            NextActivityComponent component = null;
            if (XPDLHelper.IsSimpleComponentNode(sourceActivity.ActivityType) == true)       //可流转简单类型节点
            {
                string name = "单一节点";
                var transition = TransitionBuilder.CreateJumpforwardEmptyTransition(sourceActivity, targetActivity);

                component = new NextActivityItem(name, transition, sourceActivity);     //强制拉取跳转类型的transition 为空类型
            }
            else
            {
                throw new ApplicationException(string.Format("不能跳转到其它非任务类型的节点！当前节点:{0}", 
                    sourceActivity.ActivityType));
            }
            return component;
        }

        /// <summary>
        /// 创建根显示节点
        /// </summary>
        /// <returns></returns>
        internal static NextActivityComponent CreateNextActivityComponent()
        {
            NextActivityComponent root = new NextActivityGateway("下一步步骤列表", null,  null);
            return root;
        }

        /// <summary>
        /// 根据现有下一步节点列表，创建新的下一步节点列表对象
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        internal static NextActivityComponent CreateNextActivityComponent(NextActivityComponent c)
        {
            NextActivityComponent newComp = CreateNextActivityComponent(c.Transition, c.Activity);
            return newComp;
        }
    }
}
