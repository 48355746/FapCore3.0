using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Scheduler
{
    /// <summary>
    /// 字典表的常量 - 调度任务重复间隔类型(JobRepeatIntervalType)
	/// 注意：最好由FAP工具生成，不需要手动修改
	/// 只针对属于系统编码的字典
    /// </summary>
    public class JobRepeatIntervalType
    {
        /// <summary>
        /// 秒
        /// </summary>
        public static string Second = "Second";
        /// <summary>
        /// 分
        /// </summary>
        public static string Minute = "Minute";
        /// <summary>
        /// 小时
        /// </summary>
        public static string Hour = "Hour";
        /// <summary>
        /// 天
        /// </summary>
        public static string Day = "Day";
        /// <summary>
        /// 周
        /// </summary>
        public static string Week = "Week";
        /// <summary>
        /// 月
        /// </summary>
        public static string Month = "Month";
        /// <summary>
        /// 年
        /// </summary>
        public static string Year = "Year";

    }
}
