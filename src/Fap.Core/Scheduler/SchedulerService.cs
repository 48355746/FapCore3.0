using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Fap.Core.DI;

namespace Fap.Core.Scheduler
{
    /// <summary>
    /// 任务调度工厂类
    /// </summary>
    [Service]
    public class SchedulerService : ISchedulerService
    {
        //private static object obj = new object();

        private readonly ILogger<SchedulerService> _logger;
        private readonly IServiceProvider _serviceProvider;
        public SchedulerService(IServiceProvider serviceProvider)
        {
            _logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger<SchedulerService>();
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public async void Run()
        {
            _logger.LogInformation("启动计划任务starting");
            await SchedulerManager.Instance.Init();

            var jobManager = new JobManager(_serviceProvider);
            List<JobInfo> jobInfoList = jobManager.LoadJobFromDB();
            await SchedulerManager.Instance.AddScheduleJob(jobInfoList);

            SchedulerManager.Instance.Run();
            _logger.LogInformation("启动计划任务结束");
        }

        /// <summary>
        /// 启动所有定时任务 
        /// </summary>
        public void StartAllJobs()
        {
            SchedulerManager.Instance.Run();
        }

        /// <summary>
        /// 添加一个定时任务,仅仅不存在时会添加
        /// </summary>
        /// <param name="fapJob"></param>
        public async void AddJob(FapJob fapJob)
        {
            var jobManager = new JobManager(_serviceProvider);
            JobInfo job = jobManager.GetJobInfo(fapJob);
            await SchedulerManager.Instance.AddScheduleJob(job);
        }

        /// <summary>
        /// 暂停一个定时任务
        /// </summary>
        /// <param name="fapJob"></param>
        public void PauseJob(FapJob fapJob)
        {
            SchedulerManager.Instance.PauseJob(fapJob.JobCode, fapJob.JobGroup);
        }

        /// <summary>
        /// 重启一个定时任务
        /// </summary>
        /// <param name="fapJob"></param>
        public void ResumeJob(FapJob fapJob)
        {
            SchedulerManager.Instance.ResumeJob(fapJob.JobCode, fapJob.JobGroup);
        }

        /// <summary>
        /// 删除一个定时任务
        /// </summary>
        /// <param name="fapJob"></param>
        public void RemoveJob(FapJob fapJob)
        {
            SchedulerManager.Instance.RemoveJob(fapJob.JobCode, fapJob.JobGroup);
        }

        /// <summary>
        /// 关闭所有定时任务
        /// </summary>
        public void ShutdownJobs()
        {
            SchedulerManager.Instance.Shutdown();
        }
    }
}
