using Fap.Core.DataAccess;
using Fap.Core.DI;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fap.Core.Rbac.AC
{
    public class ModuleSet : IModuleSet
    {
        private List<FapModule> _allModules = new List<FapModule>();
        private static readonly object Locker = new object();
        private bool _initialized;
        private readonly IDbSession _dbSession;
        private readonly IFapPlatformDomain _fapPlatformDomain;
        internal ModuleSet(IDbSession dbSession, IFapPlatformDomain fapPlatformDomain)
        {
            _dbSession = dbSession;
            _fapPlatformDomain = fapPlatformDomain;
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

                List<FapModule> allModules = _dbSession.Query<FapModule>($"select * from FapModule where ActiveFlag=1 and Dr=0 and ProductUid in('FAP','HCM')").ToList();
                _allModules = allModules;
                //根据注册码中的授权模块进行过滤
                //List<string> authoredModules = _fapPlatformDomain.ServiceRegisterInfo.AuthoredModules;
                //if (authoredModules != null)
                //{
                //    _allModules.Clear();
                //    List<FapModule> modules = allModules.Where(m => authoredModules.Contains(m.ModuleCode)).ToList();
                //    _allModules.AddRange(modules);
                //}

                #endregion
                _initialized = true;
            }
        }
        public IEnumerator<FapModule> GetEnumerator()
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

        public bool TryGetValue(string fid, out FapModule fapModule)
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
