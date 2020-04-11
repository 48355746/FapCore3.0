using Fap.Core.DataAccess;
using Fap.Core.Rbac.Model;
using Fap.Core.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fap.Core.Rbac.AC
{
    public class OrgDeptSet:IOrgDeptSet
    {
        private IEnumerable<OrgDept> _allOrgs = new List<OrgDept>();
        private static readonly object Locker = new object();
        private bool _initialized;
        private IDbSession _dbSession;
        internal OrgDeptSet(IDbSession dbSession)
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
                string cdate = DateTimeUtils.CurrentDateTimeStr;
                _allOrgs = _dbSession.Query<OrgDept>($"select * from OrgDept where EnableDate<'{cdate}' and DisableDate>'{cdate}' and Dr=0");
                        
                _initialized = true;
            }
        }
        public IEnumerator<OrgDept> GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _allOrgs.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _allOrgs.GetEnumerator();
        }

        public bool TryGetValue(string fid, out OrgDept fapOrg)
        {
            if (!_initialized)
            {
                Init();
            }
            var result = _allOrgs.FirstOrDefault<OrgDept>(f => f.Fid == fid);
            if (result != null)
            {
                fapOrg = result;
                return true;
            }
            fapOrg = null;
            return false;
        }

        public bool TryGetValueByPid(string pid, out IEnumerable<OrgDept> childDepts)
        {
            if (!_initialized)
            {
                Init();
            }
            childDepts= _allOrgs.Where(d => d.Pid == pid);
            return childDepts.Any();
        }
    }
}
