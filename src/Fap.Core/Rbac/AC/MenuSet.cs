using Fap.Core.DataAccess;
using Fap.Core.DI;
using Fap.Model.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fap.Core.Rbac.AC
{
    public class MenuSet : IMenuSet
    {
        private IEnumerable<FapMenu> _allMenus = new List<FapMenu>();
        private static readonly object Locker = new object();
        private bool _initialized;
        private readonly IDbSession _dbSession;
        internal MenuSet(IDbSession dbSession)
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
                #region 获取所有menu

                _allMenus = _dbSession.Query<FapMenu>($"select * from FapMenu where ActiveFlag=1");

                #endregion
                _initialized = true;
            }
        }
        public IEnumerator<FapMenu> GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _allMenus.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _allMenus.GetEnumerator();
        }

        public bool TryGetValue(string fid, out FapMenu fapMenu)
        {
            if (!_initialized)
            {
                Init();
            }
            var result = _allMenus.FirstOrDefault<FapMenu>(f => f.Fid == fid);
            if (result != null)
            {
                fapMenu = result;
                return true;
            }
            fapMenu = null;
            return false;
        }

        public bool TryGetValueByPath(string path, out FapMenu fapMenu)
        {
            if (!_initialized)
            {
                Init();
            }
            var result = _allMenus.FirstOrDefault<FapMenu>(f => f.MenuUrl == path);
            if (result != null)
            {
                fapMenu = result;
                return true;
            }
            fapMenu = null;
            return false;
        }
    }
}
