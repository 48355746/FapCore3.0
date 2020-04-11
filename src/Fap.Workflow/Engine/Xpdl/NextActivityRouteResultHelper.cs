﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fap.Workflow.Engine.Common;
using Fap.Workflow.Engine.Enums;

namespace Fap.Workflow.Engine.Xpdl
{
    /// <summary>
    /// 下一步活动路由列表的帮助类
    /// </summary>
    internal class NextActivityRouteResultHelper
    {
        internal static NextActivityRouteResult CreateNextActivityRouteResult(ActivityEntity activity)
        {
            NextActivityRouteResult result;
            if (activity.ActivityType == ActivityTypeEnum.GatewayNode)
            {
                if (activity.GatewayDirectionType == GatewayDirectionEnum.AndSplit)
                {
                    result = new NextActivityRouteResult(NextActivityRouteChoiceEnum.MustAll);
                }
                else
                {
                    result = new NextActivityRouteResult(NextActivityRouteChoiceEnum.OrMultiple);
                }
                result.GatewayActivity = activity;
            }
            else
            {
                result = new NextActivityRouteResult(NextActivityRouteChoiceEnum.Single);
                result.NormalActivity = activity;
            }
            return result;
        }     

        /// <summary>
        /// 给下一步列表中继续添加子节点
        /// </summary>
        /// <param name="nextActivityRoutedResult"></param>
        /// <param name="child"></param>
        internal static void AddNewNextActivityToRoutedList(ref NextActivityRouteResult nextActivityRoutedResult,
            NextActivityRouteResult child)
        {
            //初始化列表
            if (nextActivityRoutedResult.Children == null)
            {
                nextActivityRoutedResult.Children = new List<NextActivityRouteResult>();
            }

            //添加子节点
            if (child != null)
            {
                nextActivityRoutedResult.Children.Add(child);
            }
        }
    }
}
