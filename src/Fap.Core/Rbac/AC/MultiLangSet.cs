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
    public class MultiLangSet : IMultiLang
    {
        private IEnumerable<FapResMultiLang> _allMultiLangs = new List<FapResMultiLang>();
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
                _allMultiLangs = _dbSession.Query<FapResMultiLang>("select * from FapResMultiLang");

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
            var result = _allMultiLangs.FirstOrDefault<FapResMultiLang>(f => f.ResCode.Equals(code, StringComparison.CurrentCultureIgnoreCase));
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
