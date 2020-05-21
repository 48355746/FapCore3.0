using Fap.Core.Rbac.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Fap.Core.SignalR
{
    public interface IOnlineUser
    {
        Task Online(FapOnlineUser onlineUser);
        Task Offline(FapOnlineUser onlineUser);
        Task ReceiveMessage(string user, string message);
        Task ReceiveMessage(string message);
    }
}
