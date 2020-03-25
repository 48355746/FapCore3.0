using Fap.Core.DataAccess;
using Fap.Workflow.Engine.Common;
using Fap.Workflow.Engine.Enums;
using Fap.Workflow.Engine.Xpdl;
using Fap.Workflow.Engine.Xpdl.Exceptions;
using Microsoft.Extensions.Logging;
using System;

namespace Fap.Wrokflow.Engine.Node
{
    /// <summary>
    /// Gateway 工厂类
    /// </summary>
    internal class NodeMediatorGatewayFactory
    {
        /// <summary>
        /// 创建Gateway节点处理类实例
        /// </summary>
        /// <param name="gActivity">节点</param>
        /// <param name="processModel">流程模型类</param>
        /// <param name="session">会话</param>
        /// <returns>Gateway节点处理实例</returns>
        internal static NodeMediatorGateway CreateGatewayNodeMediator(ActivityEntity gActivity, 
            IProcessModel processModel,
            WfAppRunner runner, IServiceProvider serviceProvider)
        {
            NodeMediatorGateway nodeMediator = null;
            if (gActivity.ActivityType == ActivityTypeEnum.GatewayNode)
            {
                if (gActivity.GatewayDirectionType == GatewayDirectionEnum.AndSplit)
                {
                    nodeMediator = new NodeMediatorAndSplit(gActivity, processModel, runner,serviceProvider);
                }
                else if (gActivity.GatewayDirectionType == GatewayDirectionEnum.OrSplit)
                {
                    nodeMediator = new NodeMediatorOrSplit(gActivity, processModel, runner,serviceProvider);
                }
                else if (gActivity.GatewayDirectionType == GatewayDirectionEnum.XOrSplit)
                {
                    nodeMediator = new NodeMediatorXOrSplit(gActivity, processModel, runner,serviceProvider);
                }
                else if (gActivity.GatewayDirectionType == GatewayDirectionEnum.AndJoin)
                {
                    nodeMediator = new NodeMediatorAndJoin(gActivity, processModel, runner,serviceProvider);
                }
                else if (gActivity.GatewayDirectionType == GatewayDirectionEnum.OrJoin)
                {
                    nodeMediator = new NodeMediatorOrJoin(gActivity, processModel, runner,serviceProvider);
                }
                else if (gActivity.GatewayDirectionType == GatewayDirectionEnum.XOrJoin)
                {
                    nodeMediator = new NodeMediatorXOrJoin(gActivity, processModel, runner,serviceProvider);
                }
                else
                {
                    throw new XmlDefinitionException(string.Format("不明确的节点分支Gateway类型！{0}", gActivity.GatewayDirectionType.ToString()));
                }
            }
            else
            {
                throw new XmlDefinitionException(string.Format("不明确的节点类型！{0}", gActivity.ActivityType.ToString()));
            }
            return nodeMediator;
        }
    }
}
