using Dapper.Contrib.Extensions;
using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Hcm.Service.Organization
{
    /// <summary>
    /// 职位说明书
    /// </summary>
    public class OrgPositionManual : BaseModel
    {
        /// <summary>
        /// 职位
        /// </summary>
        public string PositionUid { get; set; }
        /// <summary>
        /// 职位 的显性字段MC
        /// </summary>
        [Computed]
        public string PositionUidMC { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string ManualContent { get; set; }
        /// <summary>
        /// 附件
        /// </summary>
        public string Attachment { get; set; }

    }
}
