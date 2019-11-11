using Dapper.Contrib.Extensions;
using Fap.Core.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fap.Core.Rbac.Model
{
    /// <summary>
    /// 对象模型：用户
    /// </summary>
    [Serializable]
    public class FapUser : BaseModel
    {
        /// <summary>
        /// 用户编码
        /// </summary>
        public string UserCode { get; set; }
        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string UserEmail { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
        public string UserPhone { get; set; }
        /// <summary>
        /// 用户密码
        /// </summary>
        public string UserPassword { get; set; }
        /// <summary>
        /// 所属用户组
        /// </summary>
        public string UserGroupUid { get; set; }
        /// <summary>
        /// 所属用户组 的显性字段MC
        /// </summary>
        [ComputedAttribute]
        public string UserGroupUidMC { get; set; }
        /// <summary>
        /// 用户类型【开发用户，业务用户】
        /// </summary>
        public string UserType { get; set; }
        /// <summary>
        /// 用户类型 的显性字段MC
        /// </summary>
        [ComputedAttribute]
        public string UserTypeMC { get; set; }
        /// <summary>
        /// 内容语种
        /// </summary>
        public string ContentLang { get; set; }
        /// <summary>
        /// 启用状态
        /// </summary>
        public int EnableState { get; set; }
        /// <summary>
        /// 尝试次数
        /// </summary>
        public int PasswordTryTimes { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string UserNote { get; set; }
        /// <summary>
        /// 密码安全级别编码
        /// </summary>
        public string PwdLevelCode { get; set; }
        /// <summary>
        /// 锁定
        /// </summary>
        public int IsLocked { get; set; }
        /// <summary>
        /// 最后登录时间
        /// </summary>
        public string LastLoginTime { get; set; }
        /// <summary>
        /// 身份
        /// </summary>
        public string UserIdentity { get; set; }
        /// <summary>
        /// 身份MC
        /// </summary>
        [ComputedAttribute]
        public string UserIdentityMC { get; set; }
        /// <summary>
        /// 主题
        /// </summary>
        public string Theme { get; set; }

        
    }
}
