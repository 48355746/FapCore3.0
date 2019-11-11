using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Infrastructure.Enums
{
    /// <summary>
    /// 字典表的常量 - 单据状态(BillStatus)
    /// 注意：最好由FAP工具生成，不需要手动修改
    /// 只针对属于系统编码的字典
    /// </summary>
    public class BillStatus
    {
        /// <summary>
        /// 草稿
        /// </summary>
        public static string DRAFT = "DRAFT";
        /// <summary>
        /// 流程中
        /// </summary>
        public static string PROCESSING = "PROCESSING";
        /// <summary>
        /// 通过
        /// </summary>
        public static string PASSED = "PASSED";
        /// <summary>
        /// 否决
        /// </summary>
        public static string REJECTED = "REJECTED";
        /// <summary>
        /// 撤回
        /// </summary>
        public static string RECALL = "RECALL";
        /// <summary>
        /// 挂起
        /// </summary>
        public static string SUSPENDED = "SUSPENDED";
        /// <summary>
        /// 退回
        /// </summary>
        public static string BACKED = "BACKED";
        /// <summary>
        /// 作废
        /// </summary>
        public static string CANCELED = "CANCELED";
        /// <summary>
        /// 终止
        /// </summary>
        public static string ENDED = "ENDED";
        /// <summary>
        /// 关闭
        /// </summary>
        public static string CLOSED = "CLOSED";
        /// <summary>
        /// 驳回
        /// </summary>
        public static string REVOKED = "REVOKED";
        /// <summary>
        /// 撤销
        /// </summary>
        public static string WITHDRAWED = "WITHDRAWED";


    }
}
