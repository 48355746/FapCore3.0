using Dapper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Fap.Core.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// 字符串不存在
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static bool IsMissing(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }
        /// <summary>
        /// 字符串不存在或太长
        /// </summary>
        /// <param name="value"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static bool IsMissingOrTooLong(this string value, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return true;
            }
            if (value.Length > maxLength)
            {
                return true;
            }

            return false;
        }
        /// <summary>
        /// 字符串存在
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static bool IsPresent(this string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }
        public static bool EqualsWithIgnoreCase(this string value, string target)
        {
            return value.Equals(target, StringComparison.OrdinalIgnoreCase);
        }
        /// <summary>
        /// 逗号拆分同时去掉内容的空格
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static List<string> SplitComma(this string value)
        {
            return value.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).AsList();
        }
        /// <summary>
        /// 分号拆分同时去掉内容的空格
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static List<string> SplitSemicolon(this string value)
        {
            return value.Split(new char[] { ';', ' ' }, StringSplitOptions.RemoveEmptyEntries).AsList();
        }
        /// <summary>
        /// 转化为整形
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static int ToInt(this string value)
        {
            if (value.IsMissing())
            {
                return 0;
            }
            return Convert.ToInt32(value);
        }
        public static bool ToBool(this string value)
        {
            return value.EqualsWithIgnoreCase("y") || value.EqualsWithIgnoreCase("yes")
                        || value.EqualsWithIgnoreCase("1") || value.EqualsWithIgnoreCase("true")
                        || value.EqualsWithIgnoreCase("t");
        }
        public static long ToLong(this string value)
        {
            if (value.IsMissing())
            {
                return 0;
            }
            return Convert.ToInt64(value);
        }
        public static decimal ToDecimal(this string value)
        {
            if (value.IsMissing())
            {
                return 0.0M;
            }
            return Convert.ToDecimal(value);
        }
        /// <summary>
        /// 去除所有空格
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RemoveSpace(this string str)
        {
            if (str.IsMissing()) return "";
            return str.Replace(" ", "").Replace("\r", "").Replace("\n", "");
        }
        /// <summary>
        /// 替换字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="oldString"></param>
        /// <param name="newString"></param>
        /// <param name="stringComparison"></param>
        /// <returns></returns>
        public static string ReplaceIgnoreCase(this string str, string oldString, string newString, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
        {
            return str.IsMissing() ? "" : str.Replace(oldString, newString, stringComparison);
        }
    }
}
