using Fap.Core.DataAccess;
using Fap.Workflow.Engine.Common;
using Fap.Workflow.Engine.Core;
using Fap.Workflow.Engine.Enums;
using Fap.Workflow.Engine.Manager;
using Fap.Workflow.Engine.Xpdl;
using Fap.Workflow.Model;
using Fap.Wrokflow.Engine.Node;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Fap.Workflow.Engine.Node
{
    /// <summary>
    /// 开始节点执行器
    /// </summary>
    internal class NodeMediatorStart : NodeMediator
    {
        internal NodeMediatorStart(ActivityForwardContext forwardContext,WfAppRunner appRunner, IDbContext dbContext,ILoggerFactory loggerFactory)
            : base(forwardContext, appRunner,dbContext,loggerFactory)
        {

        }

        /// <summary>
        /// 执行开始节点
        /// </summary>
        internal override void ExecuteWorkItem()
        {
            try
            {
                //写入流程实例
                ProcessInstanceManager pim = new ProcessInstanceManager(_dataAccessor,_loggerFactory);
                var newInstance = pim.Insert(ActivityForwardContext.ProcessInstance);
                //写入流程图实例
                DiagramInstanceManager dim = new DiagramInstanceManager(_dataAccessor,  _loggerFactory);
                dim.Insert(dim.CreateDiagramInstance(newInstance.ProcessUid, newInstance.Fid));

                ActivityForwardContext.ProcessInstance.Id = newInstance.Id;

                CompleteAutomaticlly(ActivityForwardContext.ProcessInstance,
                    ActivityForwardContext.Activity,base.AppRunner);

                //执行开始节点之后的节点集合
                ContinueForwardCurrentNode(false);

                //执行Action列表
                //ExecteActionList(ActivityForwardContext.Activity.ActionList,
                //    ActivityForwardContext.ActivityResource.AppRunner.ActionMethodParameters);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 执行外部操作的方法
        /// </summary>
        /// <param name="actionList"></param>
        /// <param name="actionMethodParameters"></param>
        public void ExecteActionList(IList<ActionEntity> actionList, IDictionary<string, ActionParameterInternal> actionMethodParameters)
        {
            if (actionList != null && actionList.Count > 0)
            {
                var actionExecutor = new ActionExecutor();
                actionExecutor.ExecteActionList(actionList, actionMethodParameters);
            }
        }

        /// <summary>
        /// 置开始节点为结束状态
        /// </summary>
        /// <param name="processInstance"></param>
        /// <param name="activityResource"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        private GatewayExecutedResult CompleteAutomaticlly(WfProcessInstance processInstance,ActivityEntity activity,WfAppRunner runner)
        {
            //开始节点没前驱信息
            var fromActivityInstance = base.CreateActivityInstanceObject(base.Linker.FromActivity, processInstance,runner);

            var activityIns= base.ActivityInstanceManager.Insert(fromActivityInstance);

            base.ActivityInstanceManager.Complete(fromActivityInstance.Fid, runner);

            fromActivityInstance.ActivityState = ActivityStateEnum.Completed.ToString();
            base.Linker.FromActivityInstance = fromActivityInstance;

            GatewayExecutedResult result = GatewayExecutedResult.CreateGatewayExecutedResult(GatewayExecutedStatus.Successed);
            return result;
        }
    }
    }
