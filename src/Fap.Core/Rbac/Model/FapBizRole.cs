using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Rbac.Model
{
    /// <summary>
    /// 业务角色
    /// </summary>
    public class FapBizRole :BaseModel
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string BizRoleCode { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string BizRoleName { get; set; }
        /// <summary>
        /// 上级角色
        /// </summary>
        public string Pid { get; set; }

    }

}
