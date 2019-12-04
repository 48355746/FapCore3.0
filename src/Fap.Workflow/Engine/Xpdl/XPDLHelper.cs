using System;
using Fap.Workflow.Engine.Enums;

namespace Fap.Workflow.Engine.Xpdl
{
    /// <summary>
    /// 常用的一些帮助方法
    /// </summary>
    internal class XPDLHelper
    {
        /// <summary>
        /// 是否简单组件节点
        /// </summary>
        /// <param name="acitivytType">活动类型</param>
        /// <returns>判断结果</returns>
        internal static Boolean IsSimpleComponentNode(ActivityTypeEnum activityType)
        {
            return activityType == ActivityTypeEnum.TaskNode
                    || activityType == ActivityTypeEnum.SignNode
                    || activityType == ActivityTypeEnum.TimerNode
                    || activityType == ActivityTypeEnum.ScriptNode
                    || activityType == ActivityTypeEnum.PluginNode
                    || activityType == ActivityTypeEnum.StartNode
                    || activityType == ActivityTypeEnum.EndNode;
           
        }

        /// <summary>
        /// 是否是可办理的任务节点
        /// </summary>
        internal static Boolean IsWorkItem(ActivityTypeEnum activityType)
        {
            return activityType == ActivityTypeEnum.TaskNode
                    || activityType == ActivityTypeEnum.SignNode
                    || activityType == ActivityTypeEnum.TimerNode
                    ||activityType==ActivityTypeEnum.SubProcessNode
                    || activityType == ActivityTypeEnum.EndNode ;
        }

        /// <summary>
        /// 根据活动类型获取工作项类型
        /// </summary>
        /// <param name="activityType">活动类型</param>
        /// <returns>工作项类型</returns>
        internal static WorkItemTypeEnum GetWorkItemType(ActivityTypeEnum activityType)
        {
            WorkItemTypeEnum workItemType = WorkItemTypeEnum.NonWorkItem;

            if (IsWorkItem(activityType))
            {
                workItemType = WorkItemTypeEnum.IsWorkItem;
            }
            return workItemType;
        }
    }
}
