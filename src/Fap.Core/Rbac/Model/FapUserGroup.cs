using Fap.Core.MetaData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fap.Core.Rbac.Model
{
    /// <summary>
    /// 对象模型：用户组
    /// </summary>
    [Serializable]
    public class FapUserGroup : BaseModel
    {
        public string UserGroupCode { get; set; }
        public string UserGroupName { get; set; }
        public int UserGroupType { get; set; }
        public string Pid { get; set; }
    }
}
