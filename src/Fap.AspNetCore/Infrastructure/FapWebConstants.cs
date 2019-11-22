using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.AspNetCore.Infrastructure
{
    /// <summary>
    /// 存储web常量
    /// </summary>
    public class FapWebConstants
    {
        /// <summary>
        /// 数据持久化方式
        /// </summary>
        public const string OPERATOR = "oper";
        /// <summary>
        /// form 提交的 表名参数
        /// </summary>
        public const string FORM_TABLENAME = "Table_Name";
        /// <summary>
        /// query param传来的表名参数
        /// </summary>
        public const string QUERY_TABLENAME = "tn";
        /// <summary>
        /// 防重复提交token
        /// </summary>
        public const string AVOID_REPEAT_TOKEN = "avoid_repeat_token";
        /// <summary>
        /// 未定义
        /// </summary>
        public const string UNDEFINED = "undefined";
        /// <summary>
        /// 逻辑删除标识
        /// </summary>
        public const string LOGICDELETE = "logicdelete";
        /// <summary>
        /// 子表数据
        /// </summary>
        public const string CHILDS_DATALIST = "childsData";
    }
}
