using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Hcm.Service.Recruit
{
    /// <summary>
    /// 字典表的常量 - 招聘简历状态(RcrtResumeStatus)
    /// 注意：最好由FAP工具生成，不需要手动修改
    /// 只针对属于系统编码的字典
    /// </summary>
    public class RcrtResumeStatus
    {
        /// <summary>
        /// 简历被创建
        /// </summary>
        public static string Created = "Created";
        /// <summary>
        /// 简历被筛选
        /// </summary>
        public static string Screen = "Screen";
        /// <summary>
        /// 批准面试
        /// </summary>
        public static string Interview = "Interview";
        /// <summary>
        /// 现场面试
        /// </summary>
        public static string Interviewing = "Interviewing";
        /// <summary>
        /// 发出Offer
        /// </summary>
        public static string Offer = "Offer";
        /// <summary>
        /// 入职
        /// </summary>
        public static string Entry = "Entry";
        /// <summary>
        /// 黑名单
        /// </summary>
        public static string BlackList = "BlackList";
        /// <summary>
        /// 优才库
        /// </summary>
        public static string TalentPool = "TalentPool";
        /// <summary>
        /// 后备人才
        /// </summary>
        public static string Reserve = "Reserve";

    }

}
