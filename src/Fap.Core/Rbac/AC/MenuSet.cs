using Fap.Core.DataAccess.BaseAccess;
using Fap.Core.DataAccess.DbContext;
using Fap.Core.Platform.Domain;
using Fap.Core.Rbac.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fap.Core.Rbac.AC
{
    [Serializable]
    public class MenuSet : IMenuSet
    {
        private List<FapMenu> _allMenus = new List<FapMenu>();
        private static readonly object Locker = new object();
        private bool _initialized;
        private readonly IPlatformDomain _fapDomain;
        private readonly ISessionFactory _sessionFactory;
        internal MenuSet(IPlatformDomain fapDomain, ISessionFactory sessionFactory)
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
                #region 获取所有menu
                using (var session = _sessionFactory.CreateSession())
                {
                    _allMenus = session.QueryWhere<FapMenu>($"ActiveFlag=1").ToList();
                }
                #endregion
                _initialized = true;
            }
        }
        public IEnumerator<Fap.Core.Rbac.Model.FapMenu> GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _allMenus.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _allMenus.GetEnumerator();
        }

        public bool TryGetValue(string fid, out Fap.Core.Rbac.Model.FapMenu fapMenu)
        {
            if (!_initialized)
            {
                Init();
            }
            var result = _allMenus.FirstOrDefault<FapMenu>(f => f.Fid == fid);
            if (result != null)
            {
                fapMenu = result;
                return true;
            }
            fapMenu = null;
            return false;
        }

        public bool TryGetValueByPath(string path, out FapMenu fapMenu)
        {
            if (!_initialized)
            {
                Init();
            }
            var result = _allMenus.FirstOrDefault<FapMenu>(f => f.MenuUrl == path);
            if (result != null)
            {
                fapMenu = result;
                return true;
            }
            fapMenu = null;
            return false;
        }
    }
}
