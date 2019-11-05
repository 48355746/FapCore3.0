using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Utility
{
    public class DateTimeUtils
    {
        public const string DATETIMEFORMAT = "yyyy-MM-dd HH:mm:ss";
        public const string DATEFORMAT = "yyyy-MM-dd";
        /// <summary>
        /// 获取永恒时间
        /// </summary>
        public static string PermanentTimeStr = "9999-12-31 23:59:59";
        /// <summary>
        /// 获取当前时间（字符串），精确到一秒钟
        /// </summary>
        public static string CurrentDateTimeStr
        {
            get
            {
                return string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now);
            }
        }
        public static string CurrentDateStr
        {
            get
            {
                return string.Format("{0:yyyy-MM-dd}", DateTime.Now);
            }
        }
        /// <summary>
        /// 获取时间的字符串
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string GetDateTimeStr(DateTime dt)
        {
            return string.Format("{0:yyyy-MM-dd HH:mm:ss}", dt);
        }
        public static string GetDateStr(DateTime dt)
        {
            return string.Format("{0:yyyy-MM-dd}", dt);
        }
        /// <summary>
        /// 获取当前时间戳
        /// </summary>
        /// <returns></returns>
        public static long Ts
        {
            get
            {
                return DateTime.Now.Ticks;
            }
        }

    }
}
