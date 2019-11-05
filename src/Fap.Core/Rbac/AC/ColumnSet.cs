using Fap.Core.DataAccess.BaseAccess;
using Fap.Core.DataAccess.DbContext;
using Fap.Core.Platform.Domain;
using Fap.Model.MetaData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Fap.Core.Rbac.AC
{
    [Serializable]
    public class ColumnSet:IColumnSet
    {
        private List<FapColumn> _allColumns = new List<FapColumn>();
        private static readonly object Locker = new object();
        private bool _initialized;
        private readonly IPlatformDomain _fapDomain;
        private ISessionFactory _sessionFactory;
        internal ColumnSet(IPlatformDomain fapDomain, ISessionFactory sessionFactory)
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
                    _allColumns = session.QueryAll<FapColumn>().ToList();
                }
                _initialized = true;
            }
        }
        public IEnumerator<Fap.Model.MetaData.FapColumn> GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _allColumns.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            if (!_initialized)
            {
                Init();
            }
            return _allColumns.GetEnumerator();
        }

        public bool TryGetValue(string fid, out Fap.Model.MetaData.FapColumn fapColumn)
        {
            if (!_initialized)
            {
                Init();
            }
            var result = _allColumns.FirstOrDefault<FapColumn>(f => f.Fid == fid);
            if (result != null)
            {
                fapColumn = result;
                return true;
            }
            fapColumn = null;
            return false;
        }


        public bool TryGetValueByTable(string tableName, out List<FapColumn> fapColumns)
        {
            if (!_initialized)
            {
                Init();
            }
            var result = _allColumns.Where<FapColumn>(c => c.TableName.Equals(tableName,StringComparison.CurrentCultureIgnoreCase));
            if (result != null&&result.Any())
            {
                fapColumns = result.ToList<FapColumn>();
                return true;
            }
            fapColumns = null;
            return false;
        }


        public bool TryGetValue(int id, out FapColumn fapColumn)
        {
            if (!_initialized)
            {
                Init();
            }
            var result = _allColumns.FirstOrDefault<FapColumn>(f => f.Id == id);
            if (result != null)
            {
                fapColumn = result;
                return true;
            }
            fapColumn = null;
            return false;
        }
    }
}
