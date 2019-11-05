using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(Fap.Core.DI.ServicesInjection))]
namespace Fap.Core.DI
{
    public class ServicesInjection : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var basePath = AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory;
                var assemblies = Directory.GetFiles(basePath, "*.dll").Select(Assembly.LoadFrom).ToArray();
                var types = assemblies.SelectMany(a => a.DefinedTypes).Select(type => type.AsType()).Where(t => t.GetCustomAttribute<ServiceAttribute>() != null).ToArray();
                var implementTypes = types.Where(t => t.IsClass).ToArray();
                foreach (var implementType in implementTypes)
                {
                    var attr = implementType.GetCustomAttribute<ServiceAttribute>();
                    if (attr.InterfaceType != null)
                    {
                        RegisterService(services, attr.ServiceLifetime, attr.InterfaceType, implementType);
                    }
                    else
                    {
                        var interfaceTypes = implementType.GetInterfaces();
                        foreach (var interfaceType in interfaceTypes)
                        {
                            RegisterService(services, attr.ServiceLifetime, interfaceType, implementType);
                        }
                    }
                }
            });
            void RegisterService(IServiceCollection services, ServiceLifetime serviceLifetime, Type interfaceType, Type implementType)
            {
                _ = (serviceLifetime switch
                {
                    ServiceLifetime.Scoped => services.AddScoped(interfaceType, implementType),
                    ServiceLifetime.Singleton => services.AddSingleton(interfaceType, implementType),
                    ServiceLifetime.Transient => services.AddTransient(interfaceType, implementType),
                    _ => throw new NotSupportedException("不支持此ServiceLifetime")
                });
            }
        }
    }
}
