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
    public class OrgDeptSet:IOrgDeptSet
    {
        private List<OrgDept> _allOrgs = new List<OrgDept>();
        private static readonly object Locker = new object();
        private bool _initialized;
        private readonly IPlatformDomain _fapDomain;
        private ISessionFactory _sessionFactory;
        internal OrgDeptSet(IPlatformDomain fapDomain, ISessionFactory sessionFactory)
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
                using (var session = _sessionFactory.CreateSession())
                {
                    _allOrgs = session.QueryAll<OrgDept>().ToList();
                }               
                _initialized = true;
            }
        }
        public IEnumerator<Fap.Core.Rbac.Model.OrgDept> GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _allOrgs.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _allOrgs.GetEnumerator();
        }

        public bool TryGetValue(string fid, out Fap.Core.Rbac.Model.OrgDept fapOrg)
        {
            if (!_initialized)
            {
                Init();
            }
            var result = _allOrgs.FirstOrDefault<OrgDept>(f => f.Fid == fid);
            if (result != null)
            {
                fapOrg = result;
                return true;
            }
            fapOrg = null;
            return false;
        }
    }
}
