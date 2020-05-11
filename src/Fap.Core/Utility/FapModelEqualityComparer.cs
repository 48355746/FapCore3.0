using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Utility
{
    public class FapModelEqualityComparer<T> : IEqualityComparer<T> where T : BaseModel
    {
        public bool Equals(T x, T y)
        {
            if (x == null)
                return y == null;
            return x.Fid == y.Fid;
        }

        public int GetHashCode(T obj)
        {
            if (obj == null)
                return 0;
            return obj.Fid.GetHashCode();
        }
    }
}
