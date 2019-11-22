using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Fap.Core.Extensions
{
    public static class ObjectExtensions
    {

        //public static string ToStringOrEmpty(this Newtonsoft.Json.Linq.JToken token)
        //{
        //    if (token == null)
        //    {
        //        return string.Empty;
        //    }
        //    return token.ToString();
        //}
        public static int ToInt(this object obj, int defaultValue = 0)
        {
            if (obj == null)
            {
                return defaultValue;
            }
            else
            {
                if (obj is int || obj is double || obj is float || obj is long)
                {
                    return (int)obj;
                }

                int i = defaultValue;
                if (int.TryParse(obj.ToString(), out i))
                {
                    return i;
                }
                else
                {
                    return defaultValue;
                }
            }
        }

        public static long ToLong(this object obj, long defaultValue = 0L)
        {
            if (obj == null)
            {
                return defaultValue;
            }
            else
            {
                if (obj is int || obj is double || obj is float || obj is long)
                {
                    return (long)obj;
                }

                long i = defaultValue;
                if (long.TryParse(obj.ToString(), out i))
                {
                    return i;
                }
                else
                {
                    return defaultValue;
                }
            }
        }

        public static double ToDouble(this object obj, double defaultValue = 0.0D)
        {
            if (obj == null)
            {
                return defaultValue;
            }
            else
            {
                if (obj is double)
                {
                    return (double)obj;
                }
                double i = defaultValue;
                if (double.TryParse(obj.ToString(), out i))
                {
                    return i;
                }
                else
                {
                    return defaultValue;
                }
            }
        }

        public static string ToStringOrEmpty(this object obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }
            return obj.ToString();
        }

        /// <summary>
        /// 转成日期
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static DateTime? ToDate(this object obj)
        {
            if (obj == null)
            {
                return null;
            }
            if (obj is DateTime)
            {
                return (DateTime)obj;
            }

            DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();
            dtFormat.ShortDatePattern = "yyyy-MM-dd";
            DateTime dt = Convert.ToDateTime(obj.ToString(), dtFormat);
            return dt;
        }

        /// <summary>
        /// 转成时间
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static DateTime? ToTime(this object obj)
        {
            if (obj == null)
            {
                return null;
            }
            if (obj is DateTime)
            {
                return (DateTime)obj;
            }

            DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();
            dtFormat.ShortDatePattern = "HH:mm:ss";
            DateTime dt = Convert.ToDateTime(obj.ToString(), dtFormat);
            return dt;
        }

        /// <summary>
        /// 转成日期时间
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static DateTime? ToDateTime(this object obj)
        {
            if (obj == null)
            {
                return null;
            }
            if (obj is DateTime)
            {
                return (DateTime)obj;
            }

            DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();
            dtFormat.ShortDatePattern = "yyyy-MM-dd HH:mm:ss";
            DateTime dt = Convert.ToDateTime(obj.ToString(), dtFormat);
            return dt;
        }
        /// <summary>
        /// y,1,yes,t,true 都会转化为true
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool ToBool(this object obj)
        {
            if (obj == null)
            {
                return false;
            }
            else
            {
                if (obj is bool)
                {
                    return (bool)obj;
                }

                string str = obj.ToString();
                return str.EqualsWithIgnoreCase("y") || str.EqualsWithIgnoreCase("yes")
                        || str.EqualsWithIgnoreCase("1") || str.EqualsWithIgnoreCase("true")
                        || str.EqualsWithIgnoreCase("t");
            }
        }

    }
}
