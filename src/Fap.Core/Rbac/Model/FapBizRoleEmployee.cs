using Dapper.Contrib.Extensions;
using Fap.Core.MetaData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fap.Core.Rbac.Model
{
    /// <summary>
    /// 业务角色员工
    /// </summary>
    [Serializable]
    public class FapBizRoleEmployee : BaseModel
    {
        /// <summary>
        /// 员工
        /// </summary>
        public string EmpUid { get; set; }
        /// <summary>
        /// 员工 的显性字段MC
        /// </summary>
        [ComputedAttribute]
        public string EmpUidMC { get; set; }
        /// <summary>
        /// 业务角色
        /// </summary>
        public string BizRoleUid { get; set; }
        /// <summary>
        /// 业务角色 的显性字段MC
        /// </summary>
        [ComputedAttribute]
        public string BizRoleUidMC { get; set; }

    }

}
