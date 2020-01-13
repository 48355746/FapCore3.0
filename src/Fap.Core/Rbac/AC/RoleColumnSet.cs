using Fap.Core.DataAccess;
using Fap.Core.Rbac.Model;
/* ==============================================================================
 * 功能描述：  
 * 创 建 者：wyf
 * 创建日期：2016/7/13 15:30:17
 * ==============================================================================*/
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fap.Core.Rbac.AC
{
    [Serializable]
    public class RoleColumnSet : IRoleColumnSet
    {
        private IEnumerable<FapRoleColumn> _allRoleColumn = new List<FapRoleColumn>();
        private static readonly object Locker = new object();
        private bool _initialized;
        private IDbSession _dbSession;
        internal RoleColumnSet(IDbSession dbSession)
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
            lock (Locker)
            {
                if (_initialized) return;
                #region 获取所有FapRoleColumn

                _allRoleColumn = _dbSession.Query<FapRoleColumn>("select * from FapRoleColumn");

                #endregion
                _initialized = true;
            }
        }
        public bool TryGetValue(string fid, out FapRoleColumn roleColumn)
        {
            if (!_initialized)
            {
                Init();
            }
            var result = _allRoleColumn.FirstOrDefault<FapRoleColumn>(f => f.Fid == fid);
            if (result != null)
            {
                roleColumn = result;
                return true;
            }
            roleColumn = null;
            return false;
        }

        public bool TryGetValueByRole(string roleUid, out IEnumerable<FapRoleColumn> roleColumns)
        {
            if (!_initialized)
            {
                Init();
            }
            roleColumns = _allRoleColumn.Where<FapRoleColumn>(f => f.RoleUid == roleUid);
            return roleColumns.Any();
        }

        public IEnumerator<FapRoleColumn> GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _allRoleColumn.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _allRoleColumn.GetEnumerator();
        }
    }
}
