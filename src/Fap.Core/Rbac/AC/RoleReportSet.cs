using Fap.Core.DataAccess.BaseAccess;
using Fap.Core.DataAccess.DbContext;
using Fap.Core.Platform.Domain;
using Fap.Core.Rbac.Model;
/* ==============================================================================
 * 功能描述：  
 * 创 建 者：wyf
 * 创建日期：2016/9/19 20:02:17
 * ==============================================================================*/
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fap.Core.Rbac.AC
{
    [Serializable]
    public class RoleReportSet:IRoleReportSet
    {
        private List<FapRoleReport> _allRoleRpt = new List<FapRoleReport>();
        private static readonly object Locker = new object();
        private bool _initialized;
        private readonly IPlatformDomain _fapDomain;
        private ISessionFactory _sessionFactory;
        internal RoleReportSet(IPlatformDomain fapDomain, ISessionFactory sessionFactory)
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
                #region 获取所有FapRoleReport
                using (var session = _sessionFactory.CreateSession())
                {
                    _allRoleRpt = session.QueryAll<FapRoleReport>().ToList();
                }
                #endregion
                _initialized = true;
            }
        }
        public bool TryGetValue(string fid, out Model.Infrastructure.FapRoleReport roleRpt)
        {
            if (!_initialized)
            {
                Init();
            }
            var result = _allRoleRpt.FirstOrDefault<FapRoleReport>(f => f.Fid == fid);
            if (result != null)
            {
                roleRpt = result;
                return true;
            }
            roleRpt = null;
            return false;
        }

        public bool TryGetValueByRole(string roleUid, out IEnumerable<Model.Infrastructure.FapRoleReport> roleRpts)
        {
            if (!_initialized)
            {
                Init();
            }
            var result = _allRoleRpt.Where<FapRoleReport>(f => f.RoleUid == roleUid);
            if (result != null && result.Any())
            {
                roleRpts = result;
                return true;
            }
            roleRpts = null;
            return false;
        }

        public IEnumerator<Model.Infrastructure.FapRoleReport> GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _allRoleRpt.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _allRoleRpt.GetEnumerator();
        }
    }
}
