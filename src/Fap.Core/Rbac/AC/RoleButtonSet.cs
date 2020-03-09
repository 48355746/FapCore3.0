using Fap.Core.DataAccess;
using Fap.Core.Rbac.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fap.Core.Rbac.AC
{
    public class RoleButtonSet : IRoleButtonSet
    {
        private IEnumerable<FapRoleButton> fapRoleButtons;
        private static readonly object Locker = new object();
        private bool _initialized;
        private readonly IDbSession _dbSession;
        public RoleButtonSet(IDbSession dbSession)
        {
            _dbSession = dbSession;
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

                fapRoleButtons = _dbSession.Query<FapRoleButton>("select * from FapRoleButton");

                _initialized = true;
            }
        }
        public IEnumerator<FapRoleButton> GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return fapRoleButtons.GetEnumerator();
        }

        public bool TryGetValue(string roleUid, out IEnumerable<FapRoleButton> roleButtons)
        {
            if (!_initialized)
            {
                Init();
            }
            roleButtons = fapRoleButtons.Where(rb => rb.RoleUid == roleUid);
            return roleButtons.Any();
           
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return fapRoleButtons.GetEnumerator();
        }
    }
}
