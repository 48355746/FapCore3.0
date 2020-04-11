using Microsoft.Extensions.DependencyInjection;
using Fap.Core.Message.Mail.Infrastructure;
using Fap.Core.Message.Mail.Infrastructure.Internal;
using Fap.Core.Message.Mail.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Message.Mail.Extensions
{
    public static class MailKitOptionsBuilderExtension
    {
        public static IMailKitOptionsBuilder UseMailKit(IMailKitOptionsBuilder builder, MailKitOptions options, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            Check.Argument.IsNotNull(builder, nameof(builder), "The MailKitOptionsBuilder is null");
            Check.Argument.IsNotNull(options, nameof(options), "The MailKitOptions is null");

            return builder.UseMailKit(options, lifetime);
        }
    }
}
