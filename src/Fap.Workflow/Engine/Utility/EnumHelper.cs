using System;

namespace Fap.Workflow.Engine.Utility
{
    /// <summary>
    /// 根据字符串转换枚举类型
    /// </summary>
    public class EnumHelper
    {
        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
    }
}
