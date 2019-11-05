using Fap.Core.DI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core
{
    [Service(Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton)]
    public class User : IUser,IUser1
    {
        private string s = Guid.NewGuid().ToString();
        public string Get(string userName)
        {
            return $"{s}===={userName}";
        }

        public string Get1(string userName)
        {
            return s;
        }
    }
}
