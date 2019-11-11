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
        /// 转化为整形
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static int ToInt(this string value)
        {
            return Convert.ToInt32(value);
        }
    }
}
