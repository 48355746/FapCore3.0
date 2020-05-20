using System.Collections.Generic;
using Fap.Core.Rbac.Model;

namespace Fap.Core.Rbac
{
    public interface IOnlineUserService
    {
        FapOnlineUser OnlineUser(FapOnlineUser onlineUser);
        void OfflineUser(string connectionId);
    }
}