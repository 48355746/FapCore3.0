using Fap.Core.Infrastructure.Domain;
using Fap.Core.Rbac;
using Fap.Core.Rbac.Model;
using Fap.Core.Utility;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Fap.Core.SignalR
{
    public class OnlineUserHub : Hub<IOnlineUser>
    {
        private readonly IOnlineUserService _onlineUserService;
        private readonly IFapApplicationContext _applicationContext;
        public OnlineUserHub(IOnlineUserService onlineUserService, IFapApplicationContext applicationContext)
        {
            _onlineUserService = onlineUserService;
            _applicationContext = applicationContext;
        }
        public async Task SendMessage(string user, string message)
        {
            await Clients.User(user).ReceiveMessage(_applicationContext.EmpName, message);
        }
        public override Task OnConnectedAsync()
        {
            //添加在线用户日志               
            RegistryOnlineUser();
            Clients.Others.Online(new FapOnlineUser
            {
                ConnectionId = Context.ConnectionId,
                EmpUid=_applicationContext.EmpUid,
                EmpUidMC = _applicationContext.EmpName,
                OnlineState=FapOnlineUser.CONST_ONLINE
            });
            return base.OnConnectedAsync();
        }
        private void RegistryOnlineUser()
        {
            FapOnlineUser onlineUser = new FapOnlineUser()
            {
                UserUid = _applicationContext.UserUid,
                ClientIP = _applicationContext.ClientIpAddress,
                DeptUid = _applicationContext.DeptUid,
                EmpUid = _applicationContext.EmpUid,
                RoleUid = _applicationContext.CurrentRoleUid,
                ConnectionId = Context.ConnectionId,
                LoginName = _applicationContext.UserName,
                LoginTime = DateTimeUtils.CurrentDateTimeStr,
                OnlineState = FapOnlineUser.CONST_ONLINE
            };
            _onlineUserService.OnlineUser(onlineUser);
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            Clients.Others.Offline(new FapOnlineUser
            {
                ConnectionId = Context.ConnectionId,
                EmpUid = _applicationContext.EmpUid,
                EmpUidMC = _applicationContext.EmpName,
                OnlineState = FapOnlineUser.CONST_OFFLINE
            });
            _onlineUserService.OfflineUser(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }
    }
}
