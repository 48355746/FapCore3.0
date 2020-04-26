using Dapper.Contrib.Extensions;
using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Hcm.Service.Recruit
{
    /// <summary>
    /// 简历邮件读取记录
    /// </summary>
    public class RcrtMailReadRecord : BaseModel
    {
        /// <summary>
        /// 员工
        /// </summary>
        public string EmpUid { get; set; }
        /// <summary>
        /// 员工 的显性字段MC
        /// </summary>
        [Computed]
        public string EmpUidMC { get; set; }
        /// <summary>
        /// 邮件UID
        /// </summary>
        public string MessageUid { get; set; }

    }
}
