using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Scheduler
{
    /// <summary>
    /// 字典表的常量 - 执行状态(ExecuteStatus)
    /// 注意：最好由FAP工具生成，不需要手动修改
    /// 只针对属于系统编码的字典
    /// </summary>
    public class ExecuteStatus
    {
        /// <summary>
        /// 未执行
        /// </summary>
        public static string NoExec = "NoExec";
        /// <summary>
        /// 执行中
        /// </summary>
        public static string Execing = "Execing";
        /// <summary>
        /// 暂停
        /// </summary>
        public static string Suspend = "Suspend";

    }
}
