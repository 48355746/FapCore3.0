
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using Quartz.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace Fap.Core.Scheduler
{
    /// <summary>
    /// 任务调度管理器
    /// </summary>
    public class SchedulerManager
    {
        // 定义一个静态变量来保存类的实例
        public static SchedulerManager Instance { get; } = new SchedulerManager();
        // 定义一个标识确保线程同步
        private static readonly object locker = new object();
        private ISchedulerFactory schedulerFactory;
        private IScheduler scheduler;
        private SchedulerManager()
        {
        }
        public async Task Init()
        {
            NameValueCollection properties = new NameValueCollection
            {
                ["quartz.scheduler.instanceName"] = "FAPScheduler",
                ["quartz.threadPool.type"] = "Quartz.Simpl.SimpleThreadPool, Quartz",
                ["quartz.threadPool.threadCount"] = "10",
                ["quartz.jobStore.misfireThreshold"] = "60000",
                ["quartz.jobStore.type"] = "Quartz.Simpl.RAMJobStore, Quartz",
                ["quartz.threadPool.threadPriority"] = "2"
            };
            schedulerFactory = new StdSchedulerFactory(properties);
            scheduler = await schedulerFactory.GetScheduler();
        }
        /// <summary>
        /// 加入调度任务
        /// </summary>
        /// <param name="jobs"></param>
        public async Task AddScheduleJob(IList<JobInfo> jobs)
        {
            if (jobs == null || jobs.Count == 0)
            {
                return;
            }
            foreach (var job in jobs)
            {
                await scheduler.ScheduleJob(job.Job, job.Trigger);
            }

        }

        /// <summary>
        /// 加入调度任务
        /// </summary>
        /// <param name="job"></param>
        public async Task AddScheduleJob(JobInfo job)
        {
            if (job == null)
            {
                return;
            }
            bool isExist = await scheduler.CheckExists(job.Job.Key);
            if (!isExist)
            {
                await scheduler.ScheduleJob(job.Job, job.Trigger);
            }
        }

        public Task ScheduleJob(IJobDetail jobDetail, ITrigger trigger)
        {

           return scheduler.ScheduleJob(jobDetail, trigger);

        }

        /// <summary>
        /// 开始调度
        /// </summary>
        public void Run()
        {
            if (scheduler != null && !scheduler.IsStarted)
            {
                scheduler.Start();
            }

        }

        ///<summary>
        /// 停止调度
        ///</summary>
        public void Shutdown()
        {
            if (scheduler != null && !scheduler.IsShutdown)
            {
                scheduler.Shutdown();
            }
        }


        ///
        /// 备用
        ///
        public void Standby()
        {
            scheduler.Standby();
        }


        ///
        /// 暂停所有
        ///
        public void PauseAll()
        {
            scheduler.PauseAll();
        }


        ///
        /// 重启所有
        ///
        public void ResumeAll()
        {
            scheduler.ResumeAll();
        }


        ///
        /// 重启
        ///
        /// 组名
        public void ResumeJobGroup(string groupName)
        {
            scheduler.ResumeJobs(GroupMatcher<JobKey>.GroupEquals(groupName));
        }


        ///
        ///  重启
        ///
        /// 组名
        public void ResumeTriggerGroup(string groupName)
        {
            scheduler.ResumeTriggers(GroupMatcher<TriggerKey>.GroupEquals(groupName));
        }


        ///
        /// 根据组名暂停作业
        ///
        /// 组名
        public void PauseJobGroup(string groupName)
        {
            scheduler.PauseJobs(GroupMatcher<JobKey>.GroupEquals(groupName));
        }


        ///
        /// 根据组名暂停触发器
        ///
        /// 组名
        public void PauseTriggerGroup(string groupName)
        {
            scheduler.PauseTriggers(GroupMatcher<TriggerKey>.GroupEquals(groupName));
        }


        ///
        /// 移除全局作业监听
        ///
        /// 监听名称
        public void RemoveGlobalJobListener(string name)
        {
            scheduler.ListenerManager.RemoveJobListener(name);
        }


        ///
        /// 移除全局触发器监听
        ///
        /// 监听名称
        public void RemoveGlobalTriggerListener(string name)
        {
            scheduler.ListenerManager.RemoveTriggerListener(name);
        }


        ///
        /// 通过名称分组删除作业
        ///
        /// 作业名
        /// 分组名
        public Task<bool> DeleteJob(string jobName, string groupName)
        {
            return scheduler.DeleteJob(new JobKey(jobName, groupName));
        }


        ///
        /// 通过名称分组暂停作业
        ///
        /// 作业名
        /// 分组名
        public void PauseJob(string jobName, string groupName)
        {
            scheduler.PauseJob(new JobKey(jobName, groupName));

        }

        /// <summary>
        /// 移除一个任务
        /// </summary>
        /// <param name="jobName">任务名称</param>
        /// <param name="groupName">分组名</param>
        public void RemoveJob(string jobName, string groupName)
        {
            JobKey jobKey = new JobKey(jobName);
            TriggerKey triggerKey = new TriggerKey(jobName, groupName);
            scheduler.PauseTrigger(triggerKey);// 停止触发器  
            scheduler.UnscheduleJob(triggerKey);// 移除触发器 

        }

        /// <summary>
        /// 重启
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="groupName"></param>
        public void ResumeJob(string jobName, string groupName)
        {
            scheduler.ResumeJob(new JobKey(jobName, groupName));


        }


        //把作业与触发器添加到调度里去
        public void TriggerJob(string jobName, string groupName)
        {
            scheduler.TriggerJob(new JobKey(jobName, groupName));
        }


        public void Interrupt(string jobName, string groupName)
        {
            scheduler.Interrupt(new JobKey(jobName, groupName));
        }


        public void ResumeTrigger(string triggerName, string groupName)
        {
            scheduler.ResumeTrigger(new TriggerKey(triggerName, groupName));
        }


        public void PauseTrigger(string triggerName, string groupName)
        {
            scheduler.PauseTrigger(new TriggerKey(triggerName, groupName));
        }


        public void UnscheduleJob(string triggerName, string groupName)
        {
            scheduler.UnscheduleJob(new TriggerKey(triggerName, groupName));
        }


        ///
        /// 判断某组作业是否被暂停
        ///
        /// 组名
        ///
        public Task<bool> IsJobGroupPaused(string groupName)
        {
            try
            {
                return scheduler.IsJobGroupPaused(groupName);
            }
            catch (NotImplementedException)
            {
                return null;
            }
        }
        ///
        /// 判断某组触发器是否暂停
        ///
        ///
        ///
        public Task<bool> IsTriggerGroupPaused(string groupName)
        {
            try
            {
                return scheduler.IsTriggerGroupPaused(groupName);
            }
            catch (NotImplementedException)
            {
                return null;
            }
        }


        ///
        /// 返回运行作业集合
        ///
        ///
        public Task<IReadOnlyCollection<IJobExecutionContext>> GetCurrentlyExecutingJobs()
        {
            return scheduler.GetCurrentlyExecutingJobs();
        }


        ///
        /// 获取所有作业键集合
        ///
        ///
        ///
        public Task<IReadOnlyCollection<JobKey>> GetJobKeys(GroupMatcher<JobKey> key)
        {
            return scheduler.GetJobKeys(key);
        }


        ///
        /// 获取某个作业
        ///
        ///
        ///
        public Task<IJobDetail> GetJobDetail(JobKey key)
        {
            return scheduler.GetJobDetail(key);
        }


        ///
        /// 获取触发器组名
        ///
        ///
        public Task<IReadOnlyCollection<string>> GetTriggerGroupNames()
        {
            return scheduler.GetTriggerGroupNames();
        }


        ///
        /// 获取工作名
        ///
        ///
        public Task<IReadOnlyCollection<string>> GetJobGroupNames()
        {
            return scheduler.GetJobGroupNames();
        }



        public Task<IReadOnlyCollection<string>> GetCalendarNames()
        {
            return scheduler.GetCalendarNames();
        }


        public IListenerManager ListenerManager
        {
            get
            {
                return scheduler.ListenerManager;
            }
        }


        public Task<ICalendar> GetCalendar(string name)
        {
            return scheduler.GetCalendar(name);
        }


        public Task<SchedulerMetaData> GetMetaData()
        {
            return scheduler.GetMetaData();
        }


        public Task<IReadOnlyCollection<ITrigger>> GetTriggersOfJob(JobKey jobKey)
        {
            try
            {
                return scheduler.GetTriggersOfJob(jobKey);
            }
            catch (NotImplementedException)
            {
                return null;
            }
        }


        public Task<IReadOnlyCollection<TriggerKey>> GetTriggerKeys(GroupMatcher<TriggerKey> matcher)
        {
            try
            {
                return scheduler.GetTriggerKeys(matcher);
            }
            catch (NotImplementedException)
            {
                return null;
            }
        }


        public Task<ITrigger> GetTrigger(TriggerKey triggerKey)
        {
            return scheduler.GetTrigger(triggerKey);
        }


        public Task<TriggerState> GetTriggerState(TriggerKey triggerKey)
        {
            return scheduler.GetTriggerState(triggerKey);
        }


        public string SchedulerName
        {
            get
            {
                return scheduler.SchedulerName;
            }
        }


        public bool InStandbyMode
        {
            get
            {
                return scheduler.InStandbyMode;
            }
        }

    }
}
