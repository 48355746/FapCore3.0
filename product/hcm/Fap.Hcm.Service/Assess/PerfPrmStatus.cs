using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Hcm.Service.Assess
{
    /// <summary>
    /// 字典表的常量 - 考核方案状态(PerfPrmStatus)
    /// 注意：最好由FAP工具生成，不需要手动修改
    /// 只针对属于系统编码的字典
    /// </summary>
    public static class PerfPrmStatus
    {
        /// <summary>
        /// 初始化
        /// </summary>
        public static string Init = "Init";
        /// <summary>
        /// 发布
        /// </summary>
        public static string Issued = "Issued ";
        /// <summary>
        /// 考核中
        /// </summary>
        public static string Starting = "Starting";
        /// <summary>
        /// 成绩公布
        /// </summary>
        public static string Result = "Result";
        /// <summary>
        /// 已归档
        /// </summary>
        public static string Filed = "Filed";
        /// <summary>
        /// 已关闭
        /// </summary>
        public static string Closed = "Closed";
        /// <summary>
        /// 已结束
        /// </summary>
        public static string Ended = "Ended";

    }
}
