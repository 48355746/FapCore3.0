using Fap.Core.DataAccess;
using Fap.Workflow.Engine.Common;
using Fap.Workflow.Engine.Manager;
using Microsoft.Extensions.Logging;
using System;

namespace Fap.Workflow.Engine.Core
{
    /// <summary>
    /// 任务撤销运行时
    /// </summary>
    internal class WfRuntimeManagerWithdraw : WfRuntimeManager
    {
        internal WfRuntimeManagerWithdraw(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        /// <summary>
        /// 撤销处理具体功能实现
        /// </summary>
        /// <param name="session">会话</param>
        internal override void ExecuteInstanceImp()
        {
            //如果审批人还没有人审批 则可以撤销回草稿态
            ProcessInstanceManager manager = new ProcessInstanceManager(_serviceProvider);
            manager.Withdrawn(AppRunner);
            
            //构造回调函数需要的数据
            WfExecutedResult result = base.WfExecutedResult;
            //result.BackwardTaskReciever = base.BackwardContext.BackwardTaskReciever;
            result.Status = WfExecutedStatus.Success;
        }
    }
}
