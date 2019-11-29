using Dapper;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Triggers;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Fap.Core.Infrastructure.Config;
using Fap.Core.DataAccess;
using Fap.Core.Extensions;

namespace Fap.Core.Scheduler
{
    /// <summary>
    /// 调度任务管理器
    /// </summary>
    public class JobManager
    {
        private IServiceProvider _provider;
        private ILogger<JobManager> _logger;
        private ILoggerFactory _logFactory;
        private readonly IDbContext _dbContext;
        public JobManager(IServiceProvider provider)
        {
            _provider = provider;
            _logFactory = provider.GetService<ILoggerFactory>();
            _dbContext = provider.GetService<IDbContext>();
            _logger = _logFactory.CreateLogger<JobManager>();
        }
        /// <summary>
        /// 从数据库中加载调度任务
        /// </summary>
        /// <returns></returns>
        public List<JobInfo> LoadJobFromDB()
        {
            List<JobInfo> jobInfos = new List<JobInfo>();

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("JobState", JobState.Enabled);
            IEnumerable<FapJob> jobList = _dbContext.Query<FapJob>($"select * from FapJob where JobState=@JobState", parameters);
            if (jobList != null && jobList.Any())
            {
                foreach (var job in jobList)
                {
                    JobInfo jobInfo = null;
                    //更新执行状态
                    job.ExecState = ExecuteStatus.Execing;
                    jobInfo = this.GetJobInfo(job);
                    if (jobInfo != null)
                    {
                        jobInfos.Add(jobInfo);
                    }
                }
                try
                {
                    _dbContext.BeginTransaction();
                    _dbContext.Execute($"update FapJob set ExecState='{ExecuteStatus.Execing}' where Id in({string.Join(',', jobList.Select(j => j.Id))})");
                    _dbContext.Execute($"update FapJob set ExecState='{ExecuteStatus.NoExec}' where Id not in({string.Join(',', jobList.Select(j => j.Id))})");
                    _dbContext.Commit();

                }
                catch (Exception)
                {
                    _dbContext.Rollback();
                }
            }
            return jobInfos;
        }

        /// <summary>
        /// FapJob对象转换成JobInfo
        /// </summary>
        /// <param name="fapJob"></param>
        /// <returns></returns>
        public JobInfo GetJobInfo(FapJob fapJob)
        {
            string assembly = fapJob.JobClass;
            //执行sql的时候默认用此任务调度
            if (fapJob.ExecSql.IsPresent())
            {
                assembly = "Fap.Core.Scheduler.Jobs.ExecSqlJob,Fap.Core";
            }
            if (fapJob.RestfulApi.IsPresent())
            {
                assembly = "Fap.Core.Scheduler.Jobs.RestfulApiJob,Fap.Core";
            }
            if (assembly.IsMissing())
            {
                return null;
            }
            Type type = Type.GetType(assembly);
            if (type == null)
            {
                return null;
            }
            IJobDetail job = new JobDetailImpl(fapJob.JobCode, fapJob.JobGroup, type) {  Description=fapJob.JobName};
            
            //执行sql，需要把参数放入上下文环境
            if (fapJob.ExecSql.IsPresent())
            {
                job.JobDataMap.Put(JobConstants.JobExecSqlKey, fapJob.ExecSql);

            }
            if (fapJob.RestfulApi.IsPresent())
            {
                job.JobDataMap.Put(JobConstants.JobRestfulApiKey, fapJob.RestfulApi);
            }
            job.JobDataMap.Put(JobConstants.JobIServiceProviderKey, _provider);
            SimpleTriggerImpl trigger = new SimpleTriggerImpl(fapJob.JobCode, fapJob.JobGroup);
            trigger.StartTimeUtc = new DateTimeOffset(GetDateTime(fapJob.StartTime));
            if (fapJob.EndTime.IsPresent())
            {
                trigger.EndTimeUtc = new DateTimeOffset(GetDateTime(fapJob.EndTime));
            }
            trigger.RepeatInterval = GetRepeateInterval(fapJob.RepeatIntervalType, fapJob.RepeatInterval);
            if (fapJob.RepeatCount > 0)
            {
                trigger.RepeatCount = fapJob.RepeatCount;
            }
            else
            {
                trigger.RepeatCount = SimpleTriggerImpl.RepeatIndefinitely;
            }

            return new JobInfo() { Job = job, Trigger = trigger };
        }

        private DateTime GetDateTime(string datetime)
        {
            if (datetime.IsMissing())
            {
                return DateTime.Now;
            }
            else
            {
               return Convert.ToDateTime(datetime);
            }
        }

        private TimeSpan GetRepeateInterval(string repeateIntervalType, int repeateInterval)
        {
            if (JobRepeatIntervalType.Second == repeateIntervalType)
            {
                return TimeSpan.FromSeconds(repeateInterval);
            }
            else if (JobRepeatIntervalType.Minute == repeateIntervalType)
            {
                return TimeSpan.FromMinutes(repeateInterval);
            }
            else if (JobRepeatIntervalType.Hour == repeateIntervalType)
            {
                return TimeSpan.FromHours(repeateInterval);
            }
            else if (JobRepeatIntervalType.Day == repeateIntervalType)
            {
                return TimeSpan.FromDays(repeateInterval);
            }
            else if (JobRepeatIntervalType.Year == repeateIntervalType)
            {
                return TimeSpan.FromDays(repeateInterval * 365);
            }
            else
            {
                return TimeSpan.FromHours(repeateInterval);
            }
        }
       
    }
}

