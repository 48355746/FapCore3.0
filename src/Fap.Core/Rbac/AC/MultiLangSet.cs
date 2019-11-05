using Fap.Core.DataAccess.BaseAccess;
using Fap.Core.DataAccess.DbContext;
using Fap.Core.Platform.Domain;
using Fap.Core.Rbac.Model;
/* ==============================================================================
 * 功能描述：  
 * 创 建 者：wyf
 * 创建日期：2017-05-11 18:34:19
 * ==============================================================================*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Fap.Core.Rbac.AC
{
    [Serializable]
    public class MultiLangSet:IMultiLang
    {
        private List<FapResMultiLang> _allMultiLangs = new List<FapResMultiLang>();
        private static readonly object Locker = new object();
        private bool _initialized;
        private readonly IPlatformDomain _fapDomain;
        private ISessionFactory _sessionFactory;
        internal MultiLangSet(IPlatformDomain fapDomain, ISessionFactory sessionFactory)
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
                    _allMultiLangs = session.QueryAll<FapResMultiLang>().ToList();
                }
                _initialized = true;
            }
        }
        public IEnumerator<Fap.Core.Rbac.Model.FapResMultiLang> GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _allMultiLangs.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _allMultiLangs.GetEnumerator();
        }

        public bool TryGetValue(string fid, out Fap.Core.Rbac.Model.FapResMultiLang fapMultiLang)
        {
            if (!_initialized)
            {
                Init();
            }
            var result = _allMultiLangs.FirstOrDefault<FapResMultiLang>(f => f.Fid == fid);
            if (result != null)
            {
                fapMultiLang = result;
                return true;
            }
            fapMultiLang = null;
            return false;
        }


        public bool TryGetValueByCode(string code, out FapResMultiLang fapMultiLang)
        {
            if (!_initialized)
            {
                Init();
            }
            var result = _allMultiLangs.FirstOrDefault<FapResMultiLang>(f => f.ResCode.Equals(code,StringComparison.CurrentCultureIgnoreCase));
            if (result != null)
            {
                fapMultiLang = result;
                return true;
            }
            fapMultiLang = null;
            return false;
        }
    }
}
