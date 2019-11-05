using Dapper;
using Fap.Core.DataAccess.DbContext;
using Fap.Core.Infrastructure.Constants;
using Fap.Core.Platform.Domain;
using Fap.Core.Rbac.Model;
using System.Collections.Generic;
using System.Linq;

namespace Fap.Core.Rbac
{
    public class LoginService:ILoginService
    {
        IDbContext _dbContext = null;
        public LoginService(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public Fap.Core.Rbac.Model.FapUser Login(string userName)
        {
           string where = "UserName=@UserName";
            DynamicParameters param=new  DynamicParameters();
            param.Add("UserName",userName);
            var users = _dbContext.QueryWhere<FapUser>(where, param);
            if(users!=null&&users.Any())
            {
                return users.First();
            }
            return null;
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
            if(user.PasswordTryTimes>5)
            {
                user.IsLocked = 1;
            }
            string sql = "update FapUser set passwordtrytimes=@trytimes,islocked=@islocked where id=@id";
            _dbContext.Execute(sql,new DynamicParameters(new { trytimes = user.PasswordTryTimes, islocked = user.IsLocked, id = user.Id }));
            return user;
            //return service.UpdateEntity<FapUser>(user, false);
        }
        /// <summary>
        /// 更新最后登录时间
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public FapUser UpdateLastLoginTime(FapUser user)
        {
            string sql = "update FapUser set LastLoginTime=@lastTime, passwordtrytimes=@tryTimes where id=@id";
            _dbContext.Execute(sql,new DynamicParameters(new { lastTime = user.LastLoginTime, tryTimes = user.PasswordTryTimes, id = user.Id }));
            return user;
        }
        public void Logout()
        {
            
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
            var list=_dbContext.Query<FapRole>(sql, param);
            if(list==null)
            {
                list = new List<FapRole>();
            }
            var tempList = list.AsList();
            tempList.Insert(0, new FapRole { Id = -1, Fid = PlatformConstants.CommonUserRoleFid, RoleCode="000", RoleName="普通用户", RoleNote="用户普通用户的授权" });
            return tempList;
        }
        /// <summary>
        /// 获取角色菜单
        /// </summary>
        /// <param name="roleUid"></param>
        /// <returns></returns>
        public IEnumerable<FapRoleMenu> GetRoleMenus(string roleUid)
        {
            //string sql = "select * from FapMenu where Fid in(select menuUid from FapRoleMenu where RoleUid=@RoleUid)";
            DynamicParameters param = new DynamicParameters();
            param.Add("RoleUid", roleUid);
            var list = _dbContext.QueryWhere<FapRoleMenu>("RoleUid=@RoleUid", param);
            return list;
        }
    }
}
