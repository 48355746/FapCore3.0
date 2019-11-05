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
    public class SysParamSet : ISysParamSet
    {
        private List<FapConfig> _allParams = new List<FapConfig>();
        private static readonly object Locker = new object();
        private bool _initialized;
        private readonly IPlatformDomain _fapDomain;
        private ISessionFactory _sessionFactory;
        internal SysParamSet(IPlatformDomain fapDomain, ISessionFactory sessionFactory)
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
                #region 获取所有FapConfig
                using (var session = _sessionFactory.CreateSession())
                {
                    _allParams = session.QueryAll<FapConfig>().ToList();
                }
                #endregion
                _initialized = true;
            }
        }
        public IEnumerator<Fap.Core.Rbac.Model.FapConfig> GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _allParams.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _allParams.GetEnumerator();
        }

        public bool TryGetValue(string fid, out Fap.Core.Rbac.Model.FapConfig fapParam)
        {
            if (!_initialized)
            {
                Init();
            }
            var result = _allParams.FirstOrDefault<FapConfig>(f => f.Fid == fid);
            if (result != null)
            {
                fapParam = result;
                return true;
            }
            fapParam = null;
            return false;
        }


        public bool TryGetValueByKey(string key, out FapConfig fapParam)
        {
            if (!_initialized)
            {
                Init();
            }
            var result = _allParams.FirstOrDefault<FapConfig>(f => f.ParamKey == key);
            if (result != null)
            {
                fapParam = result;
                return true;
            }
            fapParam = null;
            return false;
        }
    }
}
