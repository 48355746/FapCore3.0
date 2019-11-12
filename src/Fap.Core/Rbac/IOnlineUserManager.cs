using System.Collections.Generic;
using Fap.Core.Rbac.Model;

namespace Fap.Core.Rbac
{
    public interface IOnlineUserManager
    {
        FapOnlineUser AddOnlineUser(FapOnlineUser onlineUser);
        IEnumerable<FapOnlineUser> GetAllOnlineUser();
        void LogoutOnlineUser(string onlineUserUid);
        bool UpdateOnlineUser(string onlineUserUid, string roleUid);
    }
}