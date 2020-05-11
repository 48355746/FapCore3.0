using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Utility
{
    public class FapStringEqualityComparer : EqualityComparer<string>
    {
        public override bool Equals(string x, string y)
        {
            return x.Equals(y, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode(string obj)
        {
            return obj.GetHashCode();
        }
    }

}
