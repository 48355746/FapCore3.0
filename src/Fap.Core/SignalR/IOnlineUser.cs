using Fap.Core.Rbac.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Fap.Core.SignalR
{
    public interface IOnlineUser
    {
        Task Online(object onlineUser);
        Task Offline(object onlineUser);
        Task ReceiveMessage(string userName,string userId, string message);
        Task ReceiveMessage(string message);
    }
}
