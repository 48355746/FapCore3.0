using Fap.Core.DataAccess.BaseAccess;
using Fap.Core.DataAccess.DbContext;
using Fap.Core.Platform.Domain;
using Fap.Model.MetaData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fap.Core.Rbac.AC
{
    [Serializable]
    public class TableSet : ITableSet
    {
        private List<FapTable> _allTables = new List<FapTable>();
        private static readonly object Locker = new object();
        private bool _initialized;
        private readonly IPlatformDomain _fapDomain;
        private ISessionFactory _sessionFactory;
        internal TableSet(IPlatformDomain fapDomain, ISessionFactory sessionFactory)
        {
            if (fapDomain == null)
            {
                throw new ArgumentNullException("fapDomain");
            }
            _sessionFactory = sessionFactory;
            _fapDomain = fapDomain;
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
                using (var session = _sessionFactory.CreateSession())
                {
                    _allTables = session.QueryWhere<FapTable>($"ProductUid in('FAP','{_fapDomain.Product }')").ToList();
                }
                _initialized = true;
            }
        }
        public IEnumerator<Fap.Model.MetaData.FapTable> GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _allTables.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _allTables.GetEnumerator();
        }

        public bool TryGetValue(string fid, out Fap.Model.MetaData.FapTable fapTable)
        {
            if (!_initialized)
            {
                Init();
            }
            var result = _allTables.FirstOrDefault<FapTable>(f => f.Fid == fid);
            if (result != null)
            {
                fapTable = result;
                return true;
            }
            fapTable = null;
            return false;
        }


        public bool TryGetValueByName(string tableName, out FapTable fapTable)
        {
            if (!_initialized)
            {
                Init();
            }
            var result = _allTables.FirstOrDefault<FapTable>(f => f.TableName.Equals(tableName,StringComparison.CurrentCultureIgnoreCase));
            if (result != null)
            {
                fapTable = result;
                return true;
            }
            fapTable = null;
            return false;
        }
    }
}
