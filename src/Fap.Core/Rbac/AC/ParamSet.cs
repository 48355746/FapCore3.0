using Fap.Core.DataAccess;
using Fap.Core.Infrastructure.Config;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fap.Core.Rbac.AC
{
    [Serializable]
    public class ParamSet : IParamSet
    {
        private IEnumerable<FapConfig> _allParams = new List<FapConfig>();
        private static readonly object Locker = new object();
        private bool _initialized;
        private IDbSession _dbSession;
        internal ParamSet(IDbSession dbSession)
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
                #region 获取所有FapConfig
                    _allParams = _dbSession.Query<FapConfig>("select * from FapConfig");
                #endregion
                _initialized = true;
            }
        }
        public IEnumerator<FapConfig> GetEnumerator()
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

        public bool TryGetValue(string fid, out FapConfig fapParam)
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
