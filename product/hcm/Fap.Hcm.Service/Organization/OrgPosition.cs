using Dapper.Contrib.Extensions;
using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Hcm.Service.Organization
{
    /// <summary>
    /// 职位
    /// </summary>
    public class OrgPosition : BaseModel
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string PstCode { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string PstName { get; set; }
        /// <summary>
        /// 职务
        /// </summary>
        public string JobUid { get; set; }
        /// <summary>
        /// 职务 的显性字段MC
        /// </summary>
        [Computed]
        public string JobUidMC { get; set; }
        /// <summary>
        /// 部门
        /// </summary>
        public string DeptUid { get; set; }
        /// <summary>
        /// 部门 的显性字段MC
        /// </summary>
        [Computed]
        public string DeptUidMC { get; set; }
        /// <summary>
        /// 编制人数
        /// </summary>
        public int Preparation { get; set; }
        /// <summary>
        /// 在岗人数
        /// </summary>
        public int Actual { get; set; }
        /// <summary>
        /// 职位描述
        /// </summary>
        public string PstNote { get; set; }
        /// <summary>
        /// 附件
        /// </summary>
        public string Attachment { get; set; }
        /// <summary>
        /// 说明书
        /// </summary>
        public string Instructions { get; set; }

    }
}
