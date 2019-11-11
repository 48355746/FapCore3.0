using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fap.Core.DataAccess;
using Fap.Core.Infrastructure.Config;

namespace Fap.Core.Rbac.AC
{
    public class CfgBillCodeRuleSet : ICfgBillCodeRuleSet
    {
        private IEnumerable<CfgBillCodeRule> _allCfgBillCodeRule = new List<CfgBillCodeRule>();
        private static readonly object Locker = new object();
        private bool _initialized;
        private IDbSession _dbSession;
        internal CfgBillCodeRuleSet(IDbSession dbSession)
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

                _allCfgBillCodeRule = _dbSession.Query<CfgBillCodeRule>("select * from CfgBillCodeRule");

                _initialized = true;
            }
        }
        public IEnumerator<CfgBillCodeRule> GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _allCfgBillCodeRule.GetEnumerator();
        }     

        public bool TryGetValue(string tableName, out IEnumerable<CfgBillCodeRule> billCodeRules)
        {
            if (!_initialized)
            {
                Init();
            }
            var result = _allCfgBillCodeRule.Where<CfgBillCodeRule>(f => f.BillEntity == tableName);
            if (result != null)
            {
                billCodeRules = result;
                return true;
            }
            billCodeRules = null;
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _allCfgBillCodeRule.GetEnumerator();
        }
    }
}
