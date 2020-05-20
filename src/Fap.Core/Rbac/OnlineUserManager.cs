using Fap.Core.Utility;
using Fap.Core.Rbac.Model;
using System.Collections.Generic;
using Dapper;
using System;
using Dapper.Contrib.Extensions;
using Fap.Core.DataAccess;
using Fap.Core.DI;

namespace Fap.Core.Rbac
{
    /// <summary>
    /// 在线用户管理类
    /// </summary>
    [Service(Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton)]
    public class OnlineUserService : IOnlineUserService
    {
        private readonly IDbContext _dbContext;
        public OnlineUserService(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// 添加一个用户到在线用户表日志
        /// </summary>
        /// <param name="session"></param>
        /// <param name="user"></param>
        [Transactional]
        public FapOnlineUser OnlineUser(FapOnlineUser onlineUser)
        {
            var ou= _dbContext.QueryFirstOrDefaultWhere<FapOnlineUser>($"{nameof(FapOnlineUser.UserUid)}=@UserUid and {nameof(FapOnlineUser.RoleUid)}=@RoleUid and {nameof(FapOnlineUser.OnlineState)}='{FapOnlineUser.CONST_ONLINE}' and {nameof(FapOnlineUser.ClientIP)}=@ClientIP",
                new DynamicParameters(new{ onlineUser.UserUid, onlineUser.RoleUid, onlineUser.ClientIP }));
            if (ou != null)
            {
                _dbContext.Execute($"update {nameof(FapOnlineUser)} set {nameof(FapOnlineUser.ConnectionId)}=@ConnectionId where {nameof(FapOnlineUser.UserUid)}=@UserUid and {nameof(FapOnlineUser.RoleUid)}=@RoleUid and {nameof(FapOnlineUser.OnlineState)}='{FapOnlineUser.CONST_ONLINE}' and {nameof(FapOnlineUser.ClientIP)}=@ClientIP",
                    new DynamicParameters(new { onlineUser.ConnectionId ,onlineUser.UserUid, onlineUser.RoleUid, onlineUser.ClientIP }));
            }
            else
            {
                _dbContext.Insert(onlineUser);
            }
            return onlineUser;
        }
        
        /// <summary>
        /// 一个在线用户登出
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public void OfflineUser(string connectionId)
        {
            _dbContext.Execute($"update {nameof(FapOnlineUser)} set {nameof(FapOnlineUser.OnlineState)}='{FapOnlineUser.CONST_OFFLINE}',{nameof(FapOnlineUser.LogoutTime)}='{DateTimeUtils.CurrentDateTimeStr}' where {nameof(FapOnlineUser.ConnectionId)}=@ConnectionId",
                    new DynamicParameters(new { ConnectionId= connectionId }));
        }
    }
}