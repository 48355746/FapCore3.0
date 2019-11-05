using Fap.Core.DI;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

namespace Fap.Core.Infrastructure.Cache
{
    [Service(Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton)]
    public class CacheFactory : ICacheFactory
    {
        private readonly IMemoryCache memoryCache;
        private readonly IDistributedCache distributedCache;
        public CacheFactory(IServiceProvider service)
        {
            memoryCache = service.GetService<IMemoryCache>();
            distributedCache = service.GetService<IDistributedCache>();
        }
        public ICacheService GetCacheService(CacheEnum cacheEnum) => cacheEnum switch
        {
            CacheEnum.Memory => new MemoryCacheService(memoryCache),
            CacheEnum.Distributed => new DistributedCacheService(distributedCache),
            _ => throw new NotImplementedException()
        };
    }
}
