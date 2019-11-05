using Fap.Core.DataAccess;
using Fap.Core.Rbac.Model;
/* ==============================================================================
 * 功能描述：全局角色角色
 * 创 建 者：wyf
 * 创建日期：2016/7/13 16:37:54
 * ==============================================================================*/
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fap.Core.Rbac.AC
{
    [Serializable]
    public class RoleRoleSet : IRoleRoleSet
    {
        private IEnumerable<FapRoleRole> _allRoleRole = new List<FapRoleRole>();
        private static readonly object Locker = new object();
        private bool _initialized;
        private IDbSession _dbSession;
        internal RoleRoleSet(IDbSession dbSession)
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
                #region 获取所有FapRoleRole

                _allRoleRole = _dbSession.Query<FapRoleRole>("select * from FapRoleRole");

                #endregion
                _initialized = true;
            }
        }
        public bool TryGetValue(string fid, out FapRoleRole roleRole)
        {
            if (!_initialized)
            {
                Init();
            }
            var result = _allRoleRole.FirstOrDefault<FapRoleRole>(f => f.Fid == fid);
            if (result != null)
            {
                roleRole = result;
                return true;
            }
            roleRole = null;
            return false;
        }

        public bool TryGetValueByRole(string roleUid, out IEnumerable<FapRoleRole> roleRoles)
        {
            if (!_initialized)
            {
                Init();
            }
            var result = _allRoleRole.Where<FapRoleRole>(f => f.RoleUid == roleUid);
            if (result != null && result.Any())
            {
                roleRoles = result;
                return true;
            }
            roleRoles = null;
            return false;
        }

        public IEnumerator<FapRoleRole> GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _allRoleRole.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _allRoleRole.GetEnumerator();
        }
    }
}
