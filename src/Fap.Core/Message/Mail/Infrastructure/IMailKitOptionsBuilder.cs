using Microsoft.Extensions.DependencyInjection;
using Fap.Core.Message.Mail.Infrastructure.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Message.Mail.Infrastructure
{
    public interface IMailKitOptionsBuilder
    {
        /// <summary>
        /// service collection
        /// </summary>
        IServiceCollection serviceCollection { get; }

        /// <summary>
        /// get redis options and add ConnectionMultiplexer to sercice collection
        /// </summary>
        /// <param name="options">redis options</param>
        /// <param name="lifetime"></param>
        /// <returns></returns>
        IMailKitOptionsBuilder UseMailKit(MailKitOptions options, ServiceLifetime lifetime = ServiceLifetime.Scoped);
    }
}
