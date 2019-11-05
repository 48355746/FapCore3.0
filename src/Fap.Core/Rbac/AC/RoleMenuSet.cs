using Fap.Core.DataAccess.BaseAccess;
using Fap.Core.DataAccess.DbContext;
using Fap.Core.Platform.Domain;
using Fap.Core.Rbac.Model;
/* ==============================================================================
 * 功能描述：全局角色菜单
 * 创 建 者：wyf
 * 创建日期：2016/7/13 16:37:54
 * ==============================================================================*/
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fap.Core.Rbac.AC
{
    [Serializable]
    public class RoleMenuSet:IRoleMenuSet
    {
        private List<FapRoleMenu> _allRoleMenu = new List<FapRoleMenu>();
        private static readonly object Locker = new object();
        private bool _initialized;
        private readonly IPlatformDomain _fapDomain;
        private ISessionFactory _sessionFactory;
        internal RoleMenuSet(IPlatformDomain fapDomain, ISessionFactory sessionFactory)
        {
            if (fapDomain == null)
            {
                throw new ArgumentNullException("fapDomain");
            }
            _sessionFactory = sessionFactory;
            _fapDomain = fapDomain;
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
                #region 获取所有FapRoleMenu
                using (var session = _sessionFactory.CreateSession())
                {
                    _allRoleMenu = session.QueryAll<FapRoleMenu>().ToList();
                }
                #endregion
                _initialized = true;
            }
        }
        public bool TryGetValue(string fid, out Model.Infrastructure.FapRoleMenu roleMenu)
        {
            if (!_initialized)
            {
                Init();
            }
            var result = _allRoleMenu.FirstOrDefault<FapRoleMenu>(f => f.Fid == fid);
            if (result != null)
            {
                roleMenu = result;
                return true;
            }
            roleMenu = null;
            return false;
        }

        public bool TryGetValueByRole(string roleUid, out IEnumerable<Model.Infrastructure.FapRoleMenu> roleMenus)
        {
            if (!_initialized)
            {
                Init();
            }
            var result = _allRoleMenu.Where<FapRoleMenu>(f => f.RoleUid == roleUid);
            if (result != null&&result.Any())
            {
                roleMenus = result;
                return true;
            }
            roleMenus = null;
            return false;
        }

        public IEnumerator<Model.Infrastructure.FapRoleMenu> GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _allRoleMenu.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _allRoleMenu.GetEnumerator();
        }
    }
}
