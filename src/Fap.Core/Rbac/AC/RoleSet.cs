using Fap.Core.DataAccess.BaseAccess;
using Fap.Core.DataAccess.DbContext;
using Fap.Core.Infrastructure.Constants;
using Fap.Core.Platform.Domain;
using Fap.Core.Rbac.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fap.Core.Rbac.AC
{
    [Serializable]
    public class RoleSet : IRoleSet
    {
        private List<FapRole> _allRoles = new List<FapRole>();
        private static readonly object Locker = new object();
        private bool _initialized;
        private readonly IPlatformDomain _fapDomain;
        private ISessionFactory _sessionFactory;
        internal RoleSet(IPlatformDomain fapDomain, ISessionFactory sessionFactory)
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
                #region 获取所有FapRole
                using (var session = _sessionFactory.CreateSession())
                {
                    _allRoles = session.QueryAll<FapRole>().ToList();
                }
                if(_allRoles==null)
                {
                    _allRoles = new List<FapRole>();
                }
                //添加普通用户
                _allRoles.Insert(0, new FapRole { Id = -1, Fid = PlatformConstants.CommonUserRoleFid, RoleCode = "000", RoleName = "普通用户", RoleNote = "用户普通用户的授权" });
                #endregion
                _initialized = true;
            }
        }
        public IEnumerator<Fap.Core.Rbac.Model.FapRole> GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _allRoles.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _allRoles.GetEnumerator();
        }

        public bool TryGetValue(string fid, out Fap.Core.Rbac.Model.FapRole fapRole)
        {
            if (!_initialized)
            {
                Init();
            }
            var result = _allRoles.FirstOrDefault<FapRole>(f => f.Fid == fid);
            if (result != null)
            {
                fapRole = result;
                return true;
            }
            fapRole = null;
            return false;
        }
    }
}
