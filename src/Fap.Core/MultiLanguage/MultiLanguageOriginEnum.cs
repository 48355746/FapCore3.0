using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.MultiLanguage
{
    /// <summary>
    /// 多语来源
    /// </summary>
    public enum MultiLanguageOriginEnum
    {
        /// <summary>
        /// 按钮标签
        /// </summary>
        ButtonTag,
        /// <summary>
        /// key:tableName+"_"+colName
        /// </summary>
        FapColumn,
        /// <summary>
        /// key:tableName
        /// </summary>
        FapTable,
        /// <summary>
        /// js
        /// </summary>
        Javascript,
        /// <summary>
        /// key:menucode
        /// </summary>
        Menu,
        /// <summary>
        /// 模块
        /// </summary>
        Module,
        /// <summary>
        /// 多语标签
        /// </summary>
        MultiLangTag,
        /// <summary>
        /// 页面
        /// </summary>
        Page,
        /// <summary>
        /// 枚举描述
        /// </summary>
        Enum
    }
}
