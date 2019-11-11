using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Fap.Core.Infrastructure.Enums
{
    /// <summary>
    /// 多语言
    /// </summary>   
    public enum MultiLanguageEnum
    {
        /// <summary>
        /// 简体中文
        /// </summary>
        [Description("简体中文")]
        ZhCn,
        /// <summary>
        /// 繁体中文
        /// </summary>
        [Description("繁體中文")]
        ZhTW,
        /// <summary>
        /// 英语
        /// </summary>
        [Description("English")]
        En,
        /// <summary>
        /// 日语
        /// </summary>
        [Description("日本語")]
        Ja
    }
}
