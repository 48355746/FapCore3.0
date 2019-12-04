using Fap.Core.DataAccess;
using Fap.Workflow.Engine.Common;
using Fap.Workflow.Engine.Core;
using Fap.Workflow.Engine.Enums;
using Microsoft.Extensions.Logging;
using System;

namespace Fap.Workflow.Engine.Node
{
    /// <summary>
    /// 节点执行器的工厂类
    /// </summary>
    internal class NodeMediatorFactory
    {
        /// <summary>
        /// 创建节点执行器的抽象类
        /// </summary>
        /// <param name="forwardContext">活动上下文</param>
        /// <param name="session">会话</param>
        /// <returns></returns>
        internal static NodeMediator CreateNodeMediator(ActivityForwardContext forwardContext,WfAppRunner runner,IDbContext dbContext,ILoggerFactory loggerFactory)
        {
            var activityType=forwardContext.Activity.ActivityType;
            if (activityType == ActivityTypeEnum.StartNode)         //开始节点
            {
                return new NodeMediatorStart(forwardContext, runner,dbContext,loggerFactory);
            }
            else if (activityType == ActivityTypeEnum.TaskNode|| activityType== ActivityTypeEnum.TimerNode||activityType== ActivityTypeEnum.SignNode)         //任务节点
            {
                return new NodeMediatorTask(forwardContext, runner,dbContext,loggerFactory);
            }
            else if (forwardContext.Activity.ActivityType == ActivityTypeEnum.SubProcessNode)
            {
                //return new NodeMediatorSubProcess(forwardContext, session);
            }
            else
            {
                throw new ApplicationException(string.Format("不明确的节点类型: {0}", forwardContext.Activity.ActivityType.ToString()));
            }
            return null;
        }
    }
}
