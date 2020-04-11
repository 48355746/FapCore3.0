using Dapper.Contrib.Extensions;
using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Scheduler
{
    /// <summary>
    /// 调度任务
    /// </summary>
    public class FapJob : BaseModel
    {
        /// <summary>
        /// 任务名
        /// </summary>
        public string JobName { get; set; }
        /// <summary>
        /// 任务编号
        /// </summary>
        public string JobCode { get; set; }
        /// <summary>
        /// 执行状态
        /// </summary>
        public string ExecState { get; set; }
        /// <summary>
        /// 执行状态 的显性字段MC
        /// </summary>
        [Computed]
        public string ExecStateMC { get; set; }
        /// <summary>
        /// 任务分组
        /// </summary>
        public string JobGroup { get; set; }
        /// <summary>
        /// 任务分组 的显性字段MC
        /// </summary>
        [Computed]
        public string JobGroupMC { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public string StartTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public string EndTime { get; set; }
        /// <summary>
        /// 重复间隔
        /// </summary>
        public int RepeatInterval { get; set; }
        /// <summary>
        /// 重复间隔类型
        /// </summary>
        public string RepeatIntervalType { get; set; }
        /// <summary>
        /// 重复间隔类型 的显性字段MC
        /// </summary>
        [Computed]
        public string RepeatIntervalTypeMC { get; set; }
        /// <summary>
        /// 重复次数
        /// </summary>
        public int RepeatCount { get; set; }
        /// <summary>
        /// 任务实现类
        /// </summary>
        public string JobClass { get; set; }
        /// <summary>
        /// 执行sql
        /// </summary>
        public string ExecSql { get; set; }
        /// <summary>
        /// GetUrl
        /// </summary>
        public string RestfulApi { get; set; }
        /// <summary>
        /// 任务状态
        /// </summary>
        public string JobState { get; set; }
        /// <summary>
        /// 任务状态 的显性字段MC
        /// </summary>
        [Computed]
        public string JobStateMC { get; set; }

    }
}
