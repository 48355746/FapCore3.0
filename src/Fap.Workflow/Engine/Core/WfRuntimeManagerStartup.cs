using Fap.Core.DataAccess;
using Fap.Workflow.Engine.Common;
using Fap.Workflow.Engine.Manager;
using Fap.Workflow.Engine.Node;
using Microsoft.Extensions.Logging;

namespace Fap.Workflow.Engine.Core
{
    /// <summary>
    /// 流程启动运行时
    /// </summary>
    internal class WfRuntimeManagerStartup : WfRuntimeManager
    {
        internal WfRuntimeManagerStartup(IDbContext dbContext,  ILoggerFactory loggerFactory) : base(dbContext,  loggerFactory)
        {
        }

        /// <summary>
        /// 启动执行方法
        /// </summary>
        /// <param name="session">会话</param>
        internal override void ExecuteInstanceImp()
        {
            //构造流程实例
            var processInstance = new ProcessInstanceManager(_dataAccessor,  _loggerFactory)
                .CreateProcessInstance(base.AppRunner,
                base.ParentProcessInstance, null,
                (runner,processIns,process)=> { });

            //构造活动实例
            //1. 获取开始节点活动
            var startEntity = base.ProcessModel.GetStartActivity();
            
            var startExecutionContext = ActivityForwardContext.CreateStartupContext(base.ProcessModel,
                processInstance, startEntity);

            NodeMediator mediator = NodeMediatorFactory.CreateNodeMediator(startExecutionContext, base.AppRunner, _dataAccessor,  _loggerFactory);
            mediator.Linker.FromActivityInstance = RunningActivityInstance;
            mediator.ExecuteWorkItem();

            //构造回调函数需要的数据
            WfExecutedResult result = base.WfExecutedResult;
            result.ProcessInsUidStarted = processInstance.Fid;
            result.Status = WfExecutedStatus.Success;
        }
    }
}
