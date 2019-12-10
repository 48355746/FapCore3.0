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
        public FapOnlineUser AddOnlineUser(FapOnlineUser onlineUser)
        {
            //开启了事务，所以原始操作要有事务
            //更新之前的在线用户状态
            //_dbContext.Execute($"update FapOnlineUser set OnlineState='{FapOnlineUser.CONST_LOGOUT}' where UserUid=@UserUid", new DynamicParameters(new { UserUid = onlineUser.UserUid }) { });
            //insert 新在线用户
            _dbContext.Insert<FapOnlineUser>(onlineUser);           
            return onlineUser;
        }
        public bool UpdateOnlineUser(string onlineUserUid, string roleUid)
        {
            FapOnlineUser onlineUser = _dbContext.Get<FapOnlineUser>(onlineUserUid);
            if (onlineUser != null)
            {
                onlineUser.OnlineState = FapOnlineUser.CONST_LOGON;
                onlineUser.RoleUid = roleUid;
                _dbContext.Update<FapOnlineUser>(onlineUser);
            }
            return true;
        }
        /// <summary>
        /// 一个在线用户登出
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public void LogoutOnlineUser(string onlineUserUid)
        {
            FapOnlineUser onlineUser = _dbContext.Get<FapOnlineUser>(onlineUserUid);
            if (onlineUser != null)
            {
                onlineUser.OnlineState = FapOnlineUser.CONST_LOGOUT;
                onlineUser.LogoutTime = DateTimeUtils.CurrentDateTimeStr;
                _dbContext.Update<FapOnlineUser>(onlineUser);
            }
        }

        /// <summary>
        /// 得到所有在线用户表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<FapOnlineUser> GetAllOnlineUser()
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("OnlineState", FapOnlineUser.CONST_LOGON);
            //30分钟内算登陆
            param.Add("LoginTime", DateTime.Now.AddMinutes(-30).ToString("yyyy-MM-dd HH:mm:ss"));
            return _dbContext.Query<FapOnlineUser>("select * from FapOnlineUser where OnlineState =@OnlineState and LoginTime>=@LoginTime", param);

        }

    }
}