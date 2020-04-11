using Fap.Core.DataAccess;
using Fap.Workflow.Engine.Common;
using Fap.Workflow.Engine.Exceptions;
using Fap.Workflow.Engine.Manager;
using Fap.Workflow.Engine.Node;
using Microsoft.Extensions.Logging;
using System;

namespace Fap.Workflow.Engine.Core
{
    /// <summary>
    /// 应用执行运行时
    /// </summary>
    internal class WfRuntimeManagerAppRunning : WfRuntimeManager
    {
        internal WfRuntimeManagerAppRunning(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        /// <summary>
        /// 运行执行方法
        /// </summary>
        /// <param name="session">会话</param>
        internal override void ExecuteInstanceImp()
        {
            try
            {
                var result = base.WfExecutedResult;
                var processIns = new ProcessInstanceManager(_serviceProvider).GetByFid(base.TaskView.ProcessInsUid);
                var fromActivityIns = new ActivityInstanceManager(_serviceProvider).GetByFid(base.TaskView.ActivityInsUid);

                var runningExecutionContext = ActivityForwardContext.CreateRunningContext(base.TaskView,
                    base.ProcessModel, processIns, fromActivityIns);

                //执行节点
                NodeMediator mediator = NodeMediatorFactory.CreateNodeMediator(runningExecutionContext, AppRunner,_serviceProvider);
                mediator.Linker.FromActivityInstance = RunningActivityInstance;
                mediator.ExecuteWorkItem();

                //构造回调函数需要的数据
                result.Status = WfExecutedStatus.Success;
                result.Message = mediator.GetNodeMediatedMessage();

            }
            catch (WfRuntimeException rx)
            {
                var result = base.WfExecutedResult;
                result.Status = WfExecutedStatus.Failed;
                result.ExceptionType = WfExceptionType.RunApp_RuntimeError;
                result.Message = rx.Message;
                throw rx;
            }
            catch (System.Exception ex)
            {
                var result = base.WfExecutedResult;
                result.Status = WfExecutedStatus.Failed;
                result.ExceptionType = WfExceptionType.RunApp_RuntimeError;
                result.Message = ex.Message;
                throw ex;
            }
        }
    }
}
