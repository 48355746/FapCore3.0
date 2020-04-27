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
    public static class RcrtResumeStatus
    {
        /// <summary>
        /// 简历被创建
        /// </summary>
        public const string Created = "Created";
        /// <summary>
        /// 简历被筛选
        /// </summary>
        public const string Screen = "Screen";
        /// <summary>
        /// 批准面试
        /// </summary>
        public const string Interview = "Interview";
        /// <summary>
        /// 现场面试
        /// </summary>
        public const string Interviewing = "Interviewing";
        /// <summary>
        /// 发出Offer
        /// </summary>
        public const string Offer = "Offer";
        /// <summary>
        /// 入职
        /// </summary>
        public const string Entry = "Entry";
        /// <summary>
        /// 黑名单
        /// </summary>
        public const string BlackList = "BlackList";
        /// <summary>
        /// 优才库
        /// </summary>
        public const string TalentPool = "TalentPool";
        /// <summary>
        /// 后备人才
        /// </summary>
        public const string Reserve = "Reserve";

    }

}
