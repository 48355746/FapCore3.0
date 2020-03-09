using Fap.Core.MultiLanguage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Fap.Core.Extensions
{
    public static class EnumExtensions
    {
        public static string Description(this Enum value, IMultiLangService multiLangService=null)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            DescriptionAttribute attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;

            string desc = attribute == null ? value.ToString() : attribute.Description;
            if (multiLangService != null)
            {
                desc = multiLangService.GetOrAndMultiLangValue(MultiLanguageOriginEnum.Enum, $"enum_{value.ToString()}", desc);
            }
            return desc;
        }

        public static T ParseEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static IEnumerable<EnumItem> EnumItems(this Type tEnum, IMultiLangService multiLangService = null)
        {
            if (tEnum.IsEnum)
            {
                return Enum.GetValues(tEnum).OfType<Enum>()
                    .Select(x => new EnumItem
                    {
                        Key = Convert.ToInt32(x),
                        Value = x.ToString(),
                        Description = x.Description(multiLangService)

                    });
            }
            else
            {
                return null;
            }
        }
    }
    public class EnumItem
    {
        public int Key { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
    }
}
