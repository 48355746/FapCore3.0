using Fap.Core.DataAccess;
using Fap.Workflow.Engine.Common;
using Fap.Workflow.Engine.Core;
using Fap.Workflow.Engine.Enums;
using Fap.Workflow.Engine.Manager;
using Fap.Workflow.Engine.Node;
using Fap.Workflow.Engine.Xpdl;
using Fap.Workflow.Model;
using Microsoft.Extensions.Logging;

namespace Fap.Wrokflow.Engine.Node
{
    /// <summary>
    /// 结束节点处理类
    /// </summary>
    internal class NodeMediatorEnd : NodeMediator
    {
        internal NodeMediatorEnd(ActivityForwardContext forwardContext,WfAppRunner runner, IDbContext dbContext,ILoggerFactory loggerFactory)
            : base(forwardContext, runner,dbContext,loggerFactory)
        {
            
        }

        /// <summary>
        /// 节点内部业务逻辑执行
        /// </summary>
        internal override void ExecuteWorkItem()
        {
            //执行Action列表
            //ExecteActionList(Linker.ToActivity.ActionList, 
            //    ActivityForwardContext.ActivityResource.AppRunner.ActionMethodParameters);

            //设置流程完成
            ProcessInstanceManager pim = new ProcessInstanceManager(_dataAccessor,_loggerFactory);
            pim.Complete(ActivityForwardContext.ProcessInstance.Fid,AppRunner.BillUid);
        }

        /// <summary>
        /// 结束节点活动及转移实例化，没有任务数据
        /// </summary>
        /// <param name="toActivity">当前Activity</param>
        /// <param name="processInstance">流程实例</param>
        /// <param name="fromActivityInstance">起始活动实例</param>
        /// <param name="transitionGUID">转移GUID</param>
        /// <param name="transitionType">转移类型</param>
        /// <param name="flyingType">跳跃类型</param>
        /// <param name="activityResource">活动资源</param>
        /// <param name="session">Session</param>
        internal override void CreateActivityTaskTransitionInstance(ActivityEntity toActivity,
            WfProcessInstance processInstance,
            WfActivityInstance fromActivityInstance,
            string transitionGUID,
            TransitionTypeEnum transitionType,
            TransitionFlyingTypeEnum flyingType)
        {
            var toActivityInstance = base.CreateActivityInstanceObject(toActivity,
                processInstance, AppRunner);

            base.ActivityInstanceManager.Insert(toActivityInstance);

            base.ActivityInstanceManager.Complete(toActivityInstance.Fid,
                AppRunner);

            //写节点转移实例数据
            base.InsertTransitionInstance(processInstance,
                transitionGUID,
                fromActivityInstance,
                toActivityInstance,
                transitionType,
                flyingType,AppRunner);
        }
    }
}
