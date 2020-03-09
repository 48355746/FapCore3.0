using Microsoft.Extensions.DependencyInjection;
using Fap.Core.Message.Mail.Infrastructure;
using Fap.Core.Message.Mail.Infrastructure.Internal;
using Fap.Core.Message.Mail.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Message.Mail.Extensions
{
    public static class ServicesCollectionExtensions
    {
        public static IServiceCollection AddMailKit(this IServiceCollection serviceCollection, Action<MailKitOptionsBuilder> optionsAction)
        {
            Check.Argument.IsNotNull(serviceCollection, nameof(serviceCollection), "IServiceCollection is not dependency injection");
            Check.Argument.IsNotNull(optionsAction, nameof(optionsAction));

            optionsAction.Invoke(new MailKitOptionsBuilder(serviceCollection));
            return serviceCollection;
        }
    }
}
