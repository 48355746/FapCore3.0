using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Extensions
{
    public static class SafeSqlExtensions
    {
        /// <summary>
        /// 过滤sql
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ReplaceSql(this string str)
        {
            str = str.Replace("'", "").Replace("--", " ").Replace(";", "");
            return str;
        }
        /// <summary>
        /// 过滤查询sql
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string FilterDangerSql(this string str)
        {
            if (str.IsMissing()) return "";
            str = str.ReplaceIgnoreCase("DELETE ", "").ReplaceIgnoreCase("UPDATE ", "").ReplaceIgnoreCase("INSERT ", "").ReplaceIgnoreCase("DROP ", "").ReplaceIgnoreCase("ALTER ", "");
            return str;
        }
    }
}
