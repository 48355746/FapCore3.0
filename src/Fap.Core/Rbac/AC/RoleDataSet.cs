using Fap.Core.DataAccess;
using Fap.Core.Rbac.Model;
/* ==============================================================================
 * 功能描述：  
 * 创 建 者：wyf
 * 创建日期：2016/11/30 19:04:06
 * ==============================================================================*/
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fap.Core.Rbac.AC
{
    [Serializable]
    public class RoleDataSet : IRoleDataSet
    {
        private IEnumerable<FapRoleData> _allRoleDatas = new List<FapRoleData>();
        private static readonly object Locker = new object();
        private bool _initialized;
        private IDbSession _dbSession;
        internal RoleDataSet(IDbSession dbSession)
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
                #region 获取所有

                _allRoleDatas = _dbSession.Query<FapRoleData>("select * from FapRoleData");

                if (_allRoleDatas == null)
                {
                    _allRoleDatas = new List<FapRoleData>();
                }
                #endregion
                _initialized = true;
            }
        }
        public bool TryGetValue(string fid, out FapRoleData roleData)
        {
            if (!_initialized)
            {
                Init();
            }
            var result = _allRoleDatas.FirstOrDefault<FapRoleData>(f => f.Fid == fid);
            if (result != null)
            {
                roleData = result;
                return true;
            }
            roleData = null;
            return false;
        }

        public bool TryGetValueByRole(string roleUid, out IEnumerable<FapRoleData> roleDatas)
        {
            if (!_initialized)
            {
                Init();
            }
            var result = _allRoleDatas.Where<FapRoleData>(f => f.RoleUid == roleUid);
            if (result != null && result.Any())
            {
                roleDatas = result;
                return true;
            }
            roleDatas = null;
            return false;
        }

        public IEnumerator<FapRoleData> GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _allRoleDatas.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _allRoleDatas.GetEnumerator();
        }
    }
}
