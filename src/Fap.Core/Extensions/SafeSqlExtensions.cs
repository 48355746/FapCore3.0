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
        /// <summary>
        /// 是否安全字符串SQL，例如包含"slect insert"等注入关键字
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsSafeSQL(this string s)
        {
            bool ReturnValue = true;
            try
            {
                if (s.Trim() != "")
                {
                    string SqlStr = "exec|insert+|select+|delete|update|count|chr|mid|master+|truncate|char|declare|drop+|drop+table|creat+|create|*|iframe|script|";
                    SqlStr += "exec+|insert|delete+|update+|count(|count+|chr+|+mid(|+mid+|+master+|truncate+|char+|+char(|declare+|drop+table|creat+table";
                    string[] anySqlStr = SqlStr.Split('|');
                    foreach (string ss in anySqlStr)
                    {
                        if (s.ToLower().IndexOf(ss) >= 0)
                        {
                            ReturnValue = false;
                            break;
                        }
                    }
                }
            }
            catch
            {
                ReturnValue = false;
            }
            return ReturnValue;
        }
    }
}
