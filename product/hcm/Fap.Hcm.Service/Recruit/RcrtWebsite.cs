using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Hcm.Service.Recruit
{
    /// <summary>
    /// 招聘网站
    /// </summary>
    public class RcrtWebsite : BaseModel
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string WebName { get; set; }
        /// <summary>
        /// 链接地址
        /// </summary>
        public string WebUrl { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string WebDesc { get; set; }
        /// <summary>
        /// 网站帐号
        /// </summary>
        public string WebAccount { get; set; }
        /// <summary>
        /// 网站密码
        /// </summary>
        public string WebPassword { get; set; }
        /// <summary>
        /// 邮件解析插件
        /// </summary>
        public string EmailAnalysisPlugin { get; set; }

    }
}
