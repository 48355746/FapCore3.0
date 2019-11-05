using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Infrastructure.Cache
{
    public interface ICacheFactory
    {
        ICacheService GetCacheService(CacheEnum cacheEnum);
    }
    public enum CacheEnum
    {
        Memory,
        Distributed
    }
}
