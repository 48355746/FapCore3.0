using Fap.Core.DataAccess.BaseAccess;
using Fap.Core.DataAccess.DbContext;
using Fap.Core.Platform.Domain;
using Fap.Core.Rbac.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fap.Core.Rbac.AC
{
    /// <summary>
    /// 系统用户，用于开发使用
    /// </summary>
    [Serializable]
    public class SysUserSet : ISysUserSet
    {
        private List<FapUser> _allUsers = new List<FapUser>();
        private static readonly object Locker = new object();
        private bool _initialized;
        private readonly IPlatformDomain _fapDomain;
        private ISessionFactory _sessionFactory;
        internal SysUserSet(IPlatformDomain fapDomain, ISessionFactory sessionFactory)
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
                using (var session = _sessionFactory.CreateSession())
                {
                    _allUsers = session.QueryAll<FapUser>().ToList();
                }
                _initialized = true;
            }
        }
        public IEnumerator<Fap.Core.Rbac.Model.FapUser> GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _allUsers.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _allUsers.GetEnumerator();
        }

        public bool TryGetValue(string fid, out Fap.Core.Rbac.Model.FapUser fapUser)
        {
            if (!_initialized)
            {
                Init();
            }
            var result = _allUsers.FirstOrDefault<FapUser>(f => f.Fid == fid);
            if (result != null)
            {
                fapUser = result;
                return true;
            }
            fapUser = null;
            return false;
        }


        public bool TryGetValueByUserName(string userName, out FapUser fapUser)
        {
            if (!_initialized)
            {
                Init();
            }
            var result = _allUsers.FirstOrDefault<FapUser>(f => f.UserName.Equals(userName,StringComparison.CurrentCultureIgnoreCase));
            if (result != null)
            {
                fapUser = result;
                return true;
            }
            fapUser = null;
            return false;
        }
    }
}
