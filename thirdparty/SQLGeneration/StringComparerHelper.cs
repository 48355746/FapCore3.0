using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SQLGeneration
{
    internal static class StringComparerHelper
    {
        private static readonly PropertyInfo ComparerPropertyInfo;

        static StringComparerHelper()
        {
            //ComparerPropertyInfo = typeof(StringComparer).GetProperty("InvariantCultureIgnoreCase", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
        }

        public static StringComparer DefaultStringComparer
        {
            get
            {
                if (ComparerPropertyInfo == null)
                {
                    // InvariantCultureIgnoreCase not available on current platform.
                    return StringComparer.OrdinalIgnoreCase;
                }
                else
                {
                    var comparer = ComparerPropertyInfo.GetValue(null, null);
                    return (StringComparer)comparer;
                }
            }
        }
    }
}
