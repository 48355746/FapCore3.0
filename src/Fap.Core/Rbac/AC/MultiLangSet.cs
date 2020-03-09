using Fap.Core.DataAccess;
using Fap.Core.DI;
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
    public class MultiLangSet : IMultiLanguage
    {
        private IEnumerable<FapMultiLanguage> _allMultiLangs;
        private static readonly object Locker = new object();
        private bool _initialized;
        private IDbSession _dbSession;
        internal MultiLangSet(IDbSession dbSession)
        {
            _dbSession = dbSession;
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
                _allMultiLangs = _dbSession.Query<FapMultiLanguage>("select * from FapMultiLanguage where Dr=0");

                _initialized = true;
            }
        }
        public IEnumerator<FapMultiLanguage> GetEnumerator()
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

        public bool TryGetValue(string qualifier, string langKey, out FapMultiLanguage fapMultiLang)
        {
            if (!_initialized)
            {
                Init();
            }
            var result = _allMultiLangs.FirstOrDefault<FapMultiLanguage>(f =>
            f.Qualifier.Equals(qualifier, StringComparison.CurrentCultureIgnoreCase) &&
            f.LangKey.Equals(langKey, StringComparison.CurrentCultureIgnoreCase));
            if (result != null)
            {
                fapMultiLang = result;
                return true;
            }
            fapMultiLang = null;
            return false;
        }

        public bool TryGetValue(string qualifier, out IEnumerable<FapMultiLanguage> multiLanguages)
        {
            if (!_initialized)
            {
                Init();
            }
            var result = _allMultiLangs.Where<FapMultiLanguage>(f =>
            f.Qualifier.Equals(qualifier, StringComparison.CurrentCultureIgnoreCase));
            if (result != null)
            {
                multiLanguages = result;
                return true;
            }
            multiLanguages = null;
            return false;
        }
    }
}
