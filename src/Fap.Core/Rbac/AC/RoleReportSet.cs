using Fap.Core.DataAccess;
using Fap.Core.Rbac.Model;
/* ==============================================================================
 * 功能描述：  
 * 创 建 者：wyf
 * 创建日期：2016/9/19 20:02:17
 * ==============================================================================*/
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fap.Core.Rbac.AC
{
    [Serializable]
    public class RoleReportSet : IRoleReportSet
    {
        private IEnumerable<FapRoleReport> _allRoleRpt = new List<FapRoleReport>();
        private static readonly object Locker = new object();
        private bool _initialized;
        private IDbSession _dbSession;
        internal RoleReportSet(IDbSession dbSession)
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
                #region 获取所有FapRoleReport

                _allRoleRpt = _dbSession.Query<FapRoleReport>("select * from FapRoleReport").ToList();

                #endregion
                _initialized = true;
            }
        }
        public bool TryGetValue(string fid, out FapRoleReport roleRpt)
        {
            if (!_initialized)
            {
                Init();
            }
            var result = _allRoleRpt.FirstOrDefault<FapRoleReport>(f => f.Fid == fid);
            if (result != null)
            {
                roleRpt = result;
                return true;
            }
            roleRpt = null;
            return false;
        }

        public bool TryGetValueByRole(string roleUid, out IEnumerable<FapRoleReport> roleRpts)
        {
            if (!_initialized)
            {
                Init();
            }
            var result = _allRoleRpt.Where<FapRoleReport>(f => f.RoleUid == roleUid);
            if (result != null && result.Any())
            {
                roleRpts = result;
                return true;
            }
            roleRpts = null;
            return false;
        }

        public IEnumerator<FapRoleReport> GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _allRoleRpt.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _allRoleRpt.GetEnumerator();
        }
    }
}
