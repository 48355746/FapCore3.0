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
        public static string GetDescription(this Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            DescriptionAttribute attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;

            return attribute == null ? value.ToString() : attribute.Description;
        }



        public static IEnumerable<EnumItem> GetDictionary(Type tEnum)
        {
            return Enum.GetValues(tEnum).OfType<Enum>()
                .Select(x => new EnumItem
                {
                    Key = Convert.ToInt32(x),
                    Value = x.ToString(),
                    Description = x.GetDescription()
                });
        }
    }
    public class EnumItem
    {
        public int Key { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
    }
}
