using Fap.Core.DataAccess;
using Fap.Core.Rbac.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fap.Core.Rbac.AC
{
    public class MenuColumnSet : IMenuColumnSet
    {
        private IEnumerable<FapMenuColumn> _allColumn;
        private static readonly object Locker = new object();
        private bool _initialized;
        private readonly IDbSession _dbSession;
        internal MenuColumnSet(IDbSession dbSession)
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
                //获取所有菜单列
                _allColumn = _dbSession.Query<FapMenuColumn>("select * from FapMenuColumn");
                _initialized = true;
            }
        }
        public IEnumerator<FapMenuColumn> GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _allColumn.GetEnumerator();
        }


        public bool TryGetValue(string menuUid, out IEnumerable<FapMenuColumn> fapColumnList)
        {
            if (!_initialized)
            {
                Init();
            }
            fapColumnList = _allColumn.Where(m => m.MenuUid == menuUid);
            return fapColumnList.Any();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _allColumn.GetEnumerator();
        }
    }
}
