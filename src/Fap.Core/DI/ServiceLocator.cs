using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace Fap.Core.DI
{
    /// <summary>
    /// 获取服务（暂时搁置）
    /// </summary>
    public static class ServiceLocator
    {
        private static IHttpContextAccessor HttpContextAccessor;
        public static void Setup(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor;
        }
        public static T GetService<T>()
        {
            if (HttpContextAccessor == null || HttpContextAccessor.HttpContext == null) return default(T);
            return HttpContextAccessor.HttpContext.RequestServices.GetService<T>();
        }
        public static IEnumerable<T> GetServices<T>()
        {
            if (HttpContextAccessor == null || HttpContextAccessor.HttpContext == null) return Enumerable.Empty<T>();
            return HttpContextAccessor.HttpContext.RequestServices.GetServices<T>();
        }
        public static object GetService(Type type)
        {
            if (HttpContextAccessor == null || HttpContextAccessor.HttpContext == null) return null;
            return HttpContextAccessor.HttpContext.RequestServices.GetService(type);
        }
        public static IEnumerable<object> GetServices(Type type)
        {
            if (HttpContextAccessor == null || HttpContextAccessor.HttpContext == null) return Enumerable.Empty<object>();
            return HttpContextAccessor.HttpContext.RequestServices.GetServices(type);
        }
       
    }
}
