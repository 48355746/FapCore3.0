using Dapper;
using Fap.Core.DataAccess;
using Fap.Core.DI;
using Fap.Core.Extensions;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.Rbac.Model;
using Microsoft.Extensions.DependencyInjection;
using Org.BouncyCastle.Bcpg;
using System.Collections.Generic;
using System.Linq;

namespace Fap.Core.Rbac
{
    [Service(ServiceLifetime.Singleton)]
    public class LoginService : ILoginService
    {
        private readonly IDbContext _dbContext;
        private readonly IRbacService _rbacService;
        public LoginService(IDbContext dbContext, IRbacService rbacService)
        {
            _dbContext = dbContext;
            _rbacService = rbacService;
        }
        //[Transactional]
        public FapUser Login(string userName)
        {
            string where = "UserName=@UserName";
            DynamicParameters param = new DynamicParameters();
            param.Add("UserName", userName);
            var user = _dbContext.QueryFirstOrDefaultWhere<FapUser>(where, param);
            if (user != null)
            {
                if (user.UserRole.IsMissing())
                {
                    user.UserRole = FapPlatformConstants.CommonUserRoleFid;
                }
                else
                {
                    //已经去除的角色还是默认角色的话 就会被替换为普通用户
                    var role = GetUserRoles(user.Fid).FirstOrDefault(r => r.Fid == user.UserRole);
                    if (role == null)
                    {
                        user.UserRole = FapPlatformConstants.CommonUserRoleFid;
                    }
                }
            }
            return user;
        }
        /// <summary>
        /// 添加尝试次数
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public FapUser AddTryTimes(FapUser user)
        {
            user.PasswordTryTimes += 1;
            //大于5次就冻结
            if (user.PasswordTryTimes > 5)
            {
                user.IsLocked = 1;
            }
            string sql = "update FapUser set passwordtrytimes=@trytimes,islocked=@islocked where id=@id";
            _dbContext.Execute(sql, new DynamicParameters(new { trytimes = user.PasswordTryTimes, islocked = user.IsLocked, id = user.Id }));
            return user;
        }
        public void UpdateUserDefaultRole(string roleUid, string userUid)
        {
            _dbContext.Execute($"update FapUser set {nameof(FapUser.UserRole)}=@RoleUid where Fid=@Fid", new DynamicParameters(new { RoleUid = roleUid, Fid = userUid }));
        }
        /// <summary>
        /// 更新最后登录时间
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public FapUser UpdateLastLoginTime(FapUser user)
        {
            string sql = "update FapUser set LastLoginTime=@lastTime, passwordtrytimes=@tryTimes where id=@id";
            _dbContext.Execute(sql, new DynamicParameters(new { lastTime = user.LastLoginTime, tryTimes = user.PasswordTryTimes, id = user.Id }));
            return user;
        }
        public void Logout()
        {

        }
        /// <summary>
        /// 获取用户默认角色
        /// </summary>
        /// <param name="userUid"></param>
        /// <returns></returns>
        public string GetUserDefaultRole(string userUid)
        {
            string roleUid = _dbContext.ExecuteScalar<string>($"select {nameof(FapUser.UserRole)} from {nameof(FapUser)} where Fid=@Fid", new DynamicParameters(new { Fid = userUid }));
            if (roleUid.IsMissing())
            {
                roleUid = FapPlatformConstants.CommonUserRoleFid;
            }
            return roleUid;
        }
        /// <summary>
        /// 获取用户的角色列表
        /// </summary>
        /// <param name="userUid"></param>
        /// <returns></returns>
        public IEnumerable<FapRole> GetUserRoles(string userUid)
        {

            string sql = "select * from FapRole where Fid in(select RoleUid  from FapRoleUser where UserUid=@UserUid)";
            DynamicParameters param = new DynamicParameters();
            param.Add("UserUid", userUid);
            var list = _dbContext.Query<FapRole>(sql, param);
            if (list == null)
            {
                list = new List<FapRole>();
            }
            var tempList = list.AsList();
            tempList.Insert(0, new FapRole { Id = -1, Fid = FapPlatformConstants.CommonUserRoleFid, RoleCode = "000", RoleName = "普通用户", RoleNote = "用户普通用户的授权" });
            return tempList;
        }
        /// <summary>
        /// 获取角色菜单
        /// </summary>
        /// <param name="roleUid"></param>
        /// <returns></returns>
        public IEnumerable<FapRoleMenu> GetRoleMenus(string roleUid)
        {
            return _rbacService.GetRoleMenuList(roleUid);
        }
    }
}
