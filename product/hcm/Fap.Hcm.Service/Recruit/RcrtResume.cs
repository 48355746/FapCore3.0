using Dapper.Contrib.Extensions;
using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Hcm.Service.Recruit
{
    /// <summary>
    /// 招聘简历
    /// </summary>
    public class RcrtResume : BaseModel
    {
        /// <summary>
        /// 招聘职位名称
        /// </summary>
        public string ResumeName { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string FullName { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public string Gender { get; set; }
        /// <summary>
        /// 性别 的显性字段MC
        /// </summary>
        [Computed]
        public string GenderMC { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Emails { get; set; }
        /// <summary>
        /// 手机
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 学历
        /// </summary>
        public string Education { get; set; }
        /// <summary>
        /// 毕业时间
        /// </summary>
        public string GraduationDate { get; set; }
        /// <summary>
        /// 毕业院校
        /// </summary>
        public string University { get; set; }
        /// <summary>
        /// 现住址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 专业
        /// </summary>
        public string Major { get; set; }
        /// <summary>
        /// 工作年限
        /// </summary>
        public string WorkLift { get; set; }
        /// <summary>
        /// 简历来源
        /// </summary>
        public string Source { get; set; }
        /// <summary>
        /// 教育背景
        /// </summary>
        public string Educational { get; set; }
        /// <summary>
        /// 工作经历经验
        /// </summary>
        public string Experience { get; set; }
        /// <summary>
        /// HTML内容
        /// </summary>
        public string HtmlContent { get; set; }
        /// <summary>
        /// 推荐人
        /// </summary>
        public string EmpUid { get; set; }
        /// <summary>
        /// 推荐人 的显性字段MC
        /// </summary>
        [Computed]
        public string EmpUidMC { get; set; }
        /// <summary>
        /// 推荐人手机
        /// </summary>
        public string EmpPhone { get; set; }
        /// <summary>
        /// 推荐人邮箱
        /// </summary>
        public string EmpEmail { get; set; }
        /// <summary>
        /// 推荐人评价
        /// </summary>
        public string EmpJudge { get; set; }
        /// <summary>
        /// 简历附件
        /// </summary>
        public string Attachment { get; set; }
        /// <summary>
        /// 简历状态
        /// </summary>
        public string ResumeStatus { get; set; }
        /// <summary>
        /// 简历状态 的显性字段MC
        /// </summary>
        [Computed]
        public string ResumeStatusMC { get; set; }
        /// <summary>
        /// 邮件UID
        /// </summary>
        public string MessageId { get; set; }
        /// <summary>
        /// 来自
        /// </summary>
        public string MessageFrom { get; set; }
        /// <summary>
        /// 信息日期
        /// </summary>
        public string MessageDate { get; set; }

    }
}
