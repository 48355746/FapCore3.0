using Fap.Core.DataAccess.BaseAccess;
using Fap.Core.DataAccess.DbContext;
using Fap.Core.Extensions;
using Fap.Core.Platform.Domain;
using Fap.Core.Rbac.Model;
/* ==============================================================================
 * 功能描述：  
 * 创 建 者：wyf
 * 创建日期：2016/7/13 15:30:17
 * ==============================================================================*/
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fap.Core.Rbac.AC
{
    [Serializable]
    public class RoleColumnSet : IRoleColumnSet
    {
        private List<FapRoleColumn> _allRoleColumn = new List<FapRoleColumn>();
        private static readonly object Locker = new object();
        private bool _initialized;
        private readonly IPlatformDomain _fapDomain;
        private ISessionFactory _sessionFactory;
        internal RoleColumnSet(IPlatformDomain fapDomain, ISessionFactory sessionFactory)
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
            lock (Locker)
            {
                if (_initialized) return;
                #region 获取所有FapRoleColumn
                using (var session = _sessionFactory.CreateSession())
                {
                    _allRoleColumn = session.QueryAll<FapRoleColumn>().ToList();
                }
                #endregion
                _initialized = true;
            }
        }
        public bool TryGetValue(string fid, out Model.Infrastructure.FapRoleColumn roleColumn)
        {
            if (!_initialized)
            {
                Init();
            }
            var result = _allRoleColumn.FirstOrDefault<FapRoleColumn>(f => f.Fid == fid);
            if (result != null)
            {
                roleColumn = result;
                return true;
            }
            roleColumn = null;
            return false;
        }

        public bool TryGetValueByRole(string roleUid, out IEnumerable<Model.Infrastructure.FapRoleColumn> roleColumns)
        {
            if (!_initialized)
            {
                Init();
            }
            var result = _allRoleColumn.Where<FapRoleColumn>(f => f.RoleUid == roleUid);
            if (result != null&&result.Any())
            {
                roleColumns = result;
                return true;
            }
            roleColumns = null;
            return false;
        }

        public IEnumerator<Model.Infrastructure.FapRoleColumn> GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _allRoleColumn.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _allRoleColumn.GetEnumerator();
        }
    }
}
