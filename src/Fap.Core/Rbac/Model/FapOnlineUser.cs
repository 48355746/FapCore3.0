using Dapper.Contrib.Extensions;
using Fap.Core.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Rbac.Model
{
    /// <summary>
    /// 在线用户
    /// </summary>
    [Serializable]
    public class FapOnlineUser : BaseModel
    {
        public const string CONST_LOGON = "登录";
        public const string CONST_LOGOUT = "登出";
        public const string CONST_LOGEXCEPT = "异常";
        /// <summary>
        /// 用户
        /// </summary>
        public string UserUid { get; set; }
        /// <summary>
        /// 用户 的显性字段MC
        /// </summary>
        [Computed]
        public string UserUidMC { get; set; }
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
        /// 用户登陆名
        /// </summary>
        public string LoginName { get; set; }
        /// <summary>
        /// 所在部门
        /// </summary>
        public string DeptUid { get; set; }
        /// <summary>
        /// 所在部门 的显性字段MC
        /// </summary>
        [Computed]
        public string DeptUidMC { get; set; }
        /// <summary>
        /// IP地址
        /// </summary>
        public string ClientIP { get; set; }
        /// <summary>
        /// 登陆时间
        /// </summary>
        public string LoginTime { get; set; }
        /// <summary>
        /// 登出时间
        /// </summary>
        public string LogoutTime { get; set; }
        /// <summary>
        /// 在线状态
        /// </summary>
        public string OnlineState { get; set; }
        /// <summary>
        /// 当前角色
        /// </summary>
        public string RoleUid { get; set; }
        /// <summary>
        /// 当前角色 的显性字段MC
        /// </summary>
        [Computed]
        public string RoleUidMC { get; set; }


    }
}
