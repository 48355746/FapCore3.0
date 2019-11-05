using Fap.Core.DataAccess;
using Fap.Core.DI;
using Fap.Core.Rbac.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fap.Core.Rbac.AC
{
    public class ButtonSet : IButtonSet
    {
        private IEnumerable<FapButton> _allButtons = new List<FapButton>();
        private static readonly object Locker = new object();
        private bool _initialized;
        private readonly IDbSession _dbSession;
        internal ButtonSet(IDbSession dbSession)
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
                //_allButtons.Clear();
                
                //获取所有按钮，未实现
                //_allButtons = service.Query<FapButton>("");
                _initialized = true;
            }
        }
        public IEnumerator<Fap.Core.Rbac.Model.FapButton> GetEnumerator()
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

        public bool TryGetValue(string fid, out Fap.Core.Rbac.Model.FapButton fapButton)
        {
            if (!_initialized)
            {
                Init();
            }
            var result = _allButtons.FirstOrDefault<FapButton>(f => f.Fid == fid);
            if (result != null)
            {
                fapButton = result;
                return true;
            }
            fapButton = null;
            return false;
        }
    }
}
