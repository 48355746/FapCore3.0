using Fap.Core.DataAccess;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.Rbac.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fap.Core.Rbac.AC
{
    public class RoleUserSet : IRoleUserSet
    {
        private IEnumerable<FapRoleUser> _allRoleUser = new List<FapRoleUser>();
        private static readonly object Locker = new object();
        private bool _initialized;
        private IDbSession _dbSession;
        internal RoleUserSet(IDbSession dbSession)
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

                _allRoleUser = _dbSession.Query<FapRoleUser>("select * from FapRoleUser");

                _initialized = true;
            }
        }
        public IEnumerator<FapRoleUser> GetEnumerator()
        {
            return _allRoleUser.GetEnumerator();
        }

        public bool TryGetRoleValue(string userUid, out IEnumerable<string> roleUids)
        {
            if (!_initialized)
            {
                Init();
            }
            var roleUidList = _allRoleUser.Where(ru => ru.UserUid == userUid).Select(r => r.RoleUid).ToList();
            roleUidList.Insert(0, FapPlatformConstants.CommonUserRoleFid);
            roleUids = roleUidList;
            if (roleUids.Any())
            {
                return true;
            }
            return false;
        }

        public bool TryGetUserValue(string roleUid, out IEnumerable<string> userUids)
        {
            if (!_initialized)
            {
                Init();
            }
            userUids = _allRoleUser.Where(ru => ru.RoleUid == roleUid).Select(r => r.UserUid);
            if (userUids.Any())
            {
                return true;
            }
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _allRoleUser.GetEnumerator();
        }
    }
}
