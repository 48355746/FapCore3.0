using Fap.Core.DataAccess;
using Fap.Core.DI;
using Fap.Core.Rbac.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fap.Core.Rbac.AC
{
    public class MenuButtonSet : IMenuButtonSet
    {
        private IEnumerable<FapMenuButton> _allButtons;
        private static readonly object Locker = new object();
        private bool _initialized;
        private readonly IDbSession _dbSession;
        internal MenuButtonSet(IDbSession dbSession)
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
                //获取所有按钮
                _allButtons = _dbSession.Query<FapMenuButton>("select * from FapMenuButton");
                _initialized = true;
            }
        }
        public IEnumerator<FapMenuButton> GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _allButtons.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _allButtons.GetEnumerator();
        }      

        public bool TryGetValue(string menuUid, out IEnumerable<FapMenuButton> fapButtonList)
        {
            if (!_initialized)
            {
                Init();
            }
            fapButtonList = _allButtons.Where(m => m.MenuUid == menuUid);
            return fapButtonList.Any();
        }
    }
}
