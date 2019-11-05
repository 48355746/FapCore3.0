using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLGeneration
{
    using System;
    using System.Text.RegularExpressions;

    namespace SQLGeneration
    {

        internal static class RegexOptionsHelper
        {
            private static readonly RegexOptions CompiledOption;

            static RegexOptionsHelper()
            {
                // Not all platforms support compiled regex's but those that do will be supported!
                if (!Enum.TryParse("Compiled", out CompiledOption))
                    CompiledOption = RegexOptions.None;
            }

            public static RegexOptions DefaultOptions
            {
                get
                {
                    return CompiledOption;
                }
            }
        }
    }

}
