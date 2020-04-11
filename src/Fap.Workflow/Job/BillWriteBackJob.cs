using Fap.Core.DataAccess;
using Fap.Core.Scheduler;
using Fap.Workflow.Engine.WriteBack;
using Fap.Workflow.Model;
using Fap.Workflow.Service;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fap.Workflow.Job
{
    public class BillWriteBackJob : IJob
    {
        Task IJob.Execute(IJobExecutionContext context)
        {
            IServiceProvider serviceProvider = context.JobDetail.JobDataMap.Get(JobConstants.JobIServiceProviderKey) as IServiceProvider;
            IWorkflowService workflowService = serviceProvider.GetService<IWorkflowService>();
            workflowService.BillWriteBack();

            return Task.CompletedTask;
        }
    }
}
