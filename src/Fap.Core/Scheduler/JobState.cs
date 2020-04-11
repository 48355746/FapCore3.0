using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Scheduler
{
    /// <summary>
    /// 字典表的常量 - 调度任务状态(JobState)
    /// 注意：最好由FAP工具生成，不需要手动修改
    /// 只针对属于系统编码的字典
    /// </summary>
    public class JobState
    {
        /// <summary>
        /// 启用
        /// </summary>
        public static string Enabled = "Enabled";
        /// <summary>
        /// 禁用
        /// </summary>
        public static string Forbidden = "Forbidden";
        /// <summary>
        /// 未启用
        /// </summary>
        public static string NoUsing = "NoUsing";

    }
}
