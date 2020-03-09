using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Fap.Core.DI
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ServiceAttribute:Attribute
    {
        public ServiceAttribute(ServiceLifetime serviceLifetime=ServiceLifetime.Singleton)
        {
            ServiceLifetime = serviceLifetime;
        }
        public Type InterfaceType { get; set; }
        public ServiceLifetime ServiceLifetime { get; set; }
    }
}
