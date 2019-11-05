using Fap.Core.DataAccess.BaseAccess;
using Fap.Core.DataAccess.DbContext;
using Fap.Core.Platform.Domain;
using Fap.Core.Rbac.Model;
/* ==============================================================================
 * 功能描述：全局角色角色
 * 创 建 者：wyf
 * 创建日期：2016/7/13 16:37:54
 * ==============================================================================*/
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fap.Core.Rbac.AC
{
    [Serializable]
    public class RoleRoleSet:IRoleRoleSet
    {
        private List<FapRoleRole> _allRoleRole = new List<FapRoleRole>();
        private static readonly object Locker = new object();
        private bool _initialized;
        private readonly IPlatformDomain _fapDomain;
        private ISessionFactory _sessionFactory;
        internal RoleRoleSet(IPlatformDomain fapDomain, ISessionFactory sessionFactory)
        {
            if (fapDomain == null)
            {
                throw new ArgumentNullException("fapDomain");
            }
            _fapDomain = fapDomain;
            _sessionFactory = sessionFactory;
            Init();
        }
        public void Refresh()
        {
            if (_initialized)
            {
                _initialized = false;
            }
        }
        private void Init()
        {
            if (_initialized) return;
            lock (Locker)
            {
                #region 获取所有FapRoleRole
                using (var session = _sessionFactory.CreateSession())
                {
                    _allRoleRole = session.QueryAll<FapRoleRole>().ToList();
                }
                #endregion
                _initialized = true;
            }
        }
        public bool TryGetValue(string fid, out Model.Infrastructure.FapRoleRole roleRole)
        {
            if (!_initialized)
            {
                Init();
            }
            var result = _allRoleRole.FirstOrDefault<FapRoleRole>(f => f.Fid == fid);
            if (result != null)
            {
                roleRole = result;
                return true;
            }
            roleRole = null;
            return false;
        }

        public bool TryGetValueByRole(string roleUid, out IEnumerable<Model.Infrastructure.FapRoleRole> roleRoles)
        {
            if (!_initialized)
            {
                Init();
            }
            var result = _allRoleRole.Where<FapRoleRole>(f => f.RoleUid == roleUid);
            if (result != null&&result.Any())
            {
                roleRoles = result;
                return true;
            }
            roleRoles = null;
            return false;
        }

        public IEnumerator<Model.Infrastructure.FapRoleRole> GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _allRoleRole.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _allRoleRole.GetEnumerator();
        }
    }
}
