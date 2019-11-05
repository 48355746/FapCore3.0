using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Fap.Core.Infrastructure.License
{
    /// <summary>
    /// Fap注册信息
    /// </summary>
    public class RegisterInfo
    {
        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProjectName { get; set; }
        /// <summary>
        /// License的注册状态
        /// </summary>
        public EnumRegisterState RegisterState { get; set; }
        /// <summary>
        /// 授权模块
        /// </summary>
        public List<string> AuthoredModules { get; set; }
        /// <summary>
        /// 试用版时的剩余天数
        /// </summary>
        public int ExpireWhenTrial { get; set; }
        /// <summary>
        /// 试用版时的到期时间
        /// </summary>
        public string ExpireDateTimeWhenTrial { get; set; }
        /// <summary>
        /// 信息
        /// </summary>
        public string RegisterMessage { get; set; }
    }

    /// <summary>
    /// 注册状态
    /// </summary>
    public enum EnumRegisterState
    {
        /// <summary>
        /// 注册失败
        /// </summary>
        [Description("注册失败")]
        FailureRegister,
        /// <summary>
        /// 未注册
        /// </summary>
        [Description("未注册")]
        UnRegister,
        /// <summary>
        /// 试用版
        /// </summary>
        [Description("试用版")]
        Trial,
        /// <summary>
        /// 正式版
        /// </summary>
        [Description("正式版")]
        Release,
        /// <summary>
        /// 开发版
        /// </summary>
        [Description("开发版")]
        Develop
    }
}
