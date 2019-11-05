using Fap.Core.MetaData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fap.Core.Rbac.Model
{
    /// <summary>
    /// 角色组
    /// </summary>
    [Serializable]
    public class FapRoleGroup : BaseModel
    {
        /// <summary>
        /// 角色组名称
        /// </summary>
        public string RoleGroupName { get; set; }
        /// <summary>
        /// 角色组编码
        /// </summary>
        public string RoleGroupCode { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string RoleGroupType { get; set; }
        /// <summary>
        /// 上级角色组
        /// </summary>
        public string Pid { get; set; }

    }

}
