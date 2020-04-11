using Fap.Core.DataAccess;
using Fap.Core.Rbac.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fap.Core.Rbac.AC
{
    /// <summary>
    /// 系统用户，用于开发使用
    /// </summary>
    [Serializable]
    public class UserSet : IUserSet
    {
        private IEnumerable<FapUser> _allUsers = new List<FapUser>();
        private static readonly object Locker = new object();
        private bool _initialized;
        private IDbSession _dbSession;
        internal UserSet(IDbSession dbSession)
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
               _allUsers = _dbSession.Query<FapUser>("select * from FapUser");
              
                _initialized = true;
            }
        }
        public IEnumerator<FapUser> GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _allUsers.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _allUsers.GetEnumerator();
        }

        public bool TryGetValue(string fid, out FapUser fapUser)
        {
            if (!_initialized)
            {
                Init();
            }
            var result = _allUsers.FirstOrDefault<FapUser>(f => f.Fid == fid);
            if (result != null)
            {
                fapUser = result;
                return true;
            }
            fapUser = null;
            return false;
        }


        public bool TryGetValueByUserName(string userName, out FapUser fapUser)
        {
            if (!_initialized)
            {
                Init();
            }
            var result = _allUsers.FirstOrDefault<FapUser>(f => f.UserName.Equals(userName,StringComparison.CurrentCultureIgnoreCase));
            if (result != null)
            {
                fapUser = result;
                return true;
            }
            fapUser = null;
            return false;
        }
    }
}
