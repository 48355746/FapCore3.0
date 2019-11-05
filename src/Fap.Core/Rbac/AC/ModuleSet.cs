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
    public class ModuleSet : IModuleSet
    {
        private List<FapModule> _allModules = new List<FapModule>();
        private static readonly object Locker = new object();
        private bool _initialized;
        private readonly IPlatformDomain _fapDomain;
        private readonly ISessionFactory _sessionFactory;
        internal ModuleSet(IPlatformDomain fapDomain, ISessionFactory sessionFactory)
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
                #region 获取所有module
                using (var session = _sessionFactory.CreateSession())
                {
                    List<FapModule> allModules = session.QueryWhere<FapModule>($"ActiveFlag=1 and ProductUid in('FAP','{_fapDomain.Product }')").ToList();

                    //根据注册码中的授权模块进行过滤
                    List<string> authoredModules = _fapDomain.ServiceRegisterInfo.AuthoredModules;
                    if (authoredModules != null)
                    {
                        _allModules.Clear();
                        List<FapModule> modules = allModules.Where(m => authoredModules.Contains(m.ModuleCode)).ToList();
                        _allModules.AddRange(modules);
                    }
                }
                #endregion
                _initialized = true;
            }
        }
        public IEnumerator<Fap.Core.Rbac.Model.FapModule> GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _allModules.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _allModules.GetEnumerator();
        }

        public bool TryGetValue(string fid, out Fap.Core.Rbac.Model.FapModule fapModule)
        {
            if (!_initialized)
            {
                Init();
            }
            var result = _allModules.FirstOrDefault<FapModule>(f => f.Fid == fid);
            if (result != null)
            {
                fapModule = result;
                return true;
            }
            fapModule = null;
            return false;
        }
    }
}
