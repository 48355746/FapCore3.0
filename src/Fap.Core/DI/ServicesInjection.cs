using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Castle.DynamicProxy;
using Fap.Core.DataAccess.Interceptor;
using Fap.Core.Infrastructure.Domain;

namespace Fap.Core.DI
{
    public static class ServicesInjectionExtents
    {
        public static IFapBuilder AddAutoInjection(this IFapBuilder builder)
        {
            var services = builder.Services;
            var basePath = AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory;
            var assemblies = Directory.GetFiles(basePath, "*.dll").Select(Assembly.LoadFrom).ToArray();
            var types = assemblies.SelectMany(a => a.DefinedTypes).Select(type => type.AsType()).Where(t => t.GetCustomAttribute<ServiceAttribute>() != null).ToArray();
            var implementTypes = types.Where(t => t.IsClass).ToArray();
            IInterceptor interceptor = services.BuildServiceProvider().GetService<IInterceptor>();
            foreach (var implementType in implementTypes)
            {
                var attr = implementType.GetCustomAttribute<ServiceAttribute>();
                if (attr.InterfaceType != null)
                {
                    RegisterService(services, attr.ServiceLifetime, attr.InterfaceType, implementType, interceptor);
                }
                else
                {
                    var interfaceTypes = implementType.GetInterfaces();
                    foreach (var interfaceType in interfaceTypes)
                    {
                        RegisterService(services, attr.ServiceLifetime, interfaceType, implementType, interceptor);
                    }
                }
            }
            void RegisterService(IServiceCollection services, ServiceLifetime serviceLifetime, Type interfaceType, Type implementType, params IInterceptor[] interceptor)
            {
                var proxyGenerator = services.BuildServiceProvider().GetService<ProxyGenerator>();
                services.Add(new ServiceDescriptor(implementType, implementType, serviceLifetime));
                var serviceDescriptor = new ServiceDescriptor(interfaceType,(provider) => {
                    var target = provider.GetService(implementType);
                    var proxyGenerator = provider.GetService<ProxyGenerator>();
                    var proxy = proxyGenerator.CreateInterfaceProxyWithTarget(interfaceType, target, interceptor);
                    return proxy;
                },serviceLifetime);
                services.Add(serviceDescriptor);           
            }
            return builder;
        }

        //private static object InstanceProxy(Type interfaceType,Type implementType, IInterceptor[] interceptor, IServiceProvider services)
        //{
        //    var proxyGenerator = services.GetService<ProxyGenerator>();
        //    ConstructorInfo[] constructorInfos = implementType.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
        //    var paramArray = constructorInfos.First().GetParameters();
        //    object[] constructorArguments = new object[paramArray.Length];
        //    for (int i = 0; i < constructorArguments.Length; i++)
        //    {
        //        constructorArguments[i] = services.GetService(paramArray[i].ParameterType);
        //    }
        //    return proxyGenerator.CreateClassProxy(interfaceType, constructorArguments, interceptor);
        //}
    }
}
